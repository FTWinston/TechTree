namespace GameModels.Definitions
{
    public interface IUnitType : IEntityType
    {
        UnitMobility Mobility { get; }

        int MoveRange { get; }

        uint BuiltBy { get; }

        UnitFlags Flags { get; }
    }
}
