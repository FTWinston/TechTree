using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;

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

        /*
        public virtual double Initialize(EntityType type) { return 1; }
        public virtual bool Validate(EntityType type) { return true; }
        public abstract bool Clicked(Entity entity);
        */

        public abstract void Trigger(Entity entity, Cell target);

        public abstract bool CanTrigger(Entity entity, Cell target);

        protected virtual void AfterTriggered(Entity entity) { }

        public virtual void StartTurn(Entity entity) { }

        public virtual void EndTurn(Entity entity) { }

        protected Dictionary<string, int> GetData(Entity entity)
        {
            if (!entity.FeatureData.TryGetValue(this, out var featureData))
            {
                featureData = new Dictionary<string, int>();
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

        public override void Trigger(Entity entity, Cell target)
        {
            throw new NotImplementedException();
        }

        public override bool CanTrigger(Entity entity, Cell target)
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

        private const string limitedUsesDataKey = "uses";

        private const string cooldownTurnsDataKey = "cooldown";

        public override bool CanTrigger(Entity entity, Cell target)
        {
            if (entity.Mana < ManaCost)
                return false;

            var data = GetData(entity);

            if (LimitedUses.HasValue && data.TryGetValue(limitedUsesDataKey, out int usesLeft) && usesLeft <= 0)
                return false;

            if (CooldownTurns.HasValue && data.TryGetValue(cooldownTurnsDataKey, out int cooldownLeft) && cooldownLeft > 0)
                return false;

            return true;
        }

        protected override void AfterTriggered(Entity entity)
        {
            entity.Mana = Math.Max(0, entity.Mana - ManaCost);

            var data = GetData(entity);

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
        public abstract void Activate(Entity user, Cell target);

        public virtual int? Range { get; set; }

        public override bool CanTrigger(Entity entity, Cell target)
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
        public override bool CanTrigger(Entity entity, Cell target)
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

        public virtual bool CanTargetSelf => false;

        public virtual bool CanTargetBuildings { get; }

        public virtual bool CanTargetUnits { get; }

        public abstract bool CanTargetEnemies { get; }

        public abstract bool CanTargetFriendlies { get; }

        public virtual bool IsValidTarget(Entity user, Entity target)
        {
            if (target is Building && !CanTargetBuildings)
                return false;

            else if (target is Unit && !CanTargetUnits)
                return false;

            if (target == user)
                return CanTargetSelf;

            if (target.Owner == user.Owner)
                return CanTargetFriendlies;

            return CanTargetEnemies;
        }
    }

    public abstract class ToggleFeature : Feature
    {
        public override FeatureType Type => FeatureType.Toggled;

        public abstract void Enable(Entity entity);
        public abstract void Disable(Entity entity);

        public virtual int ActivateManaCost => 0;
        public virtual int ManaCostPerTurn => 0;

        private const string enabledDataKey = "enabled";

        public override void Trigger(Entity entity, Cell target)
        {
            var data = GetData(entity);
            bool isEnabled = data.ContainsKey(enabledDataKey);

            if (isEnabled)
            {
                Disable(entity);
                data.Remove(enabledDataKey);
                return;
            }
            else if (entity.Mana >= ActivateManaCost)
            {
                entity.Mana -= ActivateManaCost;
                data[enabledDataKey] = 1;
                Enable(entity);
            }
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
        public override void Trigger(Entity user, Cell target)
        {
            user.AddEffect(Activator.CreateInstance<Effect>());
        }
    }

    public abstract class TargettedStatusEffectFeature<Effect> : EntityTargettedFeature
        where Effect : IStatusEffect
    {
        public override void Trigger(Entity entity, Cell target)
        {
            if (target.Building != null && IsValidTarget(entity, target.Building))
            {
                ApplyEffect(target.Building);
            }
            else
            {
                var targetUnit = target.Units.First(u => IsValidTarget(entity, u));
                ApplyEffect(targetUnit);
            }
        }

        private void ApplyEffect(Entity target)
        {
            target.AddEffect(Activator.CreateInstance<Effect>());
        }
    }

    public abstract class SelfCellEffectFeature<Effect> : ActivatedFeature
        where Effect : ICellEffect
    {
        public override void Trigger(Entity entity, Cell target)
        {
            // TODO: this effect would need to apply to all effects within range of the unit, every turn
            // target.AddEffect(Activator.CreateInstance<Effect>());
        }
    }

    public abstract class TargettedCellEffectFeature<Effect> : TargettedFeature
        where Effect : ICellEffect
    {
        public override void Trigger(Entity entity, Cell target)
        {
            // TODO: need the concept of cell effects
            // target.AddEffect(Activator.CreateInstance<Effect>())
        }
    }
}
