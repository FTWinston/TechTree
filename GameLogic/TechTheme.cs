﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    internal class TechTheme
    {
        public UnitInfo.UnitType UnitType { get; private set; }
        public Name<UnitInfo>[] UnitNames { get; private set; }
        public Name<BuildingInfo>[] BuildingNames_Factory { get; private set; }
        public Name<BuildingInfo>[] BuildingNames_Tech { get; private set; }
        public Name<BuildingInfo>[] BuildingNames_Defense { get; private set; }
        public Name<ResearchInfo>[] ResearchNames { get; private set; }
        public Attribute[] UnitAttributes { get; private set; }

        private void Allocate<T>(T info, Name<T>[] names, Random r, List<string> usedNames)
            where T : BuyableInfo
        {
            Name<T> name;
            int nameIndex;
            do
            {
                name = names[r.Next(names.Length)];
                nameIndex = usedNames.BinarySearch(name.Value);
            } while ((name.Validation == null || name.Validation(info)) && nameIndex >= 0);

            usedNames.Insert(~nameIndex, name.Value);
            info.Name = name.Value;
        }

        public void AllocateName(UnitInfo unit, Random r, List<string> usedNames)
        {
            Allocate(unit, UnitNames, r, usedNames);
        }

        public void AllocateName(BuildingInfo building, Random r, List<string> usedNames)
        {
            Name<BuildingInfo>[] names;
            switch (building.Type)
            {
                case BuildingInfo.BuildingType.Factory:
                    names = BuildingNames_Factory; break;
                case BuildingInfo.BuildingType.Tech:
                    names = BuildingNames_Tech; break;
                case BuildingInfo.BuildingType.Defense:
                    names = BuildingNames_Defense; break;
                default:
                    throw new NotImplementedException();
            }

            Allocate(building, names, r, usedNames);
        }

        public void AllocateName(ResearchInfo research, Random r, List<string> usedNames)
        {
            Allocate(research, ResearchNames, r, usedNames);
        }

        public static TechTheme ModernInfantry = new TechTheme()
        {
            UnitType = UnitInfo.UnitType.Infantry,
            UnitNames = new Name<UnitInfo>[] { "Grunt" /* max cost applies */, "Marine", "Infantry", "Soldier", "Trooper", "GI", "Commando" /* min cost applies */, "Mercenary" /* must have some active ability */, "Sniper" /* range > 2 */, "Medic" /* healing */, "Grenadier" /* explosive */, "Paratrooper" /* (only if paradrop) */, "Scout" /* speed > 3*/ },
            BuildingNames_Factory = new Name<BuildingInfo>[] { "Barracks", "Boot Camp", "Garrison", "Outpost", "Military Base", "Academy", "Troop Training" },
            BuildingNames_Tech = new Name<BuildingInfo>[] { "Academy", "Armory", "Rifle Range", "Weapons Depot", "Magazine", "Munitions Dump" },
            BuildingNames_Defense = new Name<BuildingInfo>[] { "Gun Turret", "Gun Emplacement", "Flak Cannon", "AA Gun", "Auto-Gun", "Bunker" },
            ResearchNames = new Name<ResearchInfo>[] { },
            UnitAttributes = new Attribute[] { },
        };

        public static TechTheme Tanks = new TechTheme()
        {
            UnitType = UnitInfo.UnitType.Armor,
            UnitNames = new Name<UnitInfo>[] { "Battle Tank", new Name<UnitInfo>("Heavy Tank", u => u.Health > 5), new Name<UnitInfo>("Light Tank", u => u.Health < 4), "Artillery" /* range > 2 & explosive */ },
            BuildingNames_Factory = new Name<BuildingInfo>[] { "Factory", "War Factory", "Manufactory", "Fabricator", "Machine Shop" },
            BuildingNames_Tech = new Name<BuildingInfo>[] { "Machine Shop", "Workshop", "Repair Depot", "Armory", "Arsenal", "Foundry", "Forge" },
            BuildingNames_Defense = new Name<BuildingInfo>[] { "Missile Turret", "Artillery", "Gun Turret", "Auto Cannon" },
            ResearchNames = new Name<ResearchInfo>[] { },
            UnitAttributes = new Attribute[] { },
        };

        public static TechTheme Aircraft = new TechTheme()
        {
            UnitType = UnitInfo.UnitType.Air,
            UnitNames = new Name<UnitInfo>[] { "Predator", "Recon drone", "Gunship", "Attack Copter", "Jet Fighter", "Stealth Fighter", "Bomber", "Stealth Bomber" },
            BuildingNames_Factory = new Name<BuildingInfo>[] { "Helepad", "Aerodrome", "Airstrip", "Air Base", "Hangar" },
            BuildingNames_Tech = new Name<BuildingInfo>[] { "Control Tower", "Radar Dome", "Hangar", "Engineering Bay", "Drone Uplink", "Stealth Lab", "Ordinance Storage", "Revetment" },
            BuildingNames_Defense = new Name<BuildingInfo>[] { "Missile Launcher", "AA Gun", "Laser Turret", "Flak Cannon" },
            ResearchNames = new Name<ResearchInfo>[] { },
            UnitAttributes = new Attribute[] { },
        };

        // a factory from one theme can build a unit from another, e.g. Barracks trains Ghost, Gateway trains Templar ... however they have share a unit type
        public static TechTheme Psionics = new TechTheme()
        {
            UnitType = UnitInfo.UnitType.Infantry,
            UnitNames = new Name<UnitInfo>[] { },
            BuildingNames_Factory = new Name<BuildingInfo>[] { },
            BuildingNames_Tech = new Name<BuildingInfo>[] { },
            BuildingNames_Defense = new Name<BuildingInfo>[] { },
            ResearchNames = new Name<ResearchInfo>[] { },
            UnitAttributes = new Attribute[] { },
        };
        
        public class Name<T>
            where T : BuyableInfo
        {
            public string Value { get; private set; }
            public Func<T, bool> Validation;

            public Name(string name, Func<T, bool> validate)
            {
                Value = name;
                Validation = validate;
            }

            public static implicit operator Name<T>(string s)
            {
                return new Name<T>(s, null);
            }
        }
    }
}