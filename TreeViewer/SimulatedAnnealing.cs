using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeViewer
{
    public abstract class SimulatedAnnealing<T>
    {
        protected Random Random { get; private set; }
        public T Run(T state, int maxSteps, double targetEnergy)
        {
            double energy = DetermineEnergy(state);
            T bestState = state; double bestEnergy = energy;

            Random = new Random();
            double dMaxSteps = maxSteps;
            int step = 0;
            while (step < maxSteps && energy > targetEnergy)
            {
                double temperature = Temperature(step / dMaxSteps);
                T test = SelectNeighbour(state);
                double testEnergy = DetermineEnergy(test);

                if (AcceptanceProbability(energy, testEnergy, temperature) > Random.NextDouble())
                {
                    state = test; energy = testEnergy;
                }
                if (testEnergy < bestEnergy)
                {
                    bestState = test; bestEnergy = testEnergy;
                }
                step++;
            }
            return bestState;
        }

        public abstract T SelectNeighbour(T state);

        public abstract double DetermineEnergy(T state);

        public virtual double AcceptanceProbability(double before, double after, double temperature)
        {
            if (after < before)
                return 1;

            return Math.Exp(-(after - before) / temperature);
        }

        public virtual double Temperature(double completionFraction)
        {
            return (1.0 - completionFraction) * 5000.0;
        }
    }
}
