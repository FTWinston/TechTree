using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Linq;

using FeatureData = System.Collections.Generic.Dictionary<string, int>;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public abstract class Feature : ISelectable
    {
        public abstract string Name { get; }
        
        public abstract string Description { get; }
        
        public abstract string Symbol { get; }

        public abstract FeatureType Type { get; }

        public virtual Research? UnlockedBy { get; internal set; }

        public abstract FeatureState DetermineState(Entity entity);

        public virtual void Initialize(EntityType entityType) { }

        public virtual void Unlock(Entity entity) { }

        public bool TryTrigger(Entity entity, Cell target)
        {
            var data = GetData(entity);

            if (!CanTrigger(entity, target, data))
                return false;

            if (!Trigger(entity, target, data))
                return false;

            AfterTriggered(entity, data);
            return true;
        }

        public bool CanTrigger(Entity entity, Cell target)
        {
            var data = GetData(entity);

            return CanTrigger(entity, target, data);
        }

        protected abstract bool CanTrigger(Entity entity, Cell target, FeatureData data);

        protected abstract bool Trigger(Entity entity, Cell target, FeatureData data);

        protected virtual void AfterTriggered(Entity entity, FeatureData data) { }

        public virtual void StartTurn(Entity entity) { }

        public virtual void EndTurn(Entity entity) { }

        protected FeatureData GetData(Entity entity)
        {
            if (!entity.FeatureData.TryGetValue(this, out var featureData))
            {
                featureData = new FeatureData();
                entity.FeatureData.Add(this, featureData);
            }

            return featureData;
        }
    }

    public abstract class PassiveFeature : Feature
    {
        public override FeatureType Type => FeatureType.Passive;

        public override FeatureState DetermineState(Entity entity)
        {
            return FeatureState.None;
        }

        protected override bool Trigger(Entity entity, Cell target, FeatureData data)
        {
            throw new NotImplementedException();
        }

        protected override bool CanTrigger(Entity entity, Cell target, FeatureData data)
        {
            return false;
        }
    }

    public abstract class ActivatedFeature : Feature
    {
        public override FeatureType Type => FeatureType.Triggered;

        public virtual int ManaCost { get; set; } = 0;

        public virtual int? LimitedUses { get; set; }

        public virtual int? CooldownTurns { get; set; }

        public override FeatureState DetermineState(Entity entity)
        {
            return IsReadyToTrigger(entity, GetData(entity))
                ? FeatureState.CanTrigger
                : FeatureState.Disabled;
        }

        protected override bool CanTrigger(Entity entity, Cell target, FeatureData data)
        {
            return IsReadyToTrigger(entity, data);
        }

        private const string limitedUsesDataKey = "uses";

        private const string cooldownTurnsDataKey = "cooldown";

        private bool IsReadyToTrigger(Entity entity, FeatureData data)
        {
            if (entity.Mana < ManaCost)
                return false;

            if (LimitedUses.HasValue && data.TryGetValue(limitedUsesDataKey, out int usesLeft) && usesLeft <= 0)
                return false;

            if (CooldownTurns.HasValue && data.TryGetValue(cooldownTurnsDataKey, out int cooldownLeft) && cooldownLeft > 0)
                return false;

            return true;
        }

        protected override void AfterTriggered(Entity entity, FeatureData data)
        {
            entity.Mana = Math.Max(0, entity.Mana - ManaCost);

            if (LimitedUses.HasValue)
            {
                if (!data.TryGetValue(limitedUsesDataKey, out int usesLeft))
                    usesLeft = LimitedUses.Value;

                data[limitedUsesDataKey] = usesLeft - 1;
            }

            if (CooldownTurns.HasValue)
            {
                data[cooldownTurnsDataKey] = CooldownTurns.Value;
            }
        }
    }

    public abstract class TargettedFeature : ActivatedFeature
    {
        public virtual int? Range { get; set; }

        protected override bool CanTrigger(Entity entity, Cell target, FeatureData data)
        {
            if (!base.CanTrigger(entity, target))
                return false;

            if (Range.HasValue)
            {
                int distance = 0; // TODO: determine this somehow ... ach we need the battlefield, don't we.

                if (distance > Range.Value)
                    return false;
            }

            return true;
        }
    }

    public abstract class EntityTargettedFeature : TargettedFeature
    {
        protected override bool CanTrigger(Entity entity, Cell target, FeatureData data)
        {
            if (!base.CanTrigger(entity, target))
                return false;

            // player must be able to see a target unit (or building)
            if (!target.EntitiesThatCanSee.Any(e => e.Owner == entity.Owner))
                return false;

            if (target.Building != null && IsValidTarget(entity, target.Building))
                return true;

            foreach (var unit in target.Units)
                if (IsValidTarget(entity, unit))
                    return true;

            return false;
        }

        public abstract TargetingOptions AllowedTargets { get; }

        public bool IsValidTarget(Entity user, Entity target)
        {
            if (target is Building && !AllowedTargets.HasFlag(TargetingOptions.Buildings))
                return false;

            else if (target is Unit && !AllowedTargets.HasFlag(TargetingOptions.Units))
                return false;

            if (AllowedTargets.HasFlag(TargetingOptions.RequiresMana) && target.BaseDefinition.Mana == 0)
                return false;

            if (target == user)
                return AllowedTargets.HasFlag(TargetingOptions.Self);

            if (target.Owner == user.Owner)
                return AllowedTargets.HasFlag(TargetingOptions.SameOwner);

            return AllowedTargets.HasFlag(TargetingOptions.Enemies);
        }
    }

    public abstract class ToggleFeature : Feature
    {
        public override FeatureType Type => FeatureType.Toggled;

        public abstract void Enable(Entity entity);

        public abstract void Disable(Entity entity);

        public virtual int ActivateManaCost { get; set; } = 0;

        public virtual int ManaCostPerTurn { get; set; } = 0;

        private const string enabledDataKey = "enabled";

        protected override bool CanTrigger(Entity entity, Cell target, FeatureData data)
        {
            bool isEnabled = data.ContainsKey(enabledDataKey);

            if (isEnabled)
                return true;

            return entity.Mana >= ActivateManaCost;
        }

        protected override bool Trigger(Entity entity, Cell target, FeatureData data)
        {
            bool isEnabled = data.ContainsKey(enabledDataKey);

            if (isEnabled)
            {
                Disable(entity);
                data.Remove(enabledDataKey);
            }
            else if (entity.Mana >= ActivateManaCost)
            {
                entity.Mana -= ActivateManaCost;
                data[enabledDataKey] = 1;
                Enable(entity);
            }

            return true;
        }

        public override void StartTurn(Entity entity)
        {
            var data = GetData(entity);
            bool isEnabled = data.ContainsKey(enabledDataKey);

            if (isEnabled)
            {
                if (entity.Mana < ManaCostPerTurn)
                {
                    Disable(entity);
                    data.Remove(enabledDataKey);
                }
                else
                    entity.Mana -= ManaCostPerTurn;
            }
        }

        public override FeatureState DetermineState(Entity entity)
        {
            var data = GetData(entity);
            bool isEnabled = data.ContainsKey(enabledDataKey);

            return isEnabled
                ? FeatureState.ToggledOn
                : entity.Mana < ActivateManaCost
                    ? FeatureState.Disabled
                    : FeatureState.CanTrigger;
        }
    }

    public abstract class EffectToggleFeature<TEffect> : ToggleFeature
        where TEffect : IStatusEffect
    {
        public override void Enable(Entity entity)
        {
            entity.AddEffect(Activator.CreateInstance<TEffect>());
        }

        public override void Disable(Entity entity)
        {
            entity.RemoveEffect<TEffect>();
        }
    }

    public abstract class StatusEffectFeature<Effect> : ActivatedFeature
        where Effect : IStatusEffect
    {
        protected override bool Trigger(Entity entity, Cell target, FeatureData data)
        {
            entity.AddEffect(Activator.CreateInstance<Effect>());
            return true;
        }
    }

    public abstract class TargettedStatusEffectFeature<Effect> : EntityTargettedFeature
        where Effect : IStatusEffect
    {
        protected override bool Trigger(Entity entity, Cell target, FeatureData data)
        {
            if (target.Building != null && IsValidTarget(entity, target.Building))
            {
                ApplyEffect(target.Building);
            }
            else
            {
                var targetUnit = target.Units.FirstOrDefault(u => IsValidTarget(entity, u));
                if (targetUnit == null)
                    return false;

                ApplyEffect(targetUnit);
            }

            return true;
        }

        private void ApplyEffect(Entity target)
        {
            target.AddEffect(Activator.CreateInstance<Effect>());
        }
    }

    public abstract class SelfCellEffectFeature<Effect> : ActivatedFeature
        where Effect : ICellEffect
    {
        protected override bool Trigger(Entity entity, Cell target, FeatureData data)
        {
            // TODO: this effect would need to apply to all effects within range of the unit, every turn
            // target.AddEffect(Activator.CreateInstance<Effect>());
            return false;
        }
    }

    public abstract class TargettedCellEffectFeature<Effect> : TargettedFeature
        where Effect : ICellEffect
    {
        protected override bool Trigger(Entity entity, Cell target, FeatureData data)
        {
            // TODO: need the concept of cell effects
            // target.AddEffect(Activator.CreateInstance<Effect>())
            return false;
        }
    }
}
