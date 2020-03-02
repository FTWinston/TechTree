using ObjectiveStrategy.GameModels.Extensions;
using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class HigherHealth : PassiveFeature
    {
        public HigherHealth(int extraPoints)
        {
            ExtraPoints = extraPoints;
        }

        public HigherHealth(Dictionary<string, int> data)
        {
            ExtraPoints = data["points"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "points", ExtraPoints },
            });
        }

        public const string TypeID = "passive health";

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

        public Armored(Dictionary<string, int> data)
        {
            ExtraPoints = data["points"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "points", ExtraPoints },
            });
        }

        public const string TypeID = "passive armor";

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

        public GreaterMobility(Dictionary<string, int> data)
        {
            ExtraPoints = data["points"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "points", ExtraPoints },
            });
        }

        public const string TypeID = "passive movement";

        public override string Name => "Greater Mobility";

        public override string Description => $"Increases movement range by {ExtraPoints} cells";

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

        public GreaterVisibility(Dictionary<string, int> data)
        {
            ExtraPoints = data["points"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "points", ExtraPoints },
            });
        }

        public const string TypeID = "passive vision";

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

        public HigherMana(Dictionary<string, int> data)
        {
            ExtraPoints = data["points"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "points", ExtraPoints },
            });
        }

        public const string TypeID = "passive mana";

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
        public Detector() { }

        public Detector(Dictionary<string, int> data) { }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>());
        }

        public const string TypeID = "detector";

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

        public Supply(Dictionary<string, int> data)
        {
            Points = data["points"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "points", Points },
            });
        }

        public const string TypeID = "supply";

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

        public LongerRange(Dictionary<string, int> data)
        {
            ExtraPoints = data["points"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "points", ExtraPoints },
            });
        }

        public const string TypeID = "passive attack range";

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
