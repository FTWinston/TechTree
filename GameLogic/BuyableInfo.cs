using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GameLogic
{
    public abstract class BuyableInfo : IComparable<BuyableInfo>
    {
        protected BuyableInfo(TechTree tree) { Tree = tree; }
        protected TechTree Tree { get; private set; }

        public string Name { get; internal set; }
        
        public int MineralCost { get; internal set; }
        public int GasCost { get; internal set; }

        public float BuildTime { get; internal set; }

        internal void AllocateUniqueName(Random r, List<string> usedNames)
        {
            int nameIndex;
            do
            {
                AllocateName(r);
                nameIndex = usedNames.BinarySearch(Name);
            } while (nameIndex >= 0);
            usedNames.Insert(~nameIndex, Name);
        }
        
        protected abstract void AllocateName(Random r);

        public int CompareTo(BuyableInfo other)
        {
            return Name.CompareTo(other.Name);
        }
    }

    public abstract class EntityInfo : BuyableInfo
    {
        protected EntityInfo(TechTree tree) : base(tree) { }

        public int Health { get; internal set; }
        public int Armor { get; internal set; }
        public int Mana { get; internal set; }
        public int SupplyCost { get; internal set; }
    }

    public class BuildingInfo : EntityInfo, IComparable<BuildingInfo>
    {
        public BuildingInfo(TechTree tree) : base(tree) { }

        private int row, col;
        public int TreeColumn
        {
            get { return row; }
            internal set { row = value; Tree.MaxTreeColumn = Math.Max(Tree.MaxTreeColumn, row); }
        }
        public int TreeRow
        {
            get { return col; }
            internal set { col = value; Tree.MaxTreeRow = Math.Max(Tree.MaxTreeRow, col); }
        }
        public Color TreeColor { get; internal set; }

        public int CompareTo(BuildingInfo other)
        {
            var result = TreeRow.CompareTo(other.TreeRow);
            if (result != 0)
                return result;

            return TreeColumn.CompareTo(other.TreeColumn);
        }

        internal int CountDescendents()
        {
            int num = Unlocks.Count;
            foreach (var node in Unlocks)
                num += node.CountDescendents();
            return num;
        }

        public List<BuildingInfo> Prerequisites = new List<BuildingInfo>(), Unlocks = new List<BuildingInfo>();

        public List<UnitInfo> Builds = new List<UnitInfo>();
        public List<ResearchInfo> Research = new List<ResearchInfo>();

        public enum BuildingType
        {
            Factory,
            Tech,
            Defense,
            Resource,
        }

        public BuildingType Type { get; internal set; }

        protected override void AllocateName(Random r)
        {
            string[] names;
            switch (Type)
            {
                case BuildingType.Factory:
                    names = new string[] {"Factory", "Barracks", "Constructor", "Gateway", "Starport", "Stargate", "Garrison", "Citadel", "Hangar", "Manufactory", "Fabricator" }; break;
                case BuildingType.Tech:
                    names = new string[] {"Engineering Bay", "Armory", "Research Lab", "Psi Lab", "Academy", "Workshop", "Machine Shop", "Arsenal", "Archives", "Foundry", "Forge", "Tech Lab", "Fusion Core", "Physics Lab", "Particle Accelerator" }; break;
                case BuildingType.Resource:
                    names = new string[] {"Refinery", "Extractor", "Assimilator", "Mine" }; break;
                case BuildingType.Defense:
                    names = new string[] {"Missile Turret", "Scanner", "Bunker", "Flak Cannon", "Artillery", "AA Gun", "Tesla Coil", "Laser Turret", "Gun Turret" }; break;
                default:
                    Name = "Unknown type"; return;
            }

            Name = names[r.Next(names.Length)];
        }
    }

    public class UnitInfo : EntityInfo
    {
        public UnitInfo(TechTree tree) : base(tree) { }

        protected override void AllocateName(Random r)
        {
            throw new NotImplementedException();
        }

        [Flags]
        public enum UnitType
        {
            None = 0,
            Infantry = 1,
            Armor = 2,
            Robotic = 4,
            Bio = 8,
            Air = 16,
        }

        public UnitType Type { get; internal set; }
    }

    public class ResearchInfo : BuyableInfo
    {
        public ResearchInfo(TechTree tree) : base(tree) { }

        protected override void AllocateName(Random r)
        {
            throw new NotImplementedException();
        }
    }
}
