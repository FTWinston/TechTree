﻿using System;
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

            List<uint> factoryIDs = GenerateFactoryBuildings();

            AllocateUnitsToFactories(factoryIDs);

            GenerateTechBuildings(factoryIDs);

            // TODO: defense and possibly utility buildings
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
                    // TODO: primary building must be this building's prerequisite, or at least an ancestor
                    building.Prerequisite = primaryBuildingID;
                }
                else
                {
                    primaryBuildingID = identifier;
                }
            }

            return primaryBuildingID.Value;
        }

        private List<uint> GenerateFactoryBuildings()
        {
            var factoryIDs = new List<uint>();

            for (int i = (int)Complexity; i >= 0; i--)
            {
                var factory = GenerateFactoryBuilding();
                uint identifier = nextIdentifier++;
                Buildings.Add(identifier, factory);
                factoryIDs.Add(identifier);
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
                    if (prevPrerequisite != null & Random.Next(3) == 0)
                    {
                        unit.Prerequisite = prevPrerequisite;
                        continue;
                    }

                    // generate a new tech building to be this unit type's prerequisite
                    BuildingBuilder techBuilding = GenerateTechBuilding();
                    uint identifier = nextIdentifier++;
                    Buildings.Add(identifier, techBuilding);
                    prevPrerequisite = unit.Prerequisite = identifier;

                    // insert that into the tech tree somewhere in the factory's subtree
                    AddToSubtree(factoryID, techBuilding, identifier);
                }
            }
        }

        private void GenerateSupplyBuilding(uint prerequisiteID)
        {
            Resources.Add(ResourceType.Supply);

            var building = GenerateResourceBuilding(ResourceType.Supply, false);
            Buildings.Add(nextIdentifier++, building);
        }

        private BuildingBuilder GenerateResourceBuilding(ResourceType resource, bool isPrimary)
        {
            var building = new BuildingBuilder(Random, AllocateBuildingSymbol(), BuildingRole.Resource);
            building.VisionRange = BuildingVisionRange;

            // TODO: populate this

            return building;
        }

        private BuildingBuilder GenerateFactoryBuilding()
        {
            var building = new BuildingBuilder(Random, AllocateBuildingSymbol(), BuildingRole.Factory);
            building.VisionRange = BuildingVisionRange;

            // TODO: populate this

            return building;
        }

        private BuildingBuilder GenerateTechBuilding()
        {
            var building = new BuildingBuilder(Random, AllocateBuildingSymbol(), BuildingRole.Research);
            building.VisionRange = BuildingVisionRange;

            // TODO: populate this?

            return building;
        }

        private void AddToSubtree(uint rootID, BuildingBuilder newDescendent, uint newDescendentID)
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

                    newDescendent.Prerequisite = rootID;
                    return;
                }
            }

            // The more children a node has, the less likely it is to just have this child added directly to it.
            // For a node with 1 child, the chances of "falling on" to the next row are 1/3. For one with 2, it's 2/4, for one with 3, it's 3/5,for one with 4, it's 4/6, etc.
            if (numChildren == 0 || Random.Next(numChildren + 2) >= numChildren)
            {
                newDescendent.Prerequisite = rootID;
                return;
            }

            // Choose a building type that is unlocked by this one.
            var childID = childrenIDs[Random.Next(numChildren)];

            if (Random.Next(3) == 0)
            {// On a 1/3 chance, insert as a prerequisite of this other child building.
                BuildingBuilder child = Buildings[childID];
                child.Prerequisite = newDescendentID;
                newDescendent.Prerequisite = rootID;
            }
            else
            {// otherwise, add as a descendent of this other building
                AddToSubtree(childID, newDescendent, newDescendentID);
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