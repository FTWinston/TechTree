using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Definitions.Features;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Attack : EntityTargettedFeature
    {
        public Attack(uint id, string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int damageMin, int damageMax)
            : base(id, name, symbol, manaCost, limitedUses, cooldown, range)
        {
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public Attack(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        {
            DamageMin = data["damageMin"];
            DamageMax = data["damageMax"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("damageMin", DamageMin);
            data.Add("damageMax", DamageMax);
            return data;
        }

        internal const string TypeID = "attack";

        protected override string TypeIdentifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

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

        public int DamageMin { get; }

        public int DamageMax { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Enemies | TargetingOptions.Buildings | TargetingOptions.Units;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
