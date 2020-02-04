using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;


public class WeightTester : MonoBehaviour {

    void Start() {
        Variable a = Tensor.FromArray(new[] { 1f,2f,1f });

        Operation op = new Exp(a).Softmax();
        var sum = new Sum(op);
        print(op.Eval());
        print(sum.Eval());
        Gradients g = new Gradients(a);
        op.Backwards(g);

        print(g[a]);
    }


}
