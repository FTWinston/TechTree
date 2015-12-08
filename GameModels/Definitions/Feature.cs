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
        public abstract string Description { get; }
        public abstract char Appearance { get; }

        public EntityType EntityDefinition { get; internal set; }
        public virtual Research UnlockedBy { get { return null; } }

        public abstract bool UsesMana { get; }
        public abstract InteractionMode Mode { get; }

        public enum InteractionMode
        {
            Passive,
            Toggled,
            Triggered,
        }

        public abstract double Initialize(EntityType type, Random r);
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

    public abstract class ActivatedFeature : Feature
    {
        public override Feature.InteractionMode Mode { get { return InteractionMode.Triggered; } }
        public override bool UsesMana { get { return ManaCost > 0; } }

        public abstract void Activate(Entity entity);

        public int CooldownTurns { get; internal set; }
        public int ManaCost { get; internal set; }

        public override bool Clicked(Entity entity)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class BuildFeature : Feature
    {
        public override Feature.InteractionMode Mode { get { return InteractionMode.Triggered; } }
        public override bool UsesMana { get { return false; } }

        public UnitType TypeToBuild { get; internal set; }
        public int TurnsToComplete { get; internal set; }

        public override bool Clicked(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
