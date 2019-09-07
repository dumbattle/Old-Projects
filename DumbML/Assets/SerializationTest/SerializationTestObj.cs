using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializationTestObj : MonoBehaviour {
    public BaseClass[] list;
    public Vector2 v2;

    private void Start() {
        Initialize();
    }


    void Initialize () {
        list = new BaseClass[] {
            new BaseClass(), new DerivedA(), new DerivedC(), new DerivedC()
        };
    }
}



[System.Serializable]
public class BaseClass {
    public int BaseInt;
}
[System.Serializable]
public class DerivedA : BaseClass{
    public string DerivedStringA;
}
[System.Serializable]
public class DerivedB : BaseClass {
    public float DerivedFloatB;
}

public class DerivedC : BaseClass {
    public string StringC;
    public float FloatC;
}

