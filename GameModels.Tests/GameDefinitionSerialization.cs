using ObjectiveStrategy.GameGeneration;
using ObjectiveStrategy.GameModels.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // for test purposes only
            };
            //options.Converters.Add(new TechTreeConverter());
            //options.Converters.Add(new BattlefieldConverter());

            string serialized = JsonSerializer.Serialize(gameDef, options);

            Assert.NotNull(serialized);
            Assert.NotEmpty(serialized);
        }
    }
}
