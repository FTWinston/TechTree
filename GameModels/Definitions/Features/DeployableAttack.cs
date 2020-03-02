using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Serialization;
using System;
using System.Collections.Generic;
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

        public DeployableAttack(Dictionary<string, int> data)
        {
            ManaCostPerTurn = data["manaCostPerTurn"];
            ActivateManaCost = data["activateManaCost"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "manaCostPerTurn", ManaCostPerTurn },
                { "activateManaCost", ActivateManaCost },
            });
        }

        public const string TypeID = "deployable attack";

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
