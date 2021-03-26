using System;
using System.Collections.Generic;
using UnityEngine;
using DumbML;


public class WeightTester : MonoBehaviour {
    void Start() {

        Tensor input = new Tensor(() => UnityEngine.Random.Range(-1f, 1f), 3, 3, 2);
        Tensor filters = new Tensor(() => 1, 3, 3, 2, 1);

        Tensor output = new Tensor(1, 1, 1);
        Operation op = new InputLayer(input.Shape).Build();
        op = new FullConv2D(op, filters, (1, 1), false);

        Model m = new Model(op);
        Trainer t = new Trainer(m, new SGD(), Loss.MSE);

        print(m.Compute(input));

        for (int i = 0; i < 10; i++) {

            t.Train(new[] { input }, new[] { output }, 1);
            print(m.Compute(input));
        }

        //print(input);

    }
}
