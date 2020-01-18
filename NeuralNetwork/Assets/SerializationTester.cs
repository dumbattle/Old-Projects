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

        print("Model 1 Output:");
        print(model.Compute(input));

        var w = model.GetWeights();
        print("Model 1 Weights:");
        foreach (var t in w) {
            print(t);
        }
        print("");
        var m2 = CreateNewModel();

        print("Model 2 Output:");
        print(m2.Compute(input));

        var w2 = m2.GetWeights();
        print("Model 2 Weights:");
        foreach (var t in w2) {
            print(t);
        }
        print("");
        print("Copy model 1 weights to model 2");

        m2.SetWeights(w);

        print("Model 2 Output:");
        print(m2.Compute(input));

         w2 = m2.GetWeights();
        print("Model 2 Weights:");
        foreach (var t in w2) {
            print(t);
        }
    }

    Model CreateNewModel() {
        Operation op = new Placeholder(10);
        op = new FullyConnected(10).Build(op);

        return new Model(op);
    }
}