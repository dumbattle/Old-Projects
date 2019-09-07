using System.Collections.Generic;

public static class Extensions {
    public static T Last<T>(this T[] a, int i = 1) {
        if (a.Length == 0) {
            return default(T);
        }

        return a[a.Length - 1];
    }
    public static T Random<T>(this T[] a) {
        if (a.Length == 0) {
            return default(T);
        }
        return a[UnityEngine.Random.Range(0, a.Length)];
    }


    public static T Random<T>(this T[,] a) {
        if (a.Length == 0) {
            return default(T);
        }
        return a[UnityEngine.Random.Range(0, a.GetLength(0)), UnityEngine.Random.Range(0, a.GetLength(1))];
    }

    public static T Random<T>(this List<T> a) {
        if (a.Count == 0) {
            return default(T);
        }
        return a[UnityEngine.Random.Range(0, a.Count)];
    }

    public static bool IsInRange<T>(this T[,] a, int x, int y) {
        return x >= 0 && y >= 0 && x < a.GetLength(0) && y < a.GetLength(1);
    }

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
    public static int Clamp(this int a, int min, int max) {
        if (a < min) {
            return min;
        }
        if (a > max) {
            return max;
        }

        return a;
    }
}
