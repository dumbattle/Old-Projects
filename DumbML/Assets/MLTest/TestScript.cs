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
        nn = new NeuralNetwork(1000);
        nn.Add(new FullyConnected(1000, () => Random.value - .5f, ActivationFunction.Sigmoid));
        nn.Add(new FullyConnected(10, () => Random.value - .5f, ActivationFunction.Sigmoid));

        nn.Build();


        o = new SGD(nn, lr: .001f);
        SetData();
        //sw.Start();
        //for (int i = 0; i < 1; i++) {
        //    o.Train(data, labels);
        //}
        //sw.Stop();
        //print(sw.ElapsedMilliseconds);

    }
     void Update() {
        sw.Start();
        loss = o.Train(data, labels, batchSize: 1);
        sw.Stop();


        delay *= .9f;
        delay += .1f * sw.ElapsedMilliseconds;
        sw.Reset();


    }

    void SetData() {
        data = new Tensor[dataSize];
        labels = new Tensor[dataSize];


        for (int i = 0; i < dataSize; i++) {
            data[i] = new Tensor(() => Random.value, 1000);
            labels[i] = new Tensor(10);
            labels[i][Random.Range(0, 10)] = 1;
        }
    }
}
