using ObjectiveStrategy.GameGeneration;
using ObjectiveStrategy.GameModels;
using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GameModels.Tests
{
    public class GameDefinitionSerialization
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(10, 2)]
        [InlineData(25, 3)]
        [InlineData(50, 4)]
        [InlineData(75, 5)]
        [InlineData(90, 6)]
        [InlineData(100, 7)]
        public void SerializeAndDeserialize(int complexity, int seed)
        {
            var before = new GameDefinitionFactory()
                .GenerateGame(complexity, seed);

            var serialized = before.ToJson();

            var after = GameDefinition.FromJson(serialized);

            Assert.NotNull(after);
            Assert.NotNull(after.TechTree);
            Assert.NotNull(after.Battlefield);
            Assert.NotNull(after.Objectives);

            Assert.Equal(before.Seed, after.Seed);
            Assert.Equal(before.Complexity, after.Complexity);
            Assert.Equal(before.TurnLimit, after.TurnLimit);

            AssertBattlefieldsEqual(before.Battlefield, after.Battlefield);
            AssertObjectivesEqual(before.Objectives, after.Objectives);
            AssertTechTreesEqual(before.TechTree, after.TechTree);

            var reserialized = after.ToJson();
            Assert.Equal(serialized, reserialized);
        }

        private static void AssertBattlefieldsEqual(Battlefield before, Battlefield after)
        {
            Assert.Equal(before.Width, after.Width);
            Assert.Equal(before.Height, after.Height);
            Assert.Equal(before.StartPositions, after.StartPositions);
            Assert.Equal(before.Cells.Length, after.Cells.Length);

            for (int iCell = 0; iCell < before.Cells.Length; iCell++)
            {
                var beforeCell = before.Cells[iCell];
                var afterCell = after.Cells[iCell];

                if (beforeCell == null)
                {
                    Assert.Null(afterCell);
                    continue;
                }

                Assert.NotNull(afterCell);
                Assert.Equal(beforeCell.ID, afterCell.ID);
                Assert.Equal(beforeCell.Row, afterCell.Row);
                Assert.Equal(beforeCell.Col, afterCell.Col);
                Assert.Equal(beforeCell.Type, afterCell.Type);

                if (beforeCell.Building == null)
                    Assert.Null(beforeCell.Building);
                else
                {
                    AssertBuildingsEqual(beforeCell.Building, afterCell.Building);
                }

                Assert.Equal(beforeCell.Units.Count, afterCell.Units.Count);
                for (int iUnit = 0; iUnit < beforeCell.Units.Count; iUnit++)
                {
                    var beforeUnit = beforeCell.Units[iUnit];
                    var afterUnit = afterCell.Units[iUnit];

                    AssertUnitsEqual(beforeUnit, afterUnit);
                }
            }
        }

        private void AssertTechTreesEqual(TechTree before, TechTree after)
        {
            Assert.Equal(before.Buildings.Count, after.Buildings.Count);
            Assert.Equal(before.Units.Count, after.Units.Count);
            Assert.Equal(before.Research.Count, after.Research.Count);

            foreach (var kvp in before.Buildings)
            {
                Assert.Contains(kvp.Key, after.Buildings.Keys);

                var beforeBuilding = kvp.Value;
                var afterBuilding = after.Buildings[kvp.Key];

                AssertBuildingTypesEqual(beforeBuilding, afterBuilding);
            }

            foreach (var kvp in before.Units)
            {
                Assert.Contains(kvp.Key, after.Units.Keys);

                var beforeUnit = kvp.Value;
                var afterUnit = after.Units[kvp.Key];

                AssertUnitTypesEqual(beforeUnit, afterUnit);
            }

            foreach (var kvp in before.Research)
            {
                Assert.Contains(kvp.Key, after.Research.Keys);

                var beforeResearch = kvp.Value;
                var afterResearch = after.Research[kvp.Key];

                AssertResearchEqual(beforeResearch, afterResearch);
            }
        }

        private static void AssertObjectivesEqual(IList<Objective> before, IList<Objective> after)
        {
            Assert.Equal(before.Count, after.Count);

            for (int iObj = 0; iObj < before.Count; iObj++)
            {
                var beforeObj = before[iObj];
                var afterObj = after[iObj];

                Assert.Equal(beforeObj.CellsByPlayer, afterObj.CellsByPlayer);
                Assert.Equal(beforeObj.Description, afterObj.Description);
                Assert.Equal(beforeObj.RelativeToOpponent, afterObj.RelativeToOpponent);
                Assert.Equal(beforeObj.Subject, afterObj.Subject);
                Assert.Equal(beforeObj.SubjectTypeID, afterObj.SubjectTypeID);
                Assert.Equal(beforeObj.TargetQuantity, afterObj.TargetQuantity);
                Assert.Equal(beforeObj.Value, afterObj.Value);
            }
        }

        private void AssertBuildingTypesEqual(BuildingType before, BuildingType after)
        {
            AssertEntityTypesEqual(before, after);

            Assert.Equal(before.Builds, after.Builds);
            Assert.Equal(before.DisplayColumn, after.DisplayColumn);
            Assert.Equal(before.DisplayRow, after.DisplayRow);
            Assert.Equal(before.Prerequisite, after.Prerequisite);
            Assert.Equal(before.Researches, after.Researches);
            Assert.Equal(before.Unlocks, after.Unlocks);
        }

        private void AssertUnitTypesEqual(UnitType before, UnitType after)
        {
            AssertEntityTypesEqual(before, after);

            Assert.Equal(before.MoveRange, after.MoveRange);
            Assert.Equal(before.Mobility, after.Mobility);
            Assert.Equal(before.BuiltBy, after.BuiltBy);
            Assert.Equal(before.Flags, after.Flags);
        }

        private static void AssertEntityTypesEqual(EntityType before, EntityType after)
        {
            Assert.Equal(before.Armor, after.Armor);
            Assert.Equal(before.BuildTime, after.BuildTime);
            Assert.Equal(before.Cost, after.Cost);
            Assert.Equal(before.Health, after.Health);
            Assert.Equal(before.ID, after.ID);
            Assert.Equal(before.IsDetector, after.IsDetector);
            Assert.Equal(before.Mana, after.Mana);
            Assert.Equal(before.Name, after.Name);
            Assert.Equal(before.Symbol, after.Symbol);
            Assert.Equal(before.UpgradesFrom, after.UpgradesFrom);
            Assert.Equal(before.UpgradesTo, after.UpgradesTo);
            Assert.Equal(before.VisionRange, after.VisionRange);
            Assert.Equal(before.Features, after.Features);
            Assert.Equal(before.LockedFeatures, after.LockedFeatures);
        }

        private static void AssertBuildingsEqual(Building before, Building after)
        {
            AssertEntitiesEqual(before, after);
        }

        private static void AssertUnitsEqual(Unit before, Unit after)
        {
            AssertEntitiesEqual(before, after);

            Assert.Equal(before.MovementRemaining, after.MovementRemaining);
        }

        private static void AssertEntitiesEqual(Entity before, Entity after)
        {
            Assert.Equal(before.BaseDefinition.ID, after.BaseDefinition.ID);
            Assert.Equal(before.FeatureData, after.FeatureData);
            Assert.Equal(before.Health, after.Health);
            Assert.Equal(before.ID, after.ID);
            Assert.Equal(before.Location.ID, after.Location.ID);
            Assert.Equal(before.Mana, after.Mana);
            Assert.Equal(before.Owner.ID, after.Owner.ID);
            Assert.Equal(before.StatusEffects, after.StatusEffects);
            Assert.Equal(
                before.VisibleCells.Select(cell => cell.ID).ToArray(),
                after.VisibleCells.Select(cell => cell.ID).ToArray()
            );
        }

        private void AssertResearchEqual(Research before, Research after)
        {
            Assert.Equal(before.BuildTime, after.BuildTime);
            Assert.Equal(before.Cost, after.Cost);
            Assert.Equal(before.Description, after.Description);
            Assert.Equal(before.Name, after.Name);
            Assert.Equal(before.PerformedAt.ID, after.PerformedAt.ID);
            Assert.Equal(before.Prerequisite, after.Prerequisite);
            Assert.Equal(before.Symbol, after.Symbol);
            Assert.Equal(
                before.Unlocks.Select(feature => feature.ID).ToArray(),
                after.Unlocks.Select(feature => feature.ID).ToArray()
            );
        }
    }
}
