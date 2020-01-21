using GameModels;
using GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TreeGeneration
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

            PrepareFeatureSelection();

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

        List<Dictionary<ResourceType, double>> ResourceCostRatioKeyframes = new List<Dictionary<ResourceType, double>>();

        private void DetermineResourceCostRatios()
        {
            ResourceCostRatioKeyframes.Clear();

            switch (Resources.Count)
            {
                case 1:
                    ResourceCostRatioKeyframes.Add(CreateResourceCostKeyframe(100));
                    break;
                case 2:
                    var scale = Random.NextDouble(0.25, 0.5);
                    ResourceCostRatioKeyframes.Add(CreateResourceCostKeyframe(100, scale));
                    ResourceCostRatioKeyframes.Add(CreateResourceCostKeyframe(0, 1 - scale));
                    break;
                case 3:
                default:
                    var frame1Scale = Random.NextDouble(0.25, 0.5);
                    var frame2Scale = Random.NextDouble(0.25, 0.75);
                    ResourceCostRatioKeyframes.Add(CreateResourceCostKeyframe(100, frame1Scale, 0));
                    ResourceCostRatioKeyframes.Add(CreateResourceCostKeyframe(0, 1 - frame1Scale, frame2Scale));
                    ResourceCostRatioKeyframes.Add(CreateResourceCostKeyframe(0, 0, 1 - frame2Scale));
                    break;
            }
        }

        private Dictionary<ResourceType, double> CreateResourceCostKeyframe(params double[] values)
        {
            var keyframe = new Dictionary<ResourceType, double>();

            for (int iValue = 0; iValue < values.Length; iValue++)
                keyframe[Resources[iValue]] = values[iValue];

            return keyframe;
        }

        private Dictionary<ResourceType, int> SplitResourceCosts(int overallCost, double progressionFraction)
        {
            var iSubsequent = (int)Math.Ceiling(progressionFraction * ResourceCostRatioKeyframes.Count);

            var previousFrame = ResourceCostRatioKeyframes[iSubsequent - 1];
            var subsequentFrame = ResourceCostRatioKeyframes[iSubsequent];
            var subsequentFraction = iSubsequent - progressionFraction;
            var previousFraction = 1 - subsequentFraction;

            var result = new Dictionary<ResourceType, int>();

            foreach (var resource in previousFrame.Keys)
            {
                double resourceScale = previousFrame[resource] * previousFraction
                    + subsequentFrame[resource] * subsequentFraction;

                result[resource] = (resourceScale * overallCost)
                    .RoundNearest(10);
            }

            return result;
        }
    }
}
