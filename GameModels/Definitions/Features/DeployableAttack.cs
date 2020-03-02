using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class DeployableAttack : ToggleFeature
    {
        public DeployableAttack(int manaCostPerTurn, int activateManaCost)
        {
            ManaCostPerTurn = manaCostPerTurn;
            ActivateManaCost = activateManaCost;
        }

        public const string TypeID = "deployable attack";

        public override string Type => TypeID;

        public override string Name => "Deploy";

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Deploys this unit into an immobile configuration with a greater attack range and damage");
                return sb.ToString();
            }
        }

        public override string Symbol => "⚗";

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
