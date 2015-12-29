using GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModels.Definitions
{
    public class UnitType : EntityType<Unit>
    {
        private BuildingType builtBy;
        public BuildingType BuiltBy
        {
            get { return builtBy; }
            internal set
            {
                if (builtBy != null)
                    builtBy.Builds.Remove(this);

                builtBy = value;
                builtBy.Builds.Add(this);
            }
        }

        [Flags]
        public enum UnitFlags
        {
            None = 0,
            Agile = 1, // ignores difficult terrain
            Flying = 2, // ignores difficult and blocking terrain
            AttacksAir = 4,
            AttacksGround = 8,
        }

        public UnitFlags Flags { get; internal set; }

        internal enum Role
        {
            Worker,
            AllRounder,
            DamageDealer,
            Scout,
            MeatShield,
            Infiltrator,
            SupportCaster,
            OffensiveCaster,
            Transport,

            MaxValue
        }

        internal Role UnitRole { get; set; }

        internal int Tier { get; set; }
    }
}
