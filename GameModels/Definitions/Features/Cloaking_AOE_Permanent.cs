﻿using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Cloaking_AOE_Permanent : PassiveFeature
    {
        public Cloaking_AOE_Permanent(int radius)
        {
            Radius = radius;
            Effect = new AreaInvisible();

            // TODO: work out how to actually apply this effect to units
        }

        public Cloaking_AOE_Permanent(Dictionary<string, int> data)
            : this(data["radius"]) { }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "radius", Radius },
            });
        }

        public const string TypeID = "permanent aoe cloak";

        public override string Name => "Cloaking";

        public override string Description => "Prevents this unit from being seen by enemy units that lack the [detector] feature";

        public override string Symbol => "⚶";

        public int Radius { get; }

        private AreaInvisible Effect { get; }

        /*
        public override bool Validate(EntityType type)
        {
            // an entity type should only have one cloak
            return type.Features.FirstOrDefault(f => f is Cloaking_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Burrow) == null;
        }
        */
    }
}
