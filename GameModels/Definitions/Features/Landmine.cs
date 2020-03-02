using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectiveStrategy.GameModels.Definitions.Features
{
    public class Landmine : ActivatedFeature
    {
        public Landmine(int damageMin, int damageMax)
        {
            DamageMin = damageMin;
            DamageMax = damageMax;
        }

        public Landmine(Dictionary<string, int> data)
        {
            DamageMin = data["damageMin"];
            DamageMax = data["damageMax"];
        }

        public override FeatureDTO ToDTO()
        {
            return new FeatureDTO(TypeID, new Dictionary<string, int>()
            {
                { "damageMin", DamageMin },
                { "damageMax", DamageMax },
            });
        }

        public const string TypeID = "landmine";

        public override string Name => "Landmine";

        public override string Description
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append("Drops an invisible mine, which deals ");
                sb.Append(DamageMin);
                if (DamageMin != DamageMax)
                {
                    sb.Append("-");
                    sb.Append(DamageMax);
                }
                sb.Append(" damage to the next unit to enter this tile");

                return sb.ToString();
            }
        }

        public override string Symbol => "☌";

        public int DamageMin { get; }

        public int DamageMax { get; }

        protected override bool Trigger(Entity entity, Cell target, Dictionary<string, int> data)
        {
            throw new NotImplementedException();
        }
    }
}
