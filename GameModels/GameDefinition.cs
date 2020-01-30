namespace GameModels
{
    public class GameDefinition
    {
        public GameDefinition(int seed, int complexity, TechTree techTree, Battlefield battlefield)
        {
            Seed = seed;
            Complexity = complexity;
            TechTree = techTree;
            Battlefield = battlefield;
        }

        public int Seed { get; }

        public int Complexity { get; }

        public TechTree TechTree { get; }
        
        public Battlefield Battlefield { get; }
    }
}
