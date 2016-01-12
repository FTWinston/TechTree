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
    public class Burrow : ToggleFeature<Burrowed>
    {
        public override string Name { get { return "Burrow"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Prevents this unit from moving, attacking, or being seen by enemy units that lack the [detector] feature");
            return sb.ToString();
        }
        public override string Symbol { get { return "♉"; } }

        public Burrow(int activateManaCost)
        {
            ActivateManaCost = activateManaCost;
        }

        public override bool Validate(EntityType type)
        {
            // an entity type should only have one cloak
            return type.Features.FirstOrDefault(f => f is Cloaking_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_Permanent) == null;
        }
    }
}
