using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class PrioritizedBufferTest : MonoBehaviour {
    PrioritizedBuffer<(string, float)> b;

    // Start is called before the first frame update
    void Start() {
        b = new PrioritizedBuffer<(string, float)>(5, (f) => Mathf.Abs(f.Item2) + .00001f);

        b.Add(("Cat", 40));
        b.Add(("Dog", 50));
        b.Add(("Parrot", 4));
        b.Add(("Goldfish", 30));
        b.Add(("Hamster", 15));
        //print(b.maxSize);
        //b.Add(4);
        //print(b.TreeSize);
        //b.Add(2);
        //for (int i = 0; i < b.TreeSize; i++) {
        //    print(b.tree[i]);
        //}
    }

    // Update is called once per frame
    void Update() {
        print(b.GetRandom().Item1);
    }
}

