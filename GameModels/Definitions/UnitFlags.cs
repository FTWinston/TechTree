using System;

namespace ObjectiveStrategy.GameModels.Definitions
{
    [Flags]
    public enum UnitFlags // TODO: split these into movement flags and attack flags
    {
        None = 0,
        Agile = 1, // ignores difficult terrain
        Flying = 2, // ignores difficult and blocking terrain
        AttacksAir = 4,
        AttacksGround = 8,
    }
}
