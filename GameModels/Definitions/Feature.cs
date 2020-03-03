using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

using EntityData = System.Collections.Generic.Dictionary<string, int>;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public abstract class Feature : ISelectable
    {
        protected Feature(string name, string symbol)
        {
            Name = name;
            Symbol = symbol;
        }

        public FeatureDTO ToDTO()
        {
            return new FeatureDTO(Name, Symbol, Identifier, SerializeData());
        }

        protected virtual Dictionary<string, int> SerializeData()
        {
            return new Dictionary<string, int>();
        }

        public string Name { get; }

        public string Symbol { get; }

        protected abstract string Identifier { get; }

        public abstract string Description { get; }

        public abstract FeatureMode Mode { get; }

        public virtual Research? UnlockedBy { get; internal set; }

        public abstract FeatureState DetermineState(Entity entity);

        public virtual void Initialize(EntityType entityType) { }

        public virtual void Unlock(Entity entity) { }

        public bool TryTrigger(Entity entity, Cell target)
        {
            var data = GetEntityData(entity);

            if (!CanTrigger(entity, target, data))
                return false;

            if (!Trigger(entity, target, data))
                return false;

            AfterTriggered(entity, data);
            return true;
        }

        public bool CanTrigger(Entity entity, Cell target)
        {
            var data = GetEntityData(entity);

            return CanTrigger(entity, target, data);
        }

        protected abstract bool CanTrigger(Entity entity, Cell target, EntityData data);

        protected abstract bool Trigger(Entity entity, Cell target, EntityData data);

        protected virtual void AfterTriggered(Entity entity, EntityData data) { }

        public virtual void StartTurn(Entity entity) { }

        public virtual void EndTurn(Entity entity) { }

        protected EntityData GetEntityData(Entity entity)
        {
            if (!entity.FeatureData.TryGetValue(this, out var featureData))
            {
                featureData = new EntityData();
                entity.FeatureData.Add(this, featureData);
            }

            return featureData;
        }
    }

    public abstract class PassiveFeature : Feature
    {
        protected PassiveFeature(string name, string symbol)
            : base(name, symbol) { }

        public override FeatureMode Mode => FeatureMode.Passive;

        public override FeatureState DetermineState(Entity entity)
        {
            return FeatureState.None;
        }

        protected override bool Trigger(Entity entity, Cell target, EntityData data)
        {
            throw new NotImplementedException();
        }

        protected override bool CanTrigger(Entity entity, Cell target, EntityData data)
        {
            return false;
        }
    }

    public abstract class ActivatedFeature : Feature
    {
        protected ActivatedFeature(string name, string symbol, int manaCost, int? limitedUses, int? cooldownTurns)
            : base(name, symbol)
        {
            ManaCost = manaCost;
            LimitedUses = limitedUses;
            CooldownTurns = cooldownTurns;
        }

        protected ActivatedFeature(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol)
        {
            ManaCost = data.TryGetValue("manaCost", out int manaCost)
                ? manaCost
                : 0;

            if (data.TryGetValue("limitedUses", out int limitedUses))
                LimitedUses = limitedUses;

            if (data.TryGetValue("cooldown", out int cooldown))
                CooldownTurns = cooldown;
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();

            if (ManaCost != 0)
                data.Add("manaCost", ManaCost);

            if (LimitedUses.HasValue)
                data.Add("limitedUses", LimitedUses.Value);

            if (CooldownTurns.HasValue)
                data.Add("cooldown", CooldownTurns.Value);

            return data;
        }

        public override FeatureMode Mode => FeatureMode.Triggered;

        public virtual int ManaCost { get; set; }

        public virtual int? LimitedUses { get; set; }

        public virtual int? CooldownTurns { get; set; }

        public override FeatureState DetermineState(Entity entity)
        {
            return IsReadyToTrigger(entity, GetEntityData(entity))
                ? FeatureState.CanTrigger
                : FeatureState.Disabled;
        }

        protected override bool CanTrigger(Entity entity, Cell target, EntityData data)
        {
            return IsReadyToTrigger(entity, data);
        }

        private const string limitedUsesDataKey = "uses";

        private const string cooldownTurnsDataKey = "cooldown";

        private bool IsReadyToTrigger(Entity entity, EntityData data)
        {
            if (entity.Mana < ManaCost)
                return false;

            if (LimitedUses.HasValue && data.TryGetValue(limitedUsesDataKey, out int usesLeft) && usesLeft <= 0)
                return false;

            if (CooldownTurns.HasValue && data.TryGetValue(cooldownTurnsDataKey, out int cooldownLeft) && cooldownLeft > 0)
                return false;

            return true;
        }

        protected override void AfterTriggered(Entity entity, EntityData data)
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
        protected TargettedFeature(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int? range)
            : base(name, symbol, manaCost, limitedUses, cooldown)
        {
            Range = range;
        }

        protected TargettedFeature(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            if (data.TryGetValue("range", out int range))
                Range = range;
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();

            if (Range.HasValue)
                data.Add("range", Range.Value);

            return data;
        }

        public virtual int? Range { get; set; }

        protected override bool CanTrigger(Entity entity, Cell target, EntityData data)
        {
            if (!base.CanTrigger(entity, target, data))
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
        protected EntityTargettedFeature(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int? range)
            : base(name, symbol, manaCost, limitedUses, cooldown, range) { }
        
        protected EntityTargettedFeature(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data) { }

        protected override bool CanTrigger(Entity entity, Cell target, EntityData data)
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
        protected ToggleFeature(string name, string symbol, int activateManaCost, int manaCostPerTurn)
            : base(name, symbol)
        {
            ActivateManaCost = activateManaCost;
            ManaCostPerTurn = manaCostPerTurn;
        }

        protected ToggleFeature(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol)
        {
            ActivateManaCost = data.TryGetValue("activateManaCost", out int activateManaCost)
                ? activateManaCost
                : 0;

            ManaCostPerTurn = data.TryGetValue("manaCostPerTurn", out int manaCostPerTurn)
                ? manaCostPerTurn
                : 0;
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();

            if (ActivateManaCost != 0)
                data.Add("activateManaCost", ActivateManaCost);

            if (ManaCostPerTurn != 0)
                data.Add("manaCostPerTurn", ManaCostPerTurn);

            return data;
        }

        public override FeatureMode Mode => FeatureMode.Toggled;

        public abstract void Enable(Entity entity);

        public abstract void Disable(Entity entity);

        public virtual int ActivateManaCost { get; set; }

        public virtual int ManaCostPerTurn { get; set; }

        private const string enabledDataKey = "enabled";

        protected override bool CanTrigger(Entity entity, Cell target, EntityData data)
        {
            bool isEnabled = data.ContainsKey(enabledDataKey);

            if (isEnabled)
                return true;

            return entity.Mana >= ActivateManaCost;
        }

        protected override bool Trigger(Entity entity, Cell target, EntityData data)
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
            var data = GetEntityData(entity);
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
            var data = GetEntityData(entity);
            bool isEnabled = data.ContainsKey(enabledDataKey);

            return isEnabled
                ? FeatureState.ToggledOn
                : entity.Mana < ActivateManaCost
                    ? FeatureState.Disabled
                    : FeatureState.CanTrigger;
        }
    }

    public abstract class EffectToggleFeature<TEffect> : ToggleFeature
        where TEffect : IStatusEffect, new ()
    {
        protected EffectToggleFeature(string name, string symbol, int activateManaCost, int manaCostPerTurn)
            : base(name, symbol, activateManaCost, manaCostPerTurn) { }

        protected EffectToggleFeature(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            // if (data.TryGetValue("effect", out int effect))
                // Effect = new TEffect(Oops, effect);
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();

            // data.Add("effect", Effect.ID);

            return data;
        }

        protected TEffect Effect { get; } = new TEffect();

        public override void Enable(Entity entity)
        {
            entity.AddEffect(Effect);
        }

        public override void Disable(Entity entity)
        {
            entity.RemoveEffect(Effect);
        }
    }

    public abstract class StatusEffectFeature<TEffect> : ActivatedFeature
        where TEffect : IStatusEffect, new()
    {
        protected StatusEffectFeature(string name, string symbol, int manaCost, int? limitedUses, int? cooldownTurns)
            : base(name, symbol, manaCost, limitedUses, cooldownTurns) { }

        protected StatusEffectFeature(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            // if (data.TryGetValue("effect", out int effect))
            // Effect = new TEffect(Oops, effect);
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();

            // data.Add("effect", Effect.ID);

            return data;
        }

        protected override bool Trigger(Entity entity, Cell target, EntityData data)
        {
            entity.AddEffect(Effect);
            return true;
        }

        protected TEffect Effect { get; } = new TEffect();
    }

    public abstract class TargettedStatusEffectFeature<TEffect> : EntityTargettedFeature
        where TEffect : IStatusEffect, new()
    {
        protected TargettedStatusEffectFeature(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int? range)
            : base(name, symbol, manaCost, limitedUses, cooldown, range) { }

        protected TargettedStatusEffectFeature(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            // if (data.TryGetValue("effect", out int effect))
            // Effect = new TEffect(Oops, effect);
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();

            // data.Add("effect", Effect.ID);

            return data;
        }

        protected override bool Trigger(Entity entity, Cell target, EntityData data)
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

        protected TEffect Effect { get; } = new TEffect();

        private void ApplyEffect(Entity target)
        {
            target.AddEffect(Effect);
        }
    }

    public abstract class SelfCellEffectFeature<TEffect> : ActivatedFeature
        where TEffect : ICellEffect, new()
    {
        protected SelfCellEffectFeature(string name, string symbol, int manaCost, int? limitedUses, int? cooldownTurns)
            : base(name, symbol, manaCost, limitedUses, cooldownTurns) { }

        protected SelfCellEffectFeature(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            // if (data.TryGetValue("effect", out int effect))
            // Effect = new TEffect(Oops, effect);
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();

            // data.Add("effect", Effect.ID);

            return data;
        }

        protected override bool Trigger(Entity entity, Cell target, EntityData data)
        {
            // TODO: this effect would need to apply to all effects within range of the unit, every turn
            // target.AddEffect(Activator.CreateInstance<Effect>());
            return false;
        }

        protected TEffect Effect { get; } = new TEffect();
    }

    public abstract class TargettedCellEffectFeature<TEffect> : TargettedFeature
        where TEffect : ICellEffect, new()
    {
        protected TargettedCellEffectFeature(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int? range)
            : base(name, symbol, manaCost, limitedUses, cooldown, range) { }

        protected TargettedCellEffectFeature(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            // if (data.TryGetValue("effect", out int effect))
            // Effect = new TEffect(Oops, effect);
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();

            // data.Add("effect", Effect.ID);

            return data;
        }

        protected override bool Trigger(Entity entity, Cell target, EntityData data)
        {
            // TODO: need the concept of cell effects
            // target.AddEffect(Activator.CreateInstance<Effect>())
            return false;
        }

        protected TEffect Effect { get; } = new TEffect();
    }
}
