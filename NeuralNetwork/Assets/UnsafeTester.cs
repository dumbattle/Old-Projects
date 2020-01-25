using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using DumbML;


public class UnsafeTester : MonoBehaviour {
    Trainer trainer;
    NeuralNetwork nn;
    Tensor[] inputs, labels;

    void Start() {
        nn = new NeuralNetwork(100);
        nn.Add(new FullyConnected(10, ActivationFunction.Sigmoid,false));
        nn.Add(new FullyConnected(1, ActivationFunction.Tanh,false));
        nn.Build();

        trainer = new Trainer(nn, new SGD(), Loss.MSE);

        Tensor t = Tensor.Random(100);
        inputs = new[] { t, t ,t};
        labels = new[] {
            new Tensor(() => 0, 1),
            new Tensor(() => 1, 1),
            new Tensor(() => -1, 1)
        };
    }
    private void Update() {
        print(trainer.Train(inputs, labels,3));
        print(nn.Compute(inputs[0]));
    }


}