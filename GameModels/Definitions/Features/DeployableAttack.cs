using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class DeployableAttack : ToggleFeature
    {
        public DeployableAttack(uint id, string name, string symbol, int activateManaCost, int manaCostPerTurn)
            : base(id, name, symbol, activateManaCost, manaCostPerTurn) { }

        public DeployableAttack(uint id, string name, string symbol, Dictionary<string, int> data) 
            : base(id, name, symbol, data) { }

        internal const string TypeID = "deployable attack";

        protected override string TypeIdentifier => TypeID;

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
