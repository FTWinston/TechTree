using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class DeployableAttack : ToggleFeature
    {
        public DeployableAttack(string name, string symbol, int activateManaCost, int manaCostPerTurn)
            : base(name, symbol, activateManaCost, manaCostPerTurn) { }

        public DeployableAttack(string name, string symbol, Dictionary<string, int> data) 
            : base(name, symbol, data) { }

        public const string TypeID = "deployable attack";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Deploys this unit into an immobile configuration with a greater attack range and damage");
                return sb.ToString();
            }
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
