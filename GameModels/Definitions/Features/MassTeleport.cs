using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class MassTeleport : TargettedStatusEffectFeature<Teleporting>
    {
        public MassTeleport(uint id, string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int radius)
            : base(id, name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Radius = radius;
        }

        public MassTeleport(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        {
            Radius = data["radius"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("radius", Radius);
            return data;
        }

        internal const string TypeID = "mass teleport";

        protected override string TypeIdentifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Teleports friendly units in a ");

                sb.Append(Radius);
                sb.Append(" tile radius,");

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                sb.Append(", to this unit");

                return sb.ToString();
            }
        }

        public int Radius { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.SameOwner | TargetingOptions.Units | TargetingOptions.Self;
    }
}
