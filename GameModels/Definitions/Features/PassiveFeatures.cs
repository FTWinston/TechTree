using ObjectiveStrategy.GameModels.Extensions;
using ObjectiveStrategy.GameModels.Instances;
using System.Linq;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class HigherHealth : PassiveFeature
    {
        public HigherHealth(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name => "More Health";

        public override string Description => $"Adds {ExtraPoints} hitpoints";

        public override string Symbol => "☥";

        private int ExtraPoints { get; set; }

        public override void Initialize(EntityType type)
        {
            type.Health += ExtraPoints;
        }

        public override void Unlock(Entity entity)
        {
            entity.Health += ExtraPoints;
        }
    }
    
    public class Armored : PassiveFeature
    {
        public Armored(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name => "Armored";

        public override string Description => $"Adds {ExtraPoints} armor points";

        public override string Symbol => "♈";

        private int ExtraPoints { get; set; }

        public override void Initialize(EntityType type)
        {
            type.Armor += ExtraPoints;
        }
    }

    public class GreaterMobility : PassiveFeature
    {
        public GreaterMobility(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name => "Greater Mobility";

        public override string Description => $"Adds {ExtraPoints} action points";

        public override string Symbol => "♒";

        private int ExtraPoints { get; set; }

        public override void Initialize(EntityType type)
        {
            if (type is UnitType unitType)
                unitType.MoveRange += ExtraPoints;
        }
        
        /*
        public override bool Validate(EntityType type)
        {
            return !(type is BuildingType); // // don't allow on buildings
        }
        */
    }

    public class GreaterVisibility : PassiveFeature
    {
        public GreaterVisibility(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name => "Greater Visibility";

        public override string Description => $"Increases vision range by {ExtraPoints} tiles";

        public override string Symbol => "⚙";

        private int ExtraPoints { get; set; }

        public override void Initialize(EntityType type)
        {
            type.VisionRange += ExtraPoints;
        }
    }

	public class HigherMana : PassiveFeature
    {
        public HigherMana(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name => "Potency";

        public override string Description => $"Adds {ExtraPoints} mana points";

        public override string Symbol => "⚛";

        private int ExtraPoints { get; set; }

        public override void Initialize(EntityType type)
        {
            type.Mana += ExtraPoints;
        }

        /*
        public override bool Validate(EntityType type)
        {
            return type.Mana > 0; // only allow on types with mana
        }
        */
    }

    public class Detector : PassiveFeature
    {
        public override string Name => "Awareness";

        public override string Description => "Allows detection of invisible units";

        public override string Symbol => "☉";

        public override void Initialize(EntityType type)
        {
            type.IsDetector = true;
        }

        /*
        public override bool Validate(EntityType type)
        {// only allow this feature exactly once
            return type.VisionRange > 0 && type.Features.Count(f => f is Detector) == 1;
        }
        */
    }

    public class Supply : PassiveFeature
    {
        public Supply(int points)
        {
            Points = points;
        }

        public override string Name => "Supply";

        public override string Description => $"Provides {Points} supply points, allowing units to be built";
        
        public override string Symbol => "⛽";

        private int Points { get; set; }

        public override void Initialize(EntityType type)
        {
            if (!type.Cost.TryGetValue(ResourceType.Supply, out var existingSupplyCost))
                existingSupplyCost = 0;

            if (existingSupplyCost >= 0)
                type.Cost.AddValue(ResourceType.Supply, -Points);
            else
                type.Cost.SubtractValue(ResourceType.Supply, Points);
        }
    }

    public class LongerRange : PassiveFeature
    {
        public LongerRange(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public override string Name => "Range";

        public override string Description => $"Increases attack range by {ExtraPoints} tiles";

        public override string Symbol => "♐";

        private int ExtraPoints { get; set; }

        public override void Initialize(EntityType type)
        {
            Attack? feature = type.Features.FirstOrDefault(f => f is Attack) as Attack;

            if (feature == null)
                return;

            feature.Range += ExtraPoints;
        }

        /*
        public override bool Validate(EntityType type)
        {// only allow this feature exactly once
            return type.Features.SingleOrDefault(f => f is Attack) != null;
        }
        */
    }
}
