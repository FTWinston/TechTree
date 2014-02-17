using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GameLogic
{
    public class BuyableInfo
    {
        protected BuyableInfo(TechTree tree) { Tree = tree; }
        protected TechTree Tree { get; private set; }

        public string Name { get; set; }
        
        public int MineralCost { get; set; }
        public int GasCost { get; set; }

        public float BuildTime { get; set; }
    }

    public class EntityInfo : BuyableInfo
    {
        protected EntityInfo(TechTree tree) : base(tree) { }

        public int Health { get; set; }
        public int Armor { get; set; }
        public int Mana { get; set; }
        public int SupplyCost { get; set; }
    }

    public class BuildingInfo : EntityInfo, IComparable<BuildingInfo>
    {
        public BuildingInfo(TechTree tree) : base(tree) { }

        private int row, col;
        public int TreeColumn
        {
            get { return row; }
            set { row = value; Tree.MaxTreeColumn = Math.Max(Tree.MaxTreeColumn, row); }
        }
        public int TreeRow
        {
            get { return col; }
            set { col = value; Tree.MaxTreeRow = Math.Max(Tree.MaxTreeRow, col); }
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
    }

    public class UnitInfo : EntityInfo
    {
        public UnitInfo(TechTree tree) : base(tree) { }
    }

    public class ResearchInfo : BuyableInfo
    {
        public ResearchInfo(TechTree tree) : base(tree) { }
    }
}
