using ObjectiveStrategy.GameModels.Extensions;
using ObjectiveStrategy.GameModels.Instances;
using System.Collections.Generic;
using System.Linq;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class HigherHealth : PassiveFeature
    {
        public HigherHealth(uint id, string name, string symbol, int extraPoints)
            : base(id, name, symbol)
        {
            ExtraPoints = extraPoints;
        }

        public HigherHealth(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol)
        {
            ExtraPoints = data["points"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("points", ExtraPoints);
            return data;
        }

        internal const string TypeID = "passive health";

        protected override string TypeIdentifier => TypeID;

        public override string Description => $"Adds {ExtraPoints} hitpoints";

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
        public Armored(uint id, string name, string symbol, int extraPoints)
            : base(id, name, symbol)
        {
            ExtraPoints = extraPoints;
        }

        public Armored(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol)
        {
            ExtraPoints = data["points"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("points", ExtraPoints);
            return data;
        }

        internal const string TypeID = "passive armor";

        protected override string TypeIdentifier => TypeID;

        public override string Description => $"Adds {ExtraPoints} armor points";

        private int ExtraPoints { get; set; }

        public override void Initialize(EntityType type)
        {
            type.Armor += ExtraPoints;
        }
    }

    public class GreaterMobility : PassiveFeature
    {
        public GreaterMobility(uint id, string name, string symbol, int extraPoints)
            : base(id, name, symbol)
        {
            ExtraPoints = extraPoints;
        }

        public GreaterMobility(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol)
        {
            ExtraPoints = data["points"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("points", ExtraPoints);
            return data;
        }

        internal const string TypeID = "passive movement";

        protected override string TypeIdentifier => TypeID;

        public override string Description => $"Increases movement range by {ExtraPoints} cells";

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
        public GreaterVisibility(uint id, string name, string symbol, int extraPoints)
            : base(id, name, symbol)
        {
            ExtraPoints = extraPoints;
        }

        public GreaterVisibility(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol)
        {
            ExtraPoints = data["points"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("points", ExtraPoints);
            return data;
        }

        internal const string TypeID = "passive vision";

        protected override string TypeIdentifier => TypeID;

        public override string Description => $"Increases vision range by {ExtraPoints} tiles";

        private int ExtraPoints { get; set; }

        public override void Initialize(EntityType type)
        {
            type.VisionRange += ExtraPoints;
        }
    }

	public class HigherMana : PassiveFeature
    {
        public HigherMana(uint id, string name, string symbol, int extraPoints)
            : base(id, name, symbol)
        {
            ExtraPoints = extraPoints;
        }

        public HigherMana(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol)
        {
            ExtraPoints = data["points"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("points", ExtraPoints);
            return data;
        }

        internal const string TypeID = "passive mana";

        protected override string TypeIdentifier => TypeID;

        public override string Description => $"Adds {ExtraPoints} mana points";

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
        public Detector(uint id, string name, string symbol)
            : base(id, name, symbol)
        { }

        public Detector(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol)
        { }

        internal const string TypeID = "detector";

        protected override string TypeIdentifier => TypeID;

        public override string Description => "Allows detection of invisible units";

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
        public Supply(uint id, string name, string symbol, int points)
            : base(id, name, symbol)
        {
            Points = points;
        }

        public Supply(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol)
        {
            Points = data["points"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("points", Points);
            return data;
        }

        internal const string TypeID = "supply";

        protected override string TypeIdentifier => TypeID;

        public override string Description => $"Provides {Points} supply points, allowing units to be built";
        
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
        public LongerRange(uint id, string name, string symbol, int extraPoints)
            : base(id, name, symbol)
        {
            ExtraPoints = extraPoints;
        }

        public LongerRange(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol)
        {
            ExtraPoints = data["points"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("points", ExtraPoints);
            return data;
        }

        internal const string TypeID = "passive attack range";

        protected override string TypeIdentifier => TypeID;

        public override string Description => $"Increases attack range by {ExtraPoints} tiles";

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
