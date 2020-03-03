using ObjectiveStrategy.GameModels.Instances;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class RemoveEffects : EntityTargettedFeature
    {
        public RemoveEffects(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        { }

        public RemoveEffects(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        { }

        internal const string TypeID = "remove effects";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Removes all status effects from a target unit");

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                return sb.ToString();
            }
        }

        public override TargetingOptions AllowedTargets => TargetingOptions.AnyOwner | TargetingOptions.Units;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            foreach (var unit in target.Units)
                unit.RemoveAllEffects();

            return true;
        }
    }
}
