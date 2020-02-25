namespace ObjectiveStrategy.GameModels.Definitions
{
    public class GameDefinition
    {
        public GameDefinition(int complexity, int seed, TechTree techTree, Battlefield battlefield, Objective[] objectives)
        {
            Complexity = complexity;
            Seed = seed;
            TechTree = techTree;
            Battlefield = battlefield;
            Objectives = objectives;
        }

        public int Complexity { get; }

        public int Seed { get; }

        public int TurnLimit { get; }

        public TechTree TechTree { get; }
        
        public Battlefield Battlefield { get; }

        public Objective[] Objectives { get; }
    }
}
