using System;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public enum FeatureType
    {
        Passive, // Always on.
        Toggled, // Clicking it toggles it on/off.
        Triggered, // Clicking it activates it.
        Targeted, // Clicking it requires you to select a target to activate.
        AutoTargeted, // As above, but can also activate itself (outwith your turn). Auto-targeting can be enabled/disabled.
        Purchased, // Clicking it activates it (possibly starts a build time) and consumes resources.
    }
}
