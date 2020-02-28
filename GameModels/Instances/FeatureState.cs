namespace ObjectiveStrategy.GameModels.Instances
{
    public enum FeatureState
    {
        None = 0,
        Disabled = 1 << 0,
        CanTrigger = 1 << 1,
        ToggledOn = 1 << 2,
        Cooldown = 1 << 3,
        AutoTrigger = 1 << 4,
        CanAutoTrigger = 1 << 5,
        RequiresTarget = 1 << 6,
        CanTargetHostileUnits = 1 << 7,
        CanTargetFriendlyUnits = 1 << 8,
    }
}
