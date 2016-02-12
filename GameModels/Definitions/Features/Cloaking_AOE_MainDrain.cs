﻿using GameModels.Definitions;
using GameModels.Definitions.StatusEffects;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class Cloaking_AOE_ManaDrain : ToggleFeature<AreaInvisible>
    {
        public override string Name { get { return "AOE Cloaking"; } }
        protected override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Prevents this unit from being seen by enemy units that lack the [detector] feature");
            return sb.ToString();
        }
        public override string Symbol { get { return "⛲"; } }
        public int Radius { get; private set; }

        public Cloaking_AOE_ManaDrain(int manaCostPerTurn, int activateManaCost, int radius)
        {
            Radius = radius;
            ManaCostPerTurn = manaCostPerTurn;
            ActivateManaCost = activateManaCost;
        }

        public override bool Validate(EntityType type)
        {
            // an entity type should only have one cloak
            return type.Features.FirstOrDefault(f => f is Cloaking_ManaDrain) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Cloaking_AOE_Permanent) == null
                && type.Features.FirstOrDefault(f => f is Burrow) == null;
        }
    }
}