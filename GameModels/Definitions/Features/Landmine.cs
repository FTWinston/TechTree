﻿using ObjectiveStrategy.GameModels.Instances;
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
