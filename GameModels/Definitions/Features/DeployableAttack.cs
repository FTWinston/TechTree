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
    public class DeployableAttack : ToggleFeature
    {
        public override string Name { get { return "Deploy"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Deploys this unit into an immobile configuration with a greater attack range and damage");
            return sb.ToString();
        }
        public override string Symbol { get { return "⚗"; } }
        
        public DeployableAttack(int manaCostPerTurn, int activateManaCost)
        {
            ManaCostPerTurn = manaCostPerTurn;
            ActivateManaCost = activateManaCost;
        }

        public override void Enable(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override void Disable(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
