using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public abstract class Feature : ISelectable
    {
        public abstract string Name { get; }
        
        public abstract string Description { get; }
        
        public abstract string Symbol { get; }

        public abstract FeatureType Type { get; }

        public virtual Research? UnlockedBy { get; internal set; }

        public virtual FeatureState DetermineState(Entity entity)
        {
            return FeatureState.None;
        }

        /*
        public virtual double Initialize(EntityType type) { return 1; }
        public virtual bool Validate(EntityType type) { return true; }
        public abstract bool Clicked(Entity entity);
        */

        public virtual void Trigger(Entity entity) { }

        public virtual void StartTurn(Entity entity) { }

        public virtual void EndTurn(Entity entity) { }

        protected Dictionary<string, int> GetData(Entity entity)
        {
            if (!entity.FeatureData.TryGetValue(this, out var featureData))
                featureData = new Dictionary<string, int>();

            return featureData;
        }
    }

    public abstract class PassiveFeature : Feature
    {
        public override FeatureType Type => FeatureType.Passive;

        public override FeatureState DetermineState(Entity entity)
        {
            return FeatureState.Enabled;
        }
    }

    public abstract class ActivatedFeature : Feature
    {
        public override FeatureType Type { get { return FeatureType.Triggered; } }
        public override bool UsesMana { get { return ManaCost > 0; } }
        public virtual int LimitedUses { get { return 0; } }

        public int CooldownTurns { get; internal set; }
        public int ManaCost { get; internal set; }

        public abstract void Activate(Entity user);

        public override bool Clicked(Entity entity)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class TargettedFeature : ActivatedFeature
    {
        public abstract void Activate(Entity user, Cell target);

        public int Range { get; internal set; }

        public virtual bool IsValidTarget(Entity user, Cell target) { return true; }

        public override void Activate(Entity user)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class EntityTargettedFeature : TargettedFeature
    {
        public override bool IsValidTarget(Entity user, Cell target)
        {
            if (target.Building != null && IsValidTarget(user, target.Building))
                return true;

            foreach (var unit in target.Units)
                if (IsValidTarget(user, unit))
                    return true;

            return false;
        }

        public abstract bool IsValidTarget(Entity user, Entity target);
    }

    public abstract class ToggleFeature : Feature
    {
        public override FeatureType Type { get { return FeatureType.Toggled; } }
        public override bool UsesMana { get { return ActivateManaCost > 0 || ManaCostPerTurn > 0; } }

        public abstract void Enable(Entity entity);
        public abstract void Disable(Entity entity);

        public int ActivateManaCost { get; internal set; }
        public int ManaCostPerTurn { get; internal set; }

        public override bool Clicked(Entity entity)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class ToggleFeature<Effect> : ToggleFeature
        where Effect : IStatusEffect
    {
        protected ToggleFeature()
        {
            EffectInstance = Activator.CreateInstance<Effect>();
        }

        protected Effect EffectInstance { get; private set; }

        public override void Enable(Entity entity)
        {
            entity.AddEffect(EffectInstance);
        }
        public override void Disable(Entity entity)
        {
            entity.RemoveEffect(EffectInstance);
        }
    }

    public abstract class SelfStatusEffectFeature<Effect> : ActivatedFeature
        where Effect : IStatusEffect
    {
        protected SelfStatusEffectFeature()
        {
            EffectInstance = Activator.CreateInstance<Effect>();
        }

        protected Effect EffectInstance { get; private set; }

        public override void Activate(Entity user)
        {
            user.AddEffect(EffectInstance);
            throw new NotImplementedException(); // drain mana etc
        }
    }

    public abstract class TargettedStatusEffectFeature<Effect> : EntityTargettedFeature
        where Effect : IStatusEffect
    {
        protected TargettedStatusEffectFeature()
        {
            EffectInstance = Activator.CreateInstance<Effect>();
        }

        protected Effect EffectInstance { get; private set; }

        public override void Activate(Entity user, Cell target)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class SelfCellEffectFeature<Effect> : ActivatedFeature
        where Effect : ICellEffect
    {
        protected SelfCellEffectFeature()
        {
            EffectInstance = Activator.CreateInstance<Effect>();
        }

        protected Effect EffectInstance { get; private set; }

        public override void Activate(Entity user)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class TargettedCellEffectFeature<Effect> : TargettedFeature
        where Effect : ICellEffect
    {
        protected TargettedCellEffectFeature()
        {
            EffectInstance = Activator.CreateInstance<Effect>();
        }

        protected Effect EffectInstance { get; private set; }

        public override void Activate(Entity user, Cell target)
        {
            throw new NotImplementedException();
        }
    }
}
