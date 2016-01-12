using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public abstract partial class Feature
    {
        public abstract string Name { get; }
        public abstract string GetDescription();
        public abstract string Symbol { get; }

        public EntityType EntityDefinition { get; internal set; }
        public virtual Research UnlockedBy { get; internal set; }

        public abstract bool UsesMana { get; }
        public abstract InteractionMode Mode { get; }

        public enum InteractionMode
        {
            Passive,
            Toggled,
            Triggered,
            Purchased,
        }

        public virtual double Initialize(EntityType type) { return 1; }
        public virtual bool Validate(EntityType type) { return true; }
        public abstract bool Clicked(Entity entity);
    }

    public abstract class PassiveFeature : Feature
    {
        public override Feature.InteractionMode Mode { get { return InteractionMode.Passive; } }
        public override bool UsesMana { get { return false; } }

        public override bool Clicked(Entity entity)
        {
            return false;
        }
    }

    public abstract class ActivatedFeature : Feature
    {
        public override Feature.InteractionMode Mode { get { return InteractionMode.Triggered; } }
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
            if (target.Entity == null)
                return false;

            return IsValidTarget(user, target.Entity);
        }

        public abstract bool IsValidTarget(Entity user, Entity target);
    }

    public abstract class ToggleFeature : Feature
    {
        public override Feature.InteractionMode Mode { get { return InteractionMode.Toggled; } }
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
