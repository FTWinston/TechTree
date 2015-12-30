using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.StatusEffects
{
    public class ReducedHealth : IStatusEffect
    {
        public int Duration { get; internal set; }
        public int ReductionMax { get; internal set; }
        public int MinRemainingHitpoints { get; internal set; }
        public int HealthDrainPerTurn { get; internal set; }

        public void BeforeFirstTick(Entity target) { }
        public void AfterLastTick(Entity target) { }

        public void Tick(Entity target)
        {
            throw new NotImplementedException();
        }

        public string Name { get { return "Reduced Health"; } }

        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Reduces maximum health by up to ");
            sb.Append(ReductionMax);

            if (Duration > 0)
            {
                sb.Append("for ");
                sb.Append(Duration);
                sb.Append(" turns");
            }

            if (MinRemainingHitpoints > 0)
            {
                sb.Append(", never dropping below ");
                sb.Append(MinRemainingHitpoints);
                sb.Append(MinRemainingHitpoints == 1 ? " hitpoint" : " hitpoints");
            }

            return sb.ToString();
        }
    }
}
