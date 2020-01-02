using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModels.Definitions.Builders
{
    public partial class TreeGenerator : ITechTree
    {
        public enum TreeComplexity
        {
            Simple = 2,
            Normal = 3,
            Complex = 4,
            Extreme = 5,
        }

        public TreeGenerator(TreeComplexity complexity = TreeComplexity.Normal)
            : this(new Random().Next(int.MinValue, int.MaxValue), complexity)
        { }

        public TreeGenerator(int seed, TreeComplexity complexity = TreeComplexity.Normal)
        {
            Seed = seed;
            Complexity = complexity;
        }


        public int Seed { get; }

        private Random Random { get; set; }

        public TreeComplexity Complexity { get; set; }

        private int UnitVisionRange { get; set; }

        private int BuildingVisionRange { get; set; }

        private List<ResourceType> Resources { get; } = new List<ResourceType>();

        private Dictionary<uint, BuildingBuilder> Buildings { get; } = new Dictionary<uint, BuildingBuilder>();

        private Dictionary<uint, UnitBuilder> Units { get; } = new Dictionary<uint, UnitBuilder>();

        public Dictionary<uint, Research> Research { get; } = new Dictionary<uint, Research>();

        Dictionary<uint, IBuildingType> ITechTree.Buildings => Buildings.ToDictionary<KeyValuePair<uint, BuildingBuilder>, uint, IBuildingType>(b => b.Key, b => b.Value);

        Dictionary<uint, IUnitType> ITechTree.Units => Units.ToDictionary<KeyValuePair<uint, UnitBuilder>, uint, IUnitType>(u => u.Key, u => u.Value);

        private const string buildingSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string unitSymbols = "αβγδεζηθιλμνξπρσςφχψωυτκοϐϑϕϖϗϙϝϞϡϣϥϧϩϫϭϯϰϱϲϳ϶ϸϻ";
        private uint nextIdentifier = 0;

        public TechTree Generate()
        {
            Random = new Random(Seed);

            nextBuildingSymbol = 0;
            nextUnitSymbol = 0;

            UnitVisionRange = Random.Next(2.3, 4.7);

            BuildingVisionRange = Random.Next(UnitVisionRange - 0.7, UnitVisionRange + 0.7);

            CreateUnits();

            GenerateBuildings();

            PositionBuildings();

            GenerateUnits();

            // TODO: generate research?

            return new TechTree(this);
        }
    }
}
