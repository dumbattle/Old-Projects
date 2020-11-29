using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class ConvTester : MonoBehaviour {
    public int batchSize = 32;

    Model m;
    Trainer t;
    Operation testOP;
    Channel channel;
    Tensor[] inputs, labels;

    void Start() {
        channel = Graph.GetChannel("Loss");
        channel.autoYRange = true;

        Placeholder inputPH = new Placeholder("Input", 25, 25, 3);


        Operation op =  new Convolution2D(10, af: ActivationFunction.Sigmoid).Build(inputPH);
        op = new Convolution2D(10, af: ActivationFunction.Sigmoid).Build(op);
        op = new GlobalAveragePooling(op);
        op = new FullyConnected(10, af: ActivationFunction.Sigmoid, bias: true).Build(op);
        testOP = op = new FullyConnected(1, af: ActivationFunction.Sigmoid, bias: true).Build(op);


        m = new Model(op);

        t = new Trainer(m, new Adam(), Loss.MSE);

        inputs = new Tensor[batchSize];
        labels = new Tensor[batchSize];

        for (int i = 0; i < batchSize; i++) {
            inputs[i] = new Tensor(() => Random.Range(-1f, 1f), inputPH.shape);
            labels[i] = new Tensor(() => Random.Range(0f, 1f), op.shape);
        }
    }
    private void Update() {
        var loss = t.Train(inputs, labels, batchSize);
        channel.Feed(loss[0]);
        print(loss);    
    }

}
