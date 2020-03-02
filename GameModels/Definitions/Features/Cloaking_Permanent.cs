﻿using ObjectiveStrategy.GameModels.Definitions;
using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Instances;
using System.Linq;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Cloaking_Permanent : PassiveFeature
    {
        public Cloaking_Permanent()
        {
            Effect = new Invisible();

            // TODO: work out how to actually apply this effect to units
        }

        public const string TypeID = "permanent cloak";

        public override string Type => TypeID;

        public override string Name => "Cloaking";

        public override string Description => "Prevents this unit from being seen by enemy units that lack the [detector] feature";

        public override string Symbol => "☍";

        private Invisible Effect { get; set; }

        /*
        public override bool Validate(EntityType type)
        {
            // an entity type should only have one cloak
            return type.Features.FirstOrDefault(f => f is Cloaking_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Burrow) == null;
        }
        */
    }
}
