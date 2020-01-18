using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class SerializationTester : MonoBehaviour {
    public Model model;

    void Start() {
        Tensor input = new Tensor(10);
        input[0] = 1;
        input[1] = 2;
        input[2] = 0;
        input[3] = -1;
        input[4] = -2;
        input[5] = 1;
        input[6] = 2;
        input[7] = 0;
        input[8] = -1;
        input[9] = -2;

        model = CreateNewModel();


        print(model.Compute(input));
    }

    Model CreateNewModel() {
        Variable w = new Tensor(() => Random.value, 10, 10);

        Operation op = new Placeholder(10);
        op = new MatrixMult(op, w);

        return new Model(op);
    }
}