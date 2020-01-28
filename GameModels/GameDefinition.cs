namespace GameModels
{
    public class GameDefinition
    {
        public GameDefinition(TechTree techTree, Battlefield battlefield)
        {
            TechTree = techTree;
            Battlefield = battlefield;
        }

        public TechTree TechTree { get; }
        
        public Battlefield Battlefield { get; }
    }
}
