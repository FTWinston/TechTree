using System;

namespace ObjectiveStrategy.GameModels.Instances
{
    public enum FeatureState
    {
        None = 0,
        Enabled = 1 << 0,
        Cooldown = 1 << 1,
        CanTrigger = 1 << 2,
        AutoTrigger = 1 << 3,
        CanAutoTrigger = 1 << 4,
        RequiresTarget = 1 << 5,
        CanTargetHostileUnits = 1 << 6,
        CanTargetFriendlyUnits = 1 << 7,
    }
}
