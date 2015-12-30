using GameModels.Definitions;
using GameModels.Definitions.Features;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public partial class Feature
    {
        internal static IList<Func<Feature>> GetPassiveFeatures()
        {
            return new Func<Feature>[] {
                () => new HigherHealth(),
                () => new Armored(),
                () => new GreaterMobility(),
                () => new GreaterVisibility(),
                () => new HigherMana(),
                () => new Detector(),
                () => new Supply(),
                () => new LongerRange(),
            };
        }
    }
}

namespace GameModels.Definitions.Features
{
    public class HigherHealth : PassiveFeature
    {
        public override string Name { get { return "More Health"; } }
        public override string GetDescription() { return string.Format("Adds {0} hitpoints", ExtraPoints); }
        public override char Appearance { get { return '+'; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type, Random r)
        {
            double scale = r.NextDouble() * 0.75 + 0.25;
            ExtraPoints = (int)(30 * scale);
            type.Health += ExtraPoints;
            return 1.0 + .2 * scale;
        }
    }
    
    public class Armored : PassiveFeature
    {
        public override string Name { get { return "Armored"; } }
        public override string GetDescription() { return string.Format("Adds {0} armor points", ExtraPoints); }
        public override char Appearance { get { return '#'; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type, Random r)
        {
            ExtraPoints = r.Next(1, 3);
            type.Armor += ExtraPoints;
            return 1.0 + ExtraPoints * 0.05;
        }
    }

    public class GreaterMobility : PassiveFeature
    {
        public override string Name { get { return "Greater Mobility"; } }
        public override string GetDescription() { return string.Format("Adds {0} action points", ExtraPoints); }
        public override char Appearance { get { return '>'; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type, Random r)
        {
            ExtraPoints = r.Next(1, 3);
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
        public override string Name { get { return "Greater Visibility"; } }
        public override string GetDescription() { return string.Format("Increases vision range by {0} tiles", ExtraPoints); }
        public override char Appearance { get { return 'O'; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type, Random r)
        {
            ExtraPoints = r.Next(1, 3);
            type.VisionRange += ExtraPoints;
            return 1.0 + ExtraPoints * 0.1;
        }
    }

	public class HigherMana : PassiveFeature
    {
        public override string Name { get { return "Potency"; } }
        public override string GetDescription() { return string.Format("Adds {0} mana points", ExtraPoints); }
        public override char Appearance { get { return 'M'; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type, Random r)
        {
            double scale = r.NextDouble() * 0.75 + 0.25;
            ExtraPoints = (int)(30 * scale);
            type.Mana += ExtraPoints;
            return 1.0 + .05 * scale;
        }

        public override bool Validate(EntityType type)
        {// don't allow on a type that doesn't havea any mana-using features
            return type.Mana > 0 && type.Features.Find(f => f.UsesMana) != null;
        }
    }

    public class Detector : PassiveFeature
    {
        public override string Name { get { return "Awareness"; } }
        public override string GetDescription() { return "Allows detection of invisible units"; }
        public override char Appearance { get { return 'I'; } }

        public override double Initialize(EntityType type, Random r)
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
        public override string Name { get { return "Supply"; } }
        public override string GetDescription() { return string.Format("Provides {0} supply points, allowing units to be built", Points); }
        public override char Appearance { get { return 'S'; } }
        private int Points { get; set; }

        public override double Initialize(EntityType type, Random r)
        {
            Points = r.Next(3, 6);
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
        public override string Name { get { return "Range"; } }
        public override string GetDescription() { return string.Format("Increases attack range by {0} tiles", ExtraPoints); }
        public override char Appearance { get { return 'S'; } }
        private int ExtraPoints { get; set; }

        public override double Initialize(EntityType type, Random r)
        {
            ExtraPoints = r.Next(1, 3);

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
