using GameModels.Definitions;
using GameModels.Features;
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
                () => new LowerHealth(),
                () => new Armored(),
                () => new GreaterMobility(),
                () => new ReducedMobility(),
                () => new GreaterVisibility(),
                () => new ReducedVisibility(),
                () => new HigherMana(),
                () => new LowerMana(),
                () => new Detector(),
                () => new Supply()
            };
        }
    }
}

namespace GameModels.Features
{
    public class HigherHealth : PassiveFeature
    {
        public override string Name { get { return "More Health"; } }
        public override string Description { get { return "Increases hitpoints"; } }
        public override char Appearance { get { return '+'; } }

        public override double Initialize(EntityType type, Random r)
        {
            double scale = r.NextDouble() * 0.75 + 0.25;
            type.Health += (int)(30 * scale);
            return 1.0 + .2 * scale;
        }
    }

    public class LowerHealth : PassiveFeature
    {
        public override string Name { get { return "Lower Health"; } }
        public override string Description { get { return "Decreases hitpoints"; } }
        public override char Appearance { get { return '-'; } }

        public override double Initialize(EntityType type, Random r)
        {
            double scale = r.NextDouble() * 0.75 + 0.25;
            type.Health -= (int)(30 * scale);
            return 1.0 - .2 * scale;
        }

        public override bool Validate(EntityType type)
        {// don't allow on a type that also has "higher health"
            return type.Health > 0 && type.Features.Find(f => f.GetType() == typeof(HigherHealth)) == null;
        }
    }

    public class Armored : PassiveFeature
    {
        public override string Name { get { return "Armored"; } }
        public override string Description { get { return "Increases armor"; } }
        public override char Appearance { get { return '#'; } }

        public override double Initialize(EntityType type, Random r)
        {
            int armor = r.Next(1, 3);
            type.Armor += armor;
            return 1.0 + armor * 0.05;
        }
    }

    public class GreaterMobility : PassiveFeature
    {
        public override string Name { get { return "Greater Mobility"; } }
        public override string Description { get { return "Increases action points"; } }
        public override char Appearance { get { return '>'; } }

        public override double Initialize(EntityType type, Random r)
        {
            int points = r.Next(1, 3);
            type.ActionPoints += points;
            return 1.0 + points * 0.15;
        }
        
        public override bool Validate(EntityType type)
        {// don't allow on a type that is a building
            return !(type is BuildingType);
        }
    }

    public class ReducedMobility : PassiveFeature
    {
        public override string Name { get { return "Reduced Mobility"; } }
        public override string Description { get { return "Decreases action points"; } }
        public override char Appearance { get { return '<'; } }

        public override double Initialize(EntityType type, Random r)
        {
            int points = r.Next(1, 3);
            type.ActionPoints -= points;
            return 1.0 - points * 0.15;
        }

        public override bool Validate(EntityType type)
        {// don't allow on a type that is a building, or also has "greater mobility"
            return type.ActionPoints > 0 && !(type is BuildingType) && type.Features.Find(f => f.GetType() == typeof(GreaterMobility)) == null;
        }
    }

    public class GreaterVisibility : PassiveFeature
    {
        public override string Name { get { return "Greater Visibility"; } }
        public override string Description { get { return "Increases vision range"; } }
        public override char Appearance { get { return 'O'; } }

        public override double Initialize(EntityType type, Random r)
        {
            int points = r.Next(1, 3);
            type.VisionRange += points;
            return 1.0 + points * 0.1;
        }
    }

    public class ReducedVisibility : PassiveFeature
    {
        public override string Name { get { return "Reduced Visibility"; } }
        public override string Description { get { return "Decreases vision range"; } }
        public override char Appearance { get { return 'o'; } }

        public override double Initialize(EntityType type, Random r)
        {
            int points = r.Next(1, 3);
            type.VisionRange -= points;
            return 1.0 - points * 0.15;
        }

        public override bool Validate(EntityType type)
        {// don't reduce visibility below zero, don't allow combining with "greater visibility"
            return type.VisionRange >= 0 && type.Features.Find(f => f.GetType() == typeof(GreaterVisibility)) == null; ;
        }
    }

	public class HigherMana : PassiveFeature
    {
        public override string Name { get { return "Potency"; } }
        public override string Description { get { return "Increases mana"; } }
        public override char Appearance { get { return 'M'; } }

        public override double Initialize(EntityType type, Random r)
        {
            double scale = r.NextDouble() * 0.75 + 0.25;
            type.Mana += (int)(30 * scale);
            return 1.0 + .05 * scale;
        }

        public override bool Validate(EntityType type)
        {// don't allow on a type that doesn't havea any mana-using features
            return type.Mana > 0 && type.Features.Find(f => f.UsesMana) != null;
        }
    }

    public class LowerMana : PassiveFeature
    {
        public override string Name { get { return "Reduced Potency"; } }
        public override string Description { get { return "Decreases mana"; } }
        public override char Appearance { get { return 'W'; } }

        public override double Initialize(EntityType type, Random r)
        {
            double scale = r.NextDouble() * 0.75 + 0.25;
            type.Mana -= (int)(30 * scale);
            return 1.0 - .05 * scale;
        }

        public override bool Validate(EntityType type)
        {// don't allow on a type that doesn't have any mana-using features
            return type.Mana > 0 && type.Features.Find(f => f.UsesMana) != null && type.Features.Find(f => f.GetType() == typeof(HigherMana)) == null;
        }
    }

    public class Detector : PassiveFeature
    {
        public override string Name { get { return "Awareness"; } }
        public override string Description { get { return "Allows detection of invisible units"; } }
        public override char Appearance { get { return 'I'; } }

        public override double Initialize(EntityType type, Random r)
        {
            type.IsDetector = true;
            return 1.4;
        }

        public override bool Validate(EntityType type)
        {// only allow this feature exactly once
            return type.VisionRange > 0 && type.Features.Count(f => f.GetType() == typeof(Detector)) == 1;
        }
    }

    public class Supply : PassiveFeature
    {
        public override string Name { get { return "Supply"; } }
        public override string Description { get { return "Provides supply, allowing units to be built"; } }
        public override char Appearance { get { return 'S'; } }

        public override double Initialize(EntityType type, Random r)
        {
            int points = r.Next(3, 6);
            int before = type.SupplyCost;

            if (type.SupplyCost >= 0)
                type.SupplyCost = -points;
            else
                type.SupplyCost -= points;

            int diff = before - type.SupplyCost;
            return 1.0 + diff * 0.1;
        }
    }
}
