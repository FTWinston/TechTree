namespace ObjectiveStrategy.GameModels.Definitions
{
    public class UnitType : EntityType, IUnitType
    {
        public UnitType(IUnitType copyFrom)
            : base(copyFrom)
        {
            Mobility = copyFrom.Mobility;
            MoveRange = copyFrom.MoveRange;
            BuiltBy = copyFrom.BuiltBy;
            Flags = copyFrom.Flags;
        }

        public UnitMobility Mobility { get; internal set; }

        public int MoveRange { get; internal set; }

        public uint BuiltBy { get; internal set; }

        public UnitFlags Flags { get; internal set; }
    }
}
