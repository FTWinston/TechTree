using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class InstantHeal : EntityTargettedFeature
    {
        public InstantHeal(int range, int damageMin, int damageMax)
        {
            Range = range;
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public const string TypeID = "instant heal";

        public override string Type => TypeID;

        public override string Name => "Instant Healing";

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

        public override string Symbol => "☤";

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
