using GameModels.Definitions;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions.Features
{
    public class RemoveEffects : EntityTargettedFeature
    {
        public override string Name { get { return "Remove Effects"; } }
        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Removes all status effects from a target unit");
            
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
        public override string Symbol { get { return "⚜"; } }
        public int DamageMin { get; protected set; }
        public int DamageMax { get; protected set; }

        public RemoveEffects(int range)
        {
            Range = range;
        }

        public override bool IsValidTarget(Entity user, Entity target)
        {
            return target is Unit;
        }

        public override void Activate(Entity user, Cell target)
        {
            target.Entity.RemoveAllEffects();
            throw new NotImplementedException();
        }
    }
}
