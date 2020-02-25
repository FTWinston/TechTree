using ObjectiveStrategy.GameModels.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using static ObjectiveStrategy.GameGeneration.TreeGeneration.BuildingBuilder;

namespace ObjectiveStrategy.GameGeneration.TreeGeneration
{
    public partial class TreeGenerator
    {
        private int nextBuildingSymbol = 0;

        private string AllocateBuildingSymbol()
        {
            return nextBuildingSymbol >= buildingSymbols.Length
                ? "-"
                : buildingSymbols[nextBuildingSymbol++].ToString();
        }

        protected void GenerateBuildings()
        {
            var primaryResourceBuildingID = GenerateResourceBuildings();

            var supplyBuildingID = GenerateSupplyBuilding(primaryResourceBuildingID);

            int supplyIsFactoryPrequisiteOdds = Buildings[supplyBuildingID].Prerequisite.HasValue
                ? 4
                : 2;

            var factoryPrequisiteID = Random.Next(supplyIsFactoryPrequisiteOdds) == 0
                ? primaryResourceBuildingID
                : supplyBuildingID;

            List<uint> factoryIDs = GenerateFactoryBuildings(factoryPrequisiteID);

            AllocateUnitsToFactories(factoryIDs);

            GenerateTechBuildings(factoryIDs);

            // TODO: defense and possibly utility buildings
        }

        private void AddUnlock(uint requiredBuildingID, uint unlockedItemID, EntityBuilder unlockedItem)
        {
            if (unlockedItem.Prerequisite.HasValue)
            {
                var prevPrerequisite = Buildings[unlockedItem.Prerequisite.Value];
                prevPrerequisite.Unlocks.Remove(unlockedItemID);
            }

            var requiredBuilding = Buildings[requiredBuildingID];
            unlockedItem.Prerequisite = requiredBuildingID;
            requiredBuilding.Unlocks.Add(unlockedItemID);
        }

        private uint GenerateResourceBuildings()
        {
            int numberOfResources = Complexity <= 15
                ? 1
                    : Complexity < 75
                        ? 2
                        : Complexity < 95
                            ? 3
                            : 4;

            var resources = Enum.GetValues(typeof(ResourceType))
                .Cast<ResourceType>()
                .Where(r => r != ResourceType.Supply)
                .ToList();

            resources.Randomize(Random);

            uint? primaryBuildingID = null;

            for (int i = 0; i < numberOfResources; i++)
            {
                var resource = resources[i];

                Resources.Add(resource);

                var identifier = nextIdentifier++;
                var building = GenerateResourceBuilding(identifier, resource, !primaryBuildingID.HasValue);

                Buildings.Add(identifier, building);

                if (primaryBuildingID.HasValue)
                {
                    // Resource buildings are currently the only ones in the tree, so it takes a few for this to actually cause branching.
                    // We might also not give it a prerequisite at all.
                    if (Random.Next(3) != 0)
                        AddToSubtree(primaryBuildingID.Value, identifier, building);
                }
                else
                {
                    primaryBuildingID = identifier;
                }
            }

            DetermineResourceCostRatios();

            if (!primaryBuildingID.HasValue)
                throw new InvalidOperationException();

            return primaryBuildingID.Value;
        }

        private List<uint> GenerateFactoryBuildings(uint prerequisiteID)
        {
            var factoryIDs = new List<uint>();

            int numFactories = Math.Max(1, Complexity / 10);

            for (int i = 0; i < numFactories; i++)
            {
                uint identifier = nextIdentifier++;
                var factory = GenerateFactoryBuilding(identifier);
                Buildings.Add(identifier, factory);
                factoryIDs.Add(identifier);

                // Factories shouldn't always be a linear chain. Bigger trees should branch more.
                if (Random.Next(10) < numFactories)
                    AddToSubtree(prerequisiteID, identifier, factory);
                else
                    AddUnlock(prerequisiteID, identifier, factory);

                prerequisiteID = identifier;
            }

            return factoryIDs;
        }

        private void GenerateTechBuildings(IEnumerable<uint> factoryIDs)
        {
            foreach (var factoryID in factoryIDs)
            {
                var factory = Buildings[factoryID];

                uint? prevPrerequisite = null;
                bool first = true;

                foreach (var unitID in factory.Builds)
                {
                    if (!Units.TryGetValue(unitID, out var unit))
                    {
                        continue;
                    }

                    // TODO: have each unit save the ID of its correponding tech building

                    if (first)
                    {// the first unit type built by a building never has any prerequisites ... doesn't mean it can't have upgrades in its own building or the next one, though.
                        first = false;
                        continue;
                    }

                    // give this unit type a 1 in 3 chance of sharing a prerequisite with its predecessor
                    if (prevPrerequisite.HasValue && Random.Next(3) == 0)
                    {
                        AddUnlock(prevPrerequisite.Value, unitID, unit);
                        continue;
                    }

                    // generate a new tech building to be this unit type's prerequisite
                    uint identifier = nextIdentifier++;
                    BuildingBuilder techBuilding = GenerateTechBuilding(identifier);
                    Buildings.Add(identifier, techBuilding);

                    prevPrerequisite = identifier;
                    AddUnlock(identifier, unitID, unit);

                    // insert that into the tech tree somewhere in the factory's subtree
                    AddToSubtree(factoryID, identifier, techBuilding);
                }
            }
        }

        private uint GenerateSupplyBuilding(uint primaryResourceBuilding)
        {
            Resources.Add(ResourceType.Supply);

            var identifier = nextIdentifier++;
            var building = GenerateResourceBuilding(identifier, ResourceType.Supply, false);
            Buildings.Add(identifier, building);

            if (Random.Next(3) == 0) // 1 in 3 chance of this requiring the "root" building
                AddUnlock(primaryResourceBuilding, identifier, building);

            return identifier;
        }

        private BuildingBuilder GenerateResourceBuilding(uint identifier, ResourceType resource, bool isPrimary)
        {
            var building = new BuildingBuilder(Random, identifier, AllocateBuildingSymbol(), BuildingRole.Resource);
            building.AllocateName(UsedNames);
            building.VisionRange = BuildingVisionRange;

            // TODO: populate this

            return building;
        }

        private BuildingBuilder GenerateFactoryBuilding(uint identifier)
        {
            var building = new BuildingBuilder(Random, identifier, AllocateBuildingSymbol(), BuildingRole.Factory);
            building.AllocateName(UsedNames);
            building.VisionRange = BuildingVisionRange;

            // TODO: populate this

            return building;
        }

        private BuildingBuilder GenerateTechBuilding(uint identifier)
        {
            var building = new BuildingBuilder(Random, identifier, AllocateBuildingSymbol(), BuildingRole.Research);
            building.AllocateName(UsedNames);
            building.VisionRange = BuildingVisionRange;

            // TODO: populate this?

            return building;
        }

        private void AddToSubtree(uint rootID, uint newDescendentID, BuildingBuilder newDescendent)
        {
            BuildingBuilder root = Buildings[rootID];

            var childrenIDs = OnlyBuildings(root.Unlocks).ToArray();
            int numChildren = childrenIDs.Length;

            // Sometimes we get a really boring tree, that's just A -> B -> C -> D -> E -> F -> G -> H (etc) with no branching.
            // Instead of generating a subtree like that, if the "ancestor chain" is too long, insert further up the tree, instead.
            int chainLengthWithNoSiblings = numChildren == 0 ? 1 : 0;

            if (chainLengthWithNoSiblings > 0)
            {
                var checkID = root.Prerequisite;

                while (checkID.HasValue)
                {
                    var check = Buildings[checkID.Value];

                    if (OnlyBuildings(check.Unlocks).Count() == 1)
                        chainLengthWithNoSiblings++;
                    else
                        break;

                    checkID = check.Prerequisite;
                }

                if (chainLengthWithNoSiblings > 2)
                {
                    // Insert further up the tree instead, and then return.
                    int stepsUp = Random.Next(1, chainLengthWithNoSiblings + 1);

                    for (int i = 0; i < stepsUp; i++)
                    {
                        if (!root.Prerequisite.HasValue)
                            break;

                        rootID = root.Prerequisite.Value;
                        root = Buildings[rootID];
                    }

                    AddUnlock(rootID, newDescendentID, newDescendent);
                    return;
                }
            }

            // The more children a node has, the less likely it is to just have this child added directly to it.
            // For a node with 1 child, the chances of "falling on" to the next row are 1/3. For one with 2, it's 2/4, for one with 3, it's 3/5,for one with 4, it's 4/6, etc.
            if (numChildren == 0 || Random.Next(numChildren + 2) >= numChildren)
            {
                AddUnlock(rootID, newDescendentID, newDescendent);
                return;
            }

            // Choose a building type that is unlocked by this one.
            var childID = childrenIDs[Random.Next(numChildren)];

            if (Random.Next(3) == 0)
            {// On a 1/3 chance, insert as a prerequisite of this other child building.
                BuildingBuilder child = Buildings[childID];
                AddUnlock(newDescendentID, childID, child);
                AddUnlock(rootID, newDescendentID, newDescendent);
            }
            else
            {// otherwise, add as a descendent of this other building
                AddToSubtree(childID, newDescendentID, newDescendent);
            }
        }

        private void AllocateUnitsToFactories(List<uint> factoryIDs)
        {
            var unallocatedUnitIDs = Units.Keys.ToList();
            unallocatedUnitIDs.Randomize(Random);
            factoryIDs.Randomize(Random);

            var unitBuckets = new Queue<Queue<uint>>();
            for (var i = 0; i < factoryIDs.Count; i++)
                unitBuckets.Enqueue(new Queue<uint>());

            // First, distribute the units evenly.
            foreach (var unitID in unallocatedUnitIDs)
            {
                var bucket = unitBuckets.Dequeue();
                bucket.Enqueue(unitID);
                unitBuckets.Enqueue(bucket);
            }

            // Then, randomly move some around.
            int numToMove = Random.Next(0, factoryIDs.Count);
            var bucketArray = unitBuckets.ToArray();
            for (int i = 0; i < numToMove; i++)
            {
                bucketArray.PickRandom2(Random, out var fromBucket, out var toBucket);
                toBucket.Enqueue(fromBucket.Dequeue());
            }

            // Now set up relationship between buildings and units.
            for (int iFactory = 0; iFactory < factoryIDs.Count; iFactory++)
            {
                var unitIDs = bucketArray[iFactory];

                var factoryID = factoryIDs[iFactory];

                var factory = Buildings[factoryID];

                foreach (var unitID in unitIDs)
                {
                    var unit = Units[unitID];
                    unit.BuiltBy = factoryID;
                    factory.Builds.Add(unitID);
                }
            }
        }

        private IEnumerable<uint> OnlyBuildings(IEnumerable<uint> IDs)
        {
            return IDs
                .Where(id => Buildings.ContainsKey(id));
        }

        private IEnumerable<BuildingBuilder> GetBuildings(IEnumerable<uint> IDs)
        {
            return IDs
                .Select(id => Buildings.TryGetValue(id, out var building) ? building : null)
                .Where(b => b != null)
                .Cast<BuildingBuilder>();
        }
    }
}
