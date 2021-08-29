using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;


public class RandomTestStuffMain : MonoBehaviour {


    void Start() {
        Operation k = Tensor.FromArray(new float[,] { { 10, 0, 0 }, { 0, 10, 0 }, { 0, 0, 10 },{ 0, 0, 10 } });
        Operation v = Tensor.FromArray(new float[,] { { 1, 0 }, { 10, 0 }, { 100, 5 }, { 1000, 6 } });

        Operation q1 = Tensor.FromArray(new float[,] { { 0, 10, 0} });
        var (op, a) = Attention.ScaledDotProduct(k, v, q1);
        op.Eval();
        print(a.value);
        print(op.value);

        Operation q2 = Tensor.FromArray(new float[,] { { 0, 0, 10 } });
        (op, a) = Attention.ScaledDotProduct(k, v, q2);
        op.Eval();
        print(a.value);
        print(op.value);

        Operation q3 = Tensor.FromArray(new float[,] { { 10, 10, 0 } });
        (op, a) = Attention.ScaledDotProduct(k, v, q3);
        op.Eval();
        print(a.value);
        print(op.value);

        Operation q = new Append(q1, q2, q3);
        (op, a) = Attention.ScaledDotProduct(k, v, q);
        op.Eval();
        print(a.value);
        print(op.value);


    }
    private void Update() {
    }


}

