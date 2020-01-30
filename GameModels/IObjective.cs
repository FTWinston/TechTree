namespace GameModels
{
    public interface IObjective
    {
        bool IsSatisfied(Player player);

        int Value { get; }
    }
}
