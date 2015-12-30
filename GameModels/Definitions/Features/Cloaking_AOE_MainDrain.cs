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
    public class Cloaking_AOE_ManaDrain : ToggleFeature<AreaInvisible>
    {
        public override string Name { get { return "AOE Cloaking"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Prevents this unit from being seen by enemy units that lack the [detector] feature");
            return sb.ToString();
        }
        public override char Appearance { get { return '~'; } }

        public Cloaking_AOE_ManaDrain(int manaCostPerTurn, int activateManaCost)
        {
            ManaCostPerTurn = manaCostPerTurn;
            ActivateManaCost = activateManaCost;
        }
    }
}
