using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;
using System.Diagnostics;

public class TestScript : MonoBehaviour {
    public int dataSize = 100;
    public float loss;
    public float delay;
    

    NeuralNetwork nn;
    Optimizer o;
    Stopwatch sw = new Stopwatch();


    Tensor[] data, labels;

    void Start() {
        nn = new NeuralNetwork(28,28);
        nn.Add(new Flatten());
        nn.Add(new FullyConnected(128, () => Random.value - .5f, ActivationFunction.Relu));
        nn.Add(new FullyConnected(10, () => Random.value - .5f));
        nn.Add(new SoftMax());

        nn.Build();


        o = new Adam(nn, lr: .001f);
        SetData();


    }
     void Update() {

        sw.Start();
        loss = o.Train(data, labels, batchSize: 32);
        sw.Stop();


        delay *= .9f;
        delay += .1f * sw.ElapsedMilliseconds / 1000;
        sw.Reset();


    }

    void SetData() {
        data = new Tensor[dataSize];
        labels = new Tensor[dataSize];


        for (int i = 0; i < dataSize; i++) {
            data[i] = new Tensor(() => Random.value, 28, 28);
            labels[i] = new Tensor(10);
            labels[i][Random.Range(0, 10)] = 1;
        }
    }
}
