using UnityEngine;
public enum Orientation {
    vertical,
    horizontal
}

public class SomeThingWentWrongException : System.Exception {
    public SomeThingWentWrongException() { }
    public SomeThingWentWrongException(string msg) : base(msg) { }
}

public static class Extensions {
    public static Vector3 SetZ(this Vector3 v, float z) {
        v.z = z;
        return v;
    }
}