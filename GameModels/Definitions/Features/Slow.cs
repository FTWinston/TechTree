using ObjectiveStrategy.GameModels.Definitions.StatusEffects;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Slow : TargettedStatusEffectFeature<Slowed>
    {
        public Slow(uint id, string name, string symbol, int manaCost, int? limitedUses, int? cooldown, int range, int radius, int duration, int reducedPoints)
            : base(id, name, symbol, manaCost, limitedUses, cooldown, range)
        {
            Radius = radius;
            Effect.Duration = duration;
            Effect.ReducedPoints = reducedPoints;
        }

        public Slow(uint id, string name, string symbol, Dictionary<string, int> data)
            : base(id, name, symbol, data)
        {
            Radius = data["radius"];
            Effect.Duration = data["duration"];
            Effect.ReducedPoints = data["reducedPoints"];
        }

        protected override Dictionary<string, int> SerializeData()
        {
            var data = base.SerializeData();
            data.Add("radius", Radius);
            data.Add("duration", Effect.Duration);
            data.Add("reducedPoints", Effect.ReducedPoints);
            return data;
        }

        internal const string TypeID = "slow";

        protected override string TypeIdentifier => TypeID;

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append("Reduces by ");
                sb.Append(Effect.ReducedPoints);
                sb.Append(" the number of action points available to units");

                if (Radius != 1)
                {
                    sb.Append(" in a ");
                    sb.Append(Radius);
                    sb.Append(" tile radius,");
                }

                if (Range == 1)
                    sb.Append(" 1 tile away");
                else
                {
                    sb.Append(" up to ");
                    sb.Append(Range);
                    sb.Append(" tiles away");
                }

                sb.Append(" from moving or taking any actions");

                if (Effect.Duration > 0)
                {
                    sb.Append(", for ");
                    sb.Append(Effect.Duration);
                    sb.Append(" turns");
                }

                return sb.ToString();
            }
        }

        public int Radius { get; }

        public override TargetingOptions AllowedTargets => TargetingOptions.Units | TargetingOptions.Enemies;
    }
}
