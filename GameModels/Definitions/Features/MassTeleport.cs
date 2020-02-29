﻿using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class MassTeleport : TargettedStatusEffectFeature<Teleporting>
    {
        public override string Name => "Mass Teleport";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Teleports friendly units in a ");

                sb.Append(Radius);
                sb.Append(" tile radius,");

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                sb.Append(", to this unit");

                return sb.ToString();
            }
        }

        public override string Symbol => "⚝";

        public int Radius { get; }

        public MassTeleport(int range, int radius)
        {
            Range = range;
            Radius = radius;
        }

        public override TargetingOptions AllowedTargets => TargetingOptions.SameOwner | TargetingOptions.Units | TargetingOptions.Self;
    }
}
