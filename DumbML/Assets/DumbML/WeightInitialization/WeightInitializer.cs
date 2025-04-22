using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbML {
    public class WeightInitializer {
        public static WeightInitializer Default = new Normal(-1,1);
        protected static Random rng = new Random();
        Func<float> function;

        private WeightInitializer() { }
        public WeightInitializer(Func<float> function) {
            this.function = function;
        }

        public virtual float Next() {
            return function();
        }
        public virtual Func<float> ToFunc() {
            return () => Next();
        }
        public static implicit operator Func<float> (WeightInitializer wi) {
            return wi.ToFunc();
        }


        public class Uniform : WeightInitializer {
            float range;
            float min;
            public Uniform(float min, float max) {
                range = max - min;
                this.min = min;
            }

            public override float Next() {
                return (float)rng.NextDouble() * range + min;
            }
        }
        public class Normal : WeightInitializer {
            float stdDev;
            float mean;
            public Normal(float min, float max) {
                mean = (max + min) / 2;
                stdDev = (max - min) / 4;
            }

            public override float Next() {
                double u1 = 1.0 - rng.NextDouble(); //uniform(0,1] random doubles
                double u2 = 1.0 - rng.NextDouble();
                double randStdNormal =
                    Math.Sqrt(-2.0 * Math.Log(u1)) *
                    Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)

                double result = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
                return (float)result;
            }
        }
    }


}