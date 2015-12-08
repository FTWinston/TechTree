using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public abstract class EntityType
    {
        public string Name { get; internal set; }
        public char Appearance { get; internal set; }

        public int Health { get; internal set; }
        public int Armor { get; internal set; }
        public int Mana { get; internal set; }

        public int VisionRange { get; internal set; }
        public bool IsDetector { get; internal set; }

        // TODO: decide if we really do want to use action points for movement etc
        public int ActionPoints { get; internal set; }

        public int BuildTime { get; internal set; }

        public int MineralCost { get; internal set; }
        public int VespineCost { get; internal set; }
        public int SupplyCost { get; internal set; }

        public List<Feature> Features { get; private set; }

        protected EntityType()
        {
            Features = new List<Feature>();
        }

        internal virtual void Describe(StringBuilder sb)
        {
            sb.AppendFormat("{0}: {1} health, {2} armor, {3} mana", Name, Health, Armor, Mana);
            sb.AppendLine();
            
            sb.AppendFormat("costs {0} minerals, {1} vespine, {3}{2} supply, {4} turn{5} to build", MineralCost, VespineCost, Math.Abs(SupplyCost), SupplyCost < 0 ? "generates " : string.Empty, BuildTime, BuildTime == 1 ? string.Empty : "s");
            sb.AppendLine();

            sb.AppendFormat("{0}-tile vision range, {1} action points{2}", VisionRange, ActionPoints, IsDetector ? ", DETECTOR" : string.Empty);
            sb.AppendLine();

            foreach (var feature in Features)
            {
                sb.AppendFormat("{0}: {1}", feature.Name, feature.Description);
                sb.AppendLine();
            }
        }
    }

    public abstract class EntityType<I> : EntityType
        where I : Entity
    {
        public I CreateInstance()
        {
            throw new NotImplementedException();
        }
    }
}
