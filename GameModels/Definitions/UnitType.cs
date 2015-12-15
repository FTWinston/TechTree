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
        }

        public UnitFlags Flags { get; internal set; }

        internal enum Role
        {
            Fighter, // higher health/armor/damage
            Caster, // higher mana, more/better spell features
            Hybrid, // halfway between fighter and caster
            //Support, // carrier, healer, observer, etc
            //Worker,
        }

        internal Role UnitRole { get; set; }

        internal int Tier { get; set; }
    }
}
