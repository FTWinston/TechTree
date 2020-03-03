using ObjectiveStrategy.GameGeneration;
using System.Text.Json;
using Xunit;

namespace GameModels.Tests
{
    public class GameDefinitionSerialization
    {
        [Fact]
        public void Test1()
        {
            var definitionFactory = new GameDefinitionFactory();
            var gameDef = definitionFactory.GenerateGame();
            string serialized = JsonSerializer.Serialize(gameDef);

            Assert.NotNull(serialized);
            Assert.NotEmpty(serialized);
        }
    }
}
