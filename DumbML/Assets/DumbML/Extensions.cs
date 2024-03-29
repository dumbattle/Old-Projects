﻿using System.Collections.Generic;
using System.Text;

namespace DumbML {
    public static class Extensions {
        public static float Abs(this float a) {
            return a > 0 ? a : -a;
        }
        public static float Sqr(this float a) {
            return a * a;
        }
        public static float Sqrt(this float a) {
            return (float)System.Math.Sqrt(a);
        }
        public static float Sign(this float a) {
            return a >= 0 ? 1 : -1;
        }
        public static float Clamp(this float a, float min, float max) {
            if (a < min) {
                return min;
            }
            if (a > max) {
                return max;
            }

            return a;
        }
        public static float Lerp(this float a, float b, float t, bool bound = true) {
            if (bound) {
                t = t.Clamp(0, 1);
            }
            float result = a * (1 - t) + b * t;
            return result;
        }


        public static int Clamp(this int a, int min, int max) {
            if (a < min) {
                return min;
            }
            if (a > max) {
                return max;
            }

            return a;
        }

        public static T Last<T>(this T[] a) {
            return a[a.Length - 1];
        }
        public static T Last<T>(this List<T> a) {
            return a[a.Count - 1];
        }


        public static string TOSTRING(this int[] t) {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");


            sb.Append(t[0]);
            for (int i = 1; i < t.Length; i++) {

                sb.Append(", " + t[i]);
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}
