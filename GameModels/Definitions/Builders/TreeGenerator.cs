using System;
using System.Collections.Generic;

namespace GameModels.Definitions.Builders
{
    public partial class TreeGenerator : BaseTechTree<BuildingBuilder, UnitBuilder>
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

        private HashSet<string> UsedNames { get; } = new HashSet<string>();

        private const string buildingSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string unitSymbols = "αβγδεζηθιλμνξπρσςφχψωυτκοϐϑϕϖϗϙϝϞϡϣϥϧϩϫϭϯϰϱϲϳ϶ϸϻ";
        private uint nextIdentifier = 0;

        public TechTree Generate()
        {
            Random = new Random(Seed);

            UsedNames.Clear();

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
