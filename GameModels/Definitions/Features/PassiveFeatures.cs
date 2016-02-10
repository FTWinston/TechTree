using GameModels.Definitions;
using GameModels.Definitions.Features;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameModels.Definitions.Features
{
    public class HigherHealth : PassiveFeature
    {
        public HigherHealth(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name { get { return "More Health"; } }
        protected override string GetDescription() { return string.Format("Adds {0} hitpoints", ExtraPoints); }
        public override string Symbol { get { return "☥"; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type)
        {
            float old = type.Health;
            type.Health += ExtraPoints;
            return 1.0 * (type.Health / old);
        }
    }
    
    public class Armored : PassiveFeature
    {
        public Armored(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name { get { return "Armored"; } }
        protected override string GetDescription() { return string.Format("Adds {0} armor points", ExtraPoints); }
        public override string Symbol { get { return "♈"; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type)
        {
            type.Armor += ExtraPoints;
            return 1.0 + ExtraPoints * 0.05;
        }
    }

    public class GreaterMobility : PassiveFeature
    {
        public GreaterMobility(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name { get { return "Greater Mobility"; } }
        protected override string GetDescription() { return string.Format("Adds {0} action points", ExtraPoints); }
        public override string Symbol { get { return "♒"; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type)
        {
            type.ActionPoints += ExtraPoints;
            return 1.0 + ExtraPoints * 0.15;
        }
        
        public override bool Validate(EntityType type)
        {// don't allow on a type that is a building
            return !(type is BuildingType);
        }
    }

    public class GreaterVisibility : PassiveFeature
    {
        public GreaterVisibility(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name { get { return "Greater Visibility"; } }
        protected override string GetDescription() { return string.Format("Increases vision range by {0} tiles", ExtraPoints); }
        public override string Symbol { get { return "⚙"; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type)
        {
            type.VisionRange += ExtraPoints;
            return 1.0 + ExtraPoints * 0.1;
        }
    }

	public class HigherMana : PassiveFeature
    {
        public HigherMana(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name { get { return "Potency"; } }
        protected override string GetDescription() { return string.Format("Adds {0} mana points", ExtraPoints); }
        public override string Symbol { get { return "⚛"; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type)
        {
            float old = type.Health;
            type.Mana += ExtraPoints;
            return 1.0 * (type.Health / old);
        }

        public override bool Validate(EntityType type)
        {// don't allow on a type that doesn't havea any mana-using features
            return type.Mana > 0 && type.Features.FirstOrDefault(f => f.UsesMana) != null;
        }
    }

    public class Detector : PassiveFeature
    {
        public override string Name { get { return "Awareness"; } }
        protected override string GetDescription() { return "Allows detection of invisible units"; }
        public override string Symbol { get { return "☉"; } }

        public override double Initialize(EntityType type)
        {
            type.IsDetector = true;
            return 1.4;
        }

        public override bool Validate(EntityType type)
        {// only allow this feature exactly once
            return type.VisionRange > 0 && type.Features.Count(f => f is Detector) == 1;
        }
    }

    public class Supply : PassiveFeature
    {
        public Supply(int points)
        {
            Points = points;
        }

        public override string Name { get { return "Supply"; } }
        protected override string GetDescription() { return string.Format("Provides {0} supply points, allowing units to be built", Points); }
        public override string Symbol { get { return "⛽"; } }
        private int Points { get; set; }

        public override double Initialize(EntityType type)
        {
            int before = type.SupplyCost;

            if (type.SupplyCost >= 0)
                type.SupplyCost = -Points;
            else
                type.SupplyCost -= Points;

            int diff = before - type.SupplyCost;
            return 1.0 + diff * 0.1;
        }
    }

    public class LongerRange : PassiveFeature
    {
        public LongerRange(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name { get { return "Range"; } }
        protected override string GetDescription() { return string.Format("Increases attack range by {0} tiles", ExtraPoints); }
        public override string Symbol { get { return "♐"; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type)
        {
            Attack feature = type.Features.SingleOrDefault(f => f is Attack) as Attack;
            if (feature == null)
                return 1;

            feature.Range += ExtraPoints;

            return 1.0 + ExtraPoints * 0.1;
        }

        public override bool Validate(EntityType type)
        {// only allow this feature exactly once
            return type.Features.SingleOrDefault(f => f is Attack) != null;
        }
    }
}
