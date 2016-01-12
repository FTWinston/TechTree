using GameModels.Definitions;
using GameModels.Definitions.StatusEffects;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class Cloaking_AOE_Permanent : PassiveFeature
    {
        public override string Name { get { return "Cloaking"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Prevents this unit from being seen by enemy units that lack the [detector] feature");
            return sb.ToString();
        }
        public override string Symbol { get { return "⚶"; } }
        public int Radius { get; private set; }
        private AreaInvisible EffectInstance { get; set; }

        public Cloaking_AOE_Permanent(int radius)
        {
            Radius = radius;
            EffectInstance = new AreaInvisible();

            // TODO: work out how to actually apply this effect to units
        }

        public override bool Validate(EntityType type)
        {
            // an entity type should only have one cloak
            return type.Features.FirstOrDefault(f => f is Cloaking_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Burrow) == null;
        }
    }
}
