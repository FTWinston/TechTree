using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class Landmine : ActivatedFeature
    {
        public override string Name { get { return "Landmine"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Drops an invisible mine, which deals ");
            sb.Append(DamageMin);
            if (DamageMin != DamageMax)
            {
                sb.Append("-");
                sb.Append(DamageMax);
            }
            sb.Append(" damage to the next unit to enter this tile");

            return sb.ToString();
        }
        public override char Appearance { get { return 'n'; } }
        public int DamageMin { get; protected set; }
        public int DamageMax { get; protected set; }

        public Landmine(int damageMin, int damageMax)
        {
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public override void Activate(Entity user)
        {
            throw new NotImplementedException();
        }
    }
}
