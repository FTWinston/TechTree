using System;
using System.Collections.Generic;
using System.Linq;

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

        private Random Random { get; set; }

        public TreeComplexity Complexity { get; set; }

        private double BaseUnitValue { get; set; }

        private double MaxUnitValue { get; set; }

        private int UnitVisionRange { get; set; }

        private int BuildingVisionRange { get; set; }

        private int BaseUnitMoveRange { get; set; }

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
            
            BaseUnitValue = Random.NextDouble(1, 2);
            MaxUnitValue = Random.NextDouble(3, 5);

            UnitVisionRange = Random.Next(2.3, 4.7);
            BuildingVisionRange = Random.Next(UnitVisionRange - 0.7, UnitVisionRange + 0.7);

            BaseUnitMoveRange = Random.Next(2.3, 5.2);

            CreateUnits();

            GenerateBuildings();

            PositionBuildings();

            var buildingTreeOrder = SortBuildingsByAccessiblity();

            var unitTreeOrder = SortUnitsByAccessiblity(buildingTreeOrder);

            GenerateUnits(unitTreeOrder);

            // TODO: generate research?

            // TODO: determine building costs

            return new TechTree(this);
        }

        private List<BuildingBuilder> SortBuildingsByAccessiblity()
        {
            return Buildings.Values
                .OrderBy(b => b.DisplayRow)
                .ThenBy(b => b.DisplayColumn)
                .ToList();
        }

        private List<UnitBuilder> SortUnitsByAccessiblity(List<BuildingBuilder> buildingOrder)
        {
            return Units.Values
                .OrderBy(u => buildingOrder.IndexOf(Buildings[u.Prerequisite ?? u.BuiltBy]))
                .ToList();
        }
    }
}
