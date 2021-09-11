using System;

namespace DumbML {
    public static class RNG {
        static Random rng = new Random();

        public static float Normal() { return Normal(0, 1); }
        public static float Normal(float mean, float stdDev) {
            double u1 = 1.0 - rng.NextDouble(); // uniform(0,1] random doubles
            double u2 = 1.0 - rng.NextDouble();
            double randStdNormal =
                Math.Sqrt(-2.0 * Math.Log(u1)) *
                Math.Sin(2.0 * Math.PI * u2); // random normal(0,1)

            double result = mean + stdDev * randStdNormal; // random normal(mean,stdDev^2)
            return (float)result;
        }
    }
}
