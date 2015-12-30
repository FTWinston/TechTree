using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.StatusEffects
{
    public class HealthBoost : IStatusEffect
    {
        public int Duration { get; internal set; }
        public int ExtraHealth { get; internal set; }
        public int ExtraArmor { get; internal set; }

        public void BeforeFirstTick(Entity target)
        {
            throw new NotImplementedException();
        }

        public void AfterLastTick(Entity target)
        {
            throw new NotImplementedException();
        }

        public void Tick(Entity target) { }

        public string Name { get { return "Health boost"; } }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Increases ");

            if (ExtraHealth != 0)
            {
                sb.Append("hitpoints by ");
                sb.Append(ExtraHealth);
            }

            if (ExtraArmor != 0)
            {
                if (ExtraHealth != 0)
                    sb.Append(" and ");

                sb.Append("armor by ");
                sb.Append(ExtraArmor);
            }

            if (Duration > 0)
            {
                sb.Append(", for ");
                sb.Append(Duration);
                sb.Append(" turns");
            }
            return sb.ToString();
        }
    }
}
