﻿using GameModels.Definitions;
using GameModels.Definitions.Features;
using GameModels.Generation;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class Construction : ActivatedFeature
    {
        public override string Name { get { return "Build"; } }
        protected override string GetDescription()
        {
            return "Constructs buildings";
        }

        public override string Symbol { get { return "⚒"; } }
        
        public Construction()
        {

        }

        public override void Activate(Entity user)
        {
            throw new NotImplementedException();
        }
    }
}