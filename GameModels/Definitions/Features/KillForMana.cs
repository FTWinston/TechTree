using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class KillForMana : EntityTargettedFeature
    {
        public KillForMana(int range, float manaPerHitpoint)
        {
            Range = range;
            ManaPerHitpoint = manaPerHitpoint;
        }

        public override string Name => "Kill for Mana";

        public override string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Kills a friendly unit ");

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                sb.Append(", and restores ");
                sb.Append(ManaPerHitpoint.ToString("n1"));
                sb.Append(" mana for each hitpoint the killed unit had");

                return sb.ToString();
            }
        }

        public override string Symbol => "♏";

        public float ManaPerHitpoint { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Units | TargetingOptions.SameOwner;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
