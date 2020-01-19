﻿using System.Collections;
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

        print($"Model output: {model.Compute(input)}");
        //foreach (var o in model.forward.GetOperations()) {
        //    print(o.GetType());
        //}


        var copy = model.Copy();

        print($"Copy Output: {copy.Compute(input)}");

        //foreach (var o in copy.forward.GetOperations()) {
        //    print(o.GetType());
        //}

    }

    Model CreateNewModel() {
        Operation op = new Placeholder(10);
        var a = op = new FullyConnected(10, ActivationFunction.LeakyRelu).Build(op);
        op = new FullyConnected(10, ActivationFunction.Sigmoid).Build(op);

        return new Model(op + a);
    }

}