using ObjectiveStrategy.GameGeneration;
using Xunit;

namespace GameModels.Tests
{
    public class GameDefinitionSerialization
    {
        [Fact]
        public void Test1()
        {
            var serialized = new GameDefinitionFactory()
                .GenerateGame()
                .ToJson();

            Assert.NotNull(serialized);
            Assert.NotEmpty(serialized);
        }
    }
}
