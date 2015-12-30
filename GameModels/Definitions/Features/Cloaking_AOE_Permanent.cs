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
        public override char Appearance { get { return '~'; } }

        private AreaInvisible EffectInstance { get; set; }

        public Cloaking_AOE_Permanent()
        {
            EffectInstance = new AreaInvisible();

            // TODO: work out how to actually apply this effect to units
        }
    }
}
