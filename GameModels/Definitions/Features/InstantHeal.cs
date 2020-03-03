using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class InstantHeal : EntityTargettedFeature
    {
        public InstantHeal(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int damageMin, int damageMax)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Range = range;
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public InstantHeal(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
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

        internal const string TypeID = "instant heal";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Restores ");
                sb.Append(DamageMin);
                if (DamageMin != DamageMax)
                {
                    sb.Append("-");
                    sb.Append(DamageMax);
                }
                sb.Append(" health to a unit");

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

        public override TargetingOptions AllowedTargets => TargetingOptions.FriendlyOwner | TargetingOptions.Units;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }

        /*
        public override bool Validate(EntityType type)
        {
            // don't allow on a type that also has heal-over-time
            return type.Features.FirstOrDefault(f => f is HealOverTime) == null;
        }
        */
    }
}
