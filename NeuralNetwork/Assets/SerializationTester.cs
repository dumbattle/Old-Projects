using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class SerializationTester : MonoBehaviour {
    public Model model;

    void Start() {

        model = CreateNewModel();
    }

    Model CreateNewModel() {
        Operation op = new Placeholder(10);
        var a = op = new FullyConnected(10, ActivationFunction.LeakyRelu).Build(op);
        op = new FullyConnected(10, ActivationFunction.Sigmoid).Build(op);

        return new Model(op + a);
    }

}
