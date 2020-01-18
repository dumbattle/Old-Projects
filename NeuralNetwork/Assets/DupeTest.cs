using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;


public class DupeTest : MonoBehaviour {
    Gradients g;
    Optimizer o;
    Operation op;
    void Start() {
        Operation a = new Variable(new Tensor(1),"a");
        Operation b = new Variable(new Tensor(1),"b");
        op = a + b;
        op += 1;
        op += 1;
        op += 1;
        op += 1;
        op += 1;
        op += 1;
        op += 1;
        op += 1;
        op += 1;
        op += 1;
        op += 1;
        op += 1;
        //print(op.GetOperations().Count);


        op += op;
        op *= op;
        print(op.GetOperations().Count);
        //print(op.eval());

        g = op.GetNewGradients();
        o = new SGD(g);

      
    }
    private void Update() {
        for (int i = 0; i < 10; i++) {
            print(op.Eval());
            g.Reset();
            op.Backwards(g);
            o.Update();
        }
    }
}
