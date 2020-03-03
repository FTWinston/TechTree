using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class KillForMana : EntityTargettedFeature
    {
        public KillForMana(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, float manaPerHitpoint)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            ManaPerHitpoint = manaPerHitpoint;
        }

        public KillForMana(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            ManaPerHitpoint = data["manaPerHp"] / 100f;
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("manaPerHp", (int)(ManaPerHitpoint * 100));
            return data;
        }

        internal const string TypeID = "kill for mana";

        protected override string Identifier => TypeID;

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

        public float ManaPerHitpoint { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Units | TargetingOptions.SameOwner;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
