using System;
using UnityEngine;
namespace TD {
    public class RNG {
        System.Random rng;
        public int seed;

        public RNG (int seed) {
            this.seed = seed;
            rng = new System.Random(seed);
        }
        public RNG () {
            seed = UnityEngine.Random.Range(-9999, 9999);
            rng = new System.Random(seed);
        }

        /// <summary>
        /// inclusive [min, max) exclusive
        /// </summary>
        /// <param name="min">inclusive</param>
        /// <param name="max">exclusive</param>
        /// <returns></returns>
        public int Int(int min, int max) {
            if (max <= min) {
                throw new ArgumentException($"Cannot get random int in range [{min}, {max})");
            }
            return rng.Next(min, max);
        }


        public HexCoord Hex(int radius) {
            int r = Int(0, (radius + 1) * radius * 3 + 1);
            if (r == 0) {
                return new HexCoord(0, 0);
            }
            r--;

            for (int i = 1; i <= radius; i++) {
                r -= i * 6;
                if (r < 0) {
                    r = i;
                    break;
                }
            }

            int arm = Int(0, 6);

            HexCoord result = HexCoord.Directions[arm] * r;
            int step =Int(0, r);
            return result + HexCoord.Directions[(arm + 2) % 6] * step;
        }
    }
}