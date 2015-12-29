using GameModels.Definitions;
using GameModels.Features;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Features
{
    public class Attack : ActivatedFeature
    {
        public override string Name { get { return "Attack"; } }
        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Deals ");
                sb.Append(DamageMin);
                if (DamageMin != DamageMax)
                {
                    sb.Append("-");
                    sb.Append(DamageMax);
                }
                sb.Append(" damage to an enemy ");

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                return sb.ToString();
            }
        }
        public override char Appearance { get { return '='; } }
        public int Range { get; internal set; }
        public int DamageMin { get; protected set; }
        public int DamageMax { get; protected set; }

        public Attack(int range, int damageMin, int damageMax)
        {
            Range = range;
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public override void Activate(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
