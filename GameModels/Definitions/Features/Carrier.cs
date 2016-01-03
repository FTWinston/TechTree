using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class Carrier : PassiveFeature
    {
        public Carrier(int capacity)
        {
            Capacity = capacity;
        }

        public override string Name { get { return "Carrier"; } }
        public override string GetDescription() { return string.Format("Carries up to {0} units", Capacity); }
        public override char Appearance { get { return '+'; } }
        private int Capacity { get; set; }

        public override double Initialize(EntityType type)
        {
            return 1.0 + Capacity * 0.1;
        }
    }
}
