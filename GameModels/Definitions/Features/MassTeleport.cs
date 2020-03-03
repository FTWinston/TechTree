using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class MassTeleport : TargettedStatusEffectFeature<Teleporting>
    {
        public MassTeleport(int range, int radius)
        {
            Range = range;
            Radius = radius;
        }

        public MassTeleport(string name, string symbol, Dictionary<string, int> data) { }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData());
        }

        public const string TypeID = "mass teleport";

        protected override string Identifier => TypeID;

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
