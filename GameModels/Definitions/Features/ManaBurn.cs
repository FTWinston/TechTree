﻿using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class ManaBurn : EntityTargettedFeature
    {
        public ManaBurn(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int maxMana, float damagePerMana)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            MaxMana = maxMana;
            DamagePerMana = damagePerMana;
        }

        public ManaBurn(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            MaxMana = data["maxMana"];
            DamagePerMana = data["damagePerMana"] / 100f;
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("maxMana", MaxMana);
            data.Add("damagePerMana", (int)(DamagePerMana * 100));
            return data;
        }

        internal const string TypeID = "mana burn";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                if (MaxMana > 0)
                {
                    sb.Append("Drains up to ");
                    sb.Append(MaxMana);
                }
                else
                    sb.Append("Drains all the");

                sb.Append(" mana from an enemy");

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                sb.Append(", dealing ");
                sb.Append(DamagePerMana.ToString("n1"));
                sb.Append(" damage for each point drained");

                return sb.ToString();
            }
        }

        public int MaxMana { get; }

        public float DamagePerMana { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Units | TargetingOptions.Enemies | TargetingOptions.RequiresMana;

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException(); ;
        }
    }
}
