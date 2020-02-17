using ObjectiveStrategy.GameModels.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectiveStrategy.GameModels.Definitions
{
    public interface IEffect<T>
    {
        int Duration { get; }
        string Name { get; }
        string GetDescription();

        void BeforeFirstTick(T target);
        void Tick(T target);
        void AfterLastTick(T target);
    }

    public interface IStatusEffect : IEffect<Entity> { }

    public interface ICellEffect : IEffect<Cell> { }
}
