using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    internal class TechTheme
    {
        public UnitName[] UnitNames;
        public string[] BuildingNames_Factory;
        public string[] BuildingNames_Tech;
        public string[] BuildingNames_Defense;
        public string[] ResearchNames;

        public void AllocateName(UnitInfo unit, Random r, List<string> usedNames)
        {
            UnitName name;
            
            do
            {
                name = UnitNames[r.Next(UnitNames.Length)];
            } while ((name.Validation == null || name.Validation(unit)) && !EnsureUnique(name.Name, usedNames));

            unit.Name = name.Name;
        }

        public void AllocateName(BuildingInfo building, Random r, List<string> usedNames)
        {
            string[] possibilities;
            switch (building.Type)
            {
                case BuildingInfo.BuildingType.Factory:
                    possibilities = BuildingNames_Factory; break;
                case BuildingInfo.BuildingType.Tech:
                    possibilities = BuildingNames_Tech; break;
                case BuildingInfo.BuildingType.Defense:
                    possibilities = BuildingNames_Defense; break;
                default:
                    throw new NotImplementedException();
            }

            do
            {
                building.Name = possibilities[r.Next(possibilities.Length)];
            } while (!EnsureUnique(building.Name, usedNames));
        }

        public void AllocateName(ResearchInfo research, Random r, List<string> usedNames)
        {
            do
            {
                research.Name = ResearchNames[r.Next(ResearchNames.Length)];
            } while (!EnsureUnique(research.Name, usedNames));
        }

        private bool EnsureUnique(string name, List<string> usedNames)
        {
            int nameIndex = usedNames.BinarySearch(name);
            if (nameIndex >= 0)
                return false;

            usedNames.Insert(~nameIndex, name);
            return true;
        }

        public static TechTheme Infantry = new TechTheme()
        {
            UnitNames = new UnitName[] { "Grunt" /* max cost applies */, "Marine", "Infantry", "Soldier", "Trooper", "GI", "Commando" /* min cost applies */, "Mercenary" /* must have some active ability */, "Sniper" /* range > 2 */, "Medic" /* healing */, "Grenadier" /* explosive */, "Paratrooper" /* (only if paradrop) */, "Scout" /* speed > 3*/ },
            BuildingNames_Factory = new string[] { "Barracks", "Boot Camp", "Garrison", "Outpost" },
            BuildingNames_Tech = new string[] { "Academy", "Armory", "Rifle Range", "Weapons Depot", "Magazine", "Munitions Dump" },
            BuildingNames_Defense = new string[] { "Gun Turret", "Gun Emplacement", "Flak Cannon", "AA Gun", "Auto-Gun", "Bunker" },
            ResearchNames = new string[] { },
        };

        public static TechTheme Tanks = new TechTheme()
        {
            UnitNames = new UnitName[] { "Battle Tank", "Heavy Tank" /* health > X */, "Light Tank" /* health < Y */, "Artillery" /* range > 2 & explosive */ },
            BuildingNames_Factory = new string[] { "Factory", "War Factory", "Manufactory", "Fabricator", "Machine Shop" },
            BuildingNames_Tech = new string[] { "Machine Shop" },
            BuildingNames_Defense = new string[] { },
            ResearchNames = new string[] { },
        };

        // a factory from one theme can build a unit from another, e.g. Barracks trains Ghost, Gateway trains Templar ... however they have to be related somehow
        // ... you wouldn't want Ghosts to come from factories, or Templar from Starports. Perhaps that's just "unit type" (e.g. infantry, armor, robotic, air, bio) ?
        public static TechTheme Psionics = new TechTheme()
        {
            UnitNames = new UnitName[] { },
            BuildingNames_Factory = new string[] { },
            BuildingNames_Tech = new string[] { },
            BuildingNames_Defense = new string[] { },
            ResearchNames = new string[] { },
        };
        
        public struct UnitName
        {
            public string Name;
            public Func<UnitInfo, bool> Validation;

            public UnitName(string name, Func<UnitInfo, bool> validate)
            {
                Name = name;
                Validation = validate;
            }

            public static implicit operator UnitName(string s)
            {
                UnitName name;
                name.Name = s;
                name.Validation = null;
                return name;
            }
        }
    }
}
