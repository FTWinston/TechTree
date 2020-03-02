﻿using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System;
using System.Linq;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class HealOverTime : TargettedStatusEffectFeature<Healing>
    {
        public HealOverTime(int range, int duration, int damageMin, int damageMax)
        {
            Range = range;
            Effect.Duration = duration;
            Effect.DamageMin = damageMin;
            Effect.DamageMax = damageMax;
        }

        public const string TypeID = "heal over time";

        public override string Type => TypeID;

        public override string Name => "Healing over Time";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(Effect.GetDescription());
                sb.Append(", to a unit");

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

        public override string Symbol => "⚕";

        public override TargetingOptions AllowedTargets => TargetingOptions.SameOwner | TargetingOptions.Units;

        /*
        public override bool Validate(EntityType type)
        {
            // don't allow on a type that also has heal-over-time
            return type.Features.FirstOrDefault(f => f is InstantHeal) == null;
        }
        */
    }
}
