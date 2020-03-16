using ObjectiveStrategy.GameGeneration;
using ObjectiveStrategy.GameModels.Definitions;
using Xunit;

namespace GameModels.Tests
{
    public class GameDefinitionSerialization
    {
        [Fact]
        public void CanSerialize()
        {
            var serialized = new GameDefinitionFactory()
                .GenerateGame()
                .ToJson();

            Assert.NotNull(serialized);
            Assert.NotEmpty(serialized);
        }

        [Fact]
        public void CanDeserialize()
        {
            var serialized = new GameDefinitionFactory()
                .GenerateGame()
                .ToJson();

            var gameDefinition = GameDefinition.FromJson(serialized);
            Assert.NotNull(gameDefinition);
            Assert.NotNull(gameDefinition.TechTree);
            Assert.NotNull(gameDefinition.Battlefield);
            Assert.NotNull(gameDefinition.Objectives);
        }
    }
}
