using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using ObjectiveStrategy.GameModels.Serialization;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class DrainHealth : TargettedStatusEffectFeature<HealthDrain>
    {
        public DrainHealth(string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int duration, int damagePerTurn, float manaPerHitpoint)
            : base(name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Effect.Duration = duration;
            Effect.DamagePerTurn = damagePerTurn;
            Effect.ManaPerHitpoint = manaPerHitpoint;
        }

        public DrainHealth(string name, string symbol, Dictionary<string, int> data)
            : base(name, symbol, data)
        {
            Effect.Duration = data["duration"];
            Effect.DamagePerTurn = data["damagePerTurn"];
            Effect.ManaPerHitpoint = data["manaPerHp"] / 100f;
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("duration", Effect.Duration);
            data.Add("damagePerTurn", Effect.DamagePerTurn);
            data.Add("manaPerHp", (int)(Effect.ManaPerHitpoint * 100));
            return data;
        }

        public const string TypeID = "drain health";

        protected override string Identifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Drains ");
                sb.Append(Effect.DamagePerTurn);
                sb.Append(" per turn");

                if (Effect.Duration > 1)
                {
                    sb.Append(", over ");
                    sb.Append(Effect.Duration);
                    sb.Append(" turns");
                }

                sb.Append(" from a unit");

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                sb.Append(", restoring ");
                sb.Append(Effect.ManaPerHitpoint.ToString("n1"));
                sb.Append(" mana to the caster for each hitpoint drained");

                return sb.ToString();
            }
        }

        public override TargetingOptions AllowedTargets => TargetingOptions.AnyOwner | TargetingOptions.Units;
    }
}
