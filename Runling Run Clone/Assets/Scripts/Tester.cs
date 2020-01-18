using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour {
    [Header("Map")]
    public GameObject Map;
    public Vector2 mapSize;
    public UnitController unit;

    void Start() {
        Map.transform.localScale = mapSize;
    }

}