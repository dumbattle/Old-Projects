public static class Extentions { 
    public static T Random<T>(this T[] a) {
        return a[UnityEngine.Random.Range(0, a.Length)];
    }
}