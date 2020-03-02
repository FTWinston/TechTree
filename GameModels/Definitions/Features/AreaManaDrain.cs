using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class AreaManaDrain : TargettedFeature
    {
        public AreaManaDrain() { }

        public AreaManaDrain(Dictionary<string, int> data) { }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>());
        }

        public const string TypeID = "area mana drain";

        public override string Name => "Mana Drain";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Drains all mana from everything in a ");
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

                return sb.ToString();
            }
        }

        public override string Symbol => "♊";

        public int Radius { get; internal set; }

        public AreaManaDrain(int range, int radius, int damageMin, int damageMax)
        {
            Range = range;
            Radius = radius;
        }

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
