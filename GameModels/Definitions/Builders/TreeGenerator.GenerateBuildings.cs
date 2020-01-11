using System;
using System.Collections.Generic;
using System.Linq;
using static GameModels.Definitions.Builders.BuildingBuilder;

namespace GameModels.Definitions.Builders
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

            GenerateSupplyBuilding(primaryResourceBuildingID);

            List<uint> factoryIDs = GenerateFactoryBuildings(primaryResourceBuildingID);

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
            int numberOfResources = Complexity < TreeComplexity.Extreme
                ? 2
                : 3;

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
                var building = GenerateResourceBuilding(resource, !primaryBuildingID.HasValue);

                Buildings.Add(identifier, building);

                if (primaryBuildingID.HasValue)
                {
                    // Given that resource buildings are the only ones in the tree, this can only cause branching
                    // if there are at least 3 resource buildings.
                    AddToSubtree(primaryBuildingID.Value, identifier, building);
                }
                else
                {
                    primaryBuildingID = identifier;
                }
            }

            return primaryBuildingID.Value;
        }

        private List<uint> GenerateFactoryBuildings(uint primaryResourceBuildingID)
        {
            uint prerequisiteID = primaryResourceBuildingID;

            var factoryIDs = new List<uint>();

            int complexity = (int)Complexity;

            for (int i = 0; i < complexity; i++)
            {
                var factory = GenerateFactoryBuilding();
                uint identifier = nextIdentifier++;
                Buildings.Add(identifier, factory);
                factoryIDs.Add(identifier);

                // Factories shouldn't always be a linear chain. Bigger trees should branch more.
                if (Random.Next(10) < complexity)
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
                    if (prevPrerequisite.HasValue & Random.Next(3) == 0)
                    {
                        AddUnlock(prevPrerequisite.Value, unitID, unit);
                        continue;
                    }

                    // generate a new tech building to be this unit type's prerequisite
                    BuildingBuilder techBuilding = GenerateTechBuilding();
                    uint identifier = nextIdentifier++;
                    Buildings.Add(identifier, techBuilding);
                    prevPrerequisite = identifier;
                    AddUnlock(identifier, unitID, unit);

                    // insert that into the tech tree somewhere in the factory's subtree
                    AddToSubtree(factoryID, identifier, techBuilding);
                }
            }
        }

        private void GenerateSupplyBuilding(uint prerequisiteID)
        {
            Resources.Add(ResourceType.Supply);

            var building = GenerateResourceBuilding(ResourceType.Supply, false);

            var identifier = nextIdentifier++;
            Buildings.Add(identifier, building);
            AddUnlock(prerequisiteID, identifier, building);
        }

        private BuildingBuilder GenerateResourceBuilding(ResourceType resource, bool isPrimary)
        {
            var building = new BuildingBuilder(Random, AllocateBuildingSymbol(), BuildingRole.Resource);
            building.AllocateName(UsedNames);
            building.VisionRange = BuildingVisionRange;

            // TODO: populate this

            return building;
        }

        private BuildingBuilder GenerateFactoryBuilding()
        {
            var building = new BuildingBuilder(Random, AllocateBuildingSymbol(), BuildingRole.Factory);
            building.AllocateName(UsedNames);
            building.VisionRange = BuildingVisionRange;

            // TODO: populate this

            return building;
        }

        private BuildingBuilder GenerateTechBuilding()
        {
            var building = new BuildingBuilder(Random, AllocateBuildingSymbol(), BuildingRole.Research);
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
            int numToMove = Random.Next(0, (int)Complexity);
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
                .Where(b => b != null);
        }
    }
}
