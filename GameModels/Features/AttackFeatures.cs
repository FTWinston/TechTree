using GameModels.Definitions;
using GameModels.Features;
using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public partial class Feature
    {
        internal static IList<Func<Feature>> GetAttackFeatures()
        {
            return new Func<Feature>[] {
                () => new RangedAttack(),
                () => new RangedAttack(),
                () => new MeleeAttack(),
            };
        }
    }
}

namespace GameModels.Features
{
    public class RangedAttack : ActivatedFeature
    {
        public override string Name { get { return "Attack"; } }
        public override string Description { get { return "Deals damage to an enemy several tiles away"; } }
        public override char Appearance { get { return '='; } }
        public int Range { get; protected set; }

        public override double Initialize(EntityType type, Random r)
        {
            // TODO: damage and range should be based on tier and role
            Range = r.Next(1, 4);
            int damage = r.Next(5, 10);
            return 1;// +0.2 * Range;
        }

        public override void Activate(Entity entity)
        {
            throw new NotImplementedException();
        }
    }

    public class MeleeAttack : RangedAttack
    {
        public override string Description { get { return "Deals damage to an adjacent enemy"; } }
        public override char Appearance { get { return '-'; } }

        public override double Initialize(EntityType type, Random r)
        {
            // TODO: damage should be based on tier and role
            Range = 0;
            int damage = r.Next(7, 15);
            return 1;// .25;
        }

        public override void Activate(Entity entity)
        {
            // first move, then activate as before
            throw new NotImplementedException();

            base.Activate(entity);
        }
    }
}
