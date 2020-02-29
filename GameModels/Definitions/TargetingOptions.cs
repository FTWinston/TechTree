using System;

namespace ObjectiveStrategy.GameModels.Definitions
{
    [Flags]
    public enum TargetingOptions
    {
        None = 0,

        Self = 1 << 0,
        SameOwner = 1 << 1,
        Allies = 1 << 2,
        Enemies = 1 << 3,

        FriendlyOwner = SameOwner | Allies,
        OtherOwner = Allies | Enemies,
        AnyOwner = SameOwner | Allies | Enemies,

        Units = 1 << 4,
        Buildings = 1 << 5,
        AnyType = 1 << 6,

        RequiresMana = 1 << 7,
    }
}