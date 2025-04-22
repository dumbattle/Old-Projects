using System.Collections.Generic;
using UnityEngine;

namespace LPE {
    public static partial class LPEExtensions {
        public static bool IsInRange<T>(this T[] arr, int i) {
            return i >= 0 && i < arr.Length;
        }
        public static bool IsInRange<T>(this List<T> arr, int i) {
            return i >= 0 && i < arr.Count;
        }

        public static float Max(this float f, float maxValue) {
            return f > maxValue ? maxValue : f;
        }
        public static int Max(this int f, int maxValue) {
            return f > maxValue ? maxValue : f;
        }
        public static int Min(this int f, int maxValue) {
            return f < maxValue ? maxValue : f;
        }

        public static Vector3 SetZ(this Vector2 v, float z) {
            return new Vector3(v.x, v.y, z);
        }
        public static Vector2 SetY(this Vector2 v, float y) {
            return new Vector2(v.x, y);
        }
        public static Vector3 SetZ(this Vector3 v, float z) {
            return new Vector3(v.x, v.y, z);
        }
        public static Vector3 XZY(this Vector3 v) {
            return new Vector3(v.x, v.z, v.y);
        }
        public static Vector3 XZY(this Vector2 v) {
            return new Vector3(v.x, 0, v.y);
        }

        public static float PathLength(this List<Vector2> p) {
            float result = 0;

            Vector2 a = p[0];
            for (int i = 1; i < p.Count; i++) {
                Vector2 b = p[i];

                result += (a - b).magnitude;    
            }
            return result;
        }
    }


}