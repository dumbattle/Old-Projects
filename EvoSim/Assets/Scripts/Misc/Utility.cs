using UnityEngine;
using System;

public static class Utility {
    static System.Random rng = new System.Random();

    public static Vector2Int RandomVector2Int(Vector2Int maxeExclusive) {
        return new Vector2Int(UnityEngine.Random.Range(0, maxeExclusive.x), UnityEngine.Random.Range(0, maxeExclusive.y));
    }

    public static float Gaussian(float mean, float stdDev) {
        return mean + Gaussian(stdDev);
    }
    public static float Gaussian(float stdDev) {
        double u1 = 1.0 - rng.NextDouble();
        double u2 = 1.0 - rng.NextDouble();
        float randStdNormal = (float)(Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2));
        float randNormal = stdDev * randStdNormal;
        return randNormal;
    }
}
