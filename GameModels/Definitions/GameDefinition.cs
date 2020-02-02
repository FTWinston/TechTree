namespace GameModels.Definitions
{
    public class GameDefinition
    {
        public GameDefinition(int complexity, int seed, TechTree techTree, Battlefield battlefield)
        {
            Complexity = complexity;
            Seed = seed;
            TechTree = techTree;
            Battlefield = battlefield;
        }

        public int Complexity { get; }

        public int Seed { get; }

        public int TurnLimit { get; }

        public TechTree TechTree { get; }
        
        public Battlefield Battlefield { get; }
    }
}
