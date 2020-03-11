using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class SerializationTester : MonoBehaviour {
    public Model model;
    public MLModelAsset modelAsset;

    void Start() {
        Tensor input = Tensor.FromArray(new float[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        model = CreateNewModel();

        modelAsset.Save(model);
        model = modelAsset.LoadModel();
    }

    Model CreateNewModel() {
        Operation op = new Placeholder(10);
        var a = op = new FullyConnected(10, ActivationFunction.LeakyRelu).Build(op);
        op = new FullyConnected(10, ActivationFunction.Sigmoid).Build(op);

        return new Model(op + op);
    }

}