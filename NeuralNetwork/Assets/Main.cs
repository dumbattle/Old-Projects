using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using DumbML;

public class Main : MonoBehaviour {
        static ProfilerMarker profile = new ProfilerMarker("Main.TrainStep");
    public int count = 32;
    public int batchSize = 32;
    public float delay = 0;
    public float loss = 0;
    Stopwatch sw = new Stopwatch();

    TrainableModel nn;
    Gradients grad;
    Optimizer opt;
    Placeholder input, target;

    Tensor[] inputs, labels;
    int inputSize = 28 * 28;
    int intermediate = 100;
    int outputSize = 10;

    Channel c;
    void Start() {
        c = Graph.GetChannel("loss");
        c.autoYRange = true;
        nn = CreateModel();

        inputs = new Tensor[count];
        labels= new Tensor[count];

        for (int i = 0; i < count; i++) {
            inputs[i] = new Tensor(() => Random.Range(-1f, 1f), 20);
            labels[i] = new Tensor(() => Random.Range(0f, 1f), nn.outputShape);
        }


    }


    TrainableModel CreateModel() {
        Operation op = new InputLayer(20).Build();

        op = new FullyConnected(100, ActivationFunction.LeakyRelu, bias: true).Build(op);
        op = new FullyConnected(100, ActivationFunction.LeakyRelu, bias: true).Build(op);

        var a = new FullyConnected(100, ActivationFunction.LeakyRelu, bias: true).Build(op);
        a = new FullyConnected(2, bias: true).Build(a);

        var v = new FullyConnected(100, ActivationFunction.LeakyRelu, bias: true).Build(op);
        v = new FullyConnected(1, bias: true).Build(v);

        v = v - new Sum(a) / 2;
        v = new Append(v, v);

        op = a + v;
        op.Optimize();
        var r = new TrainableModel(op);

        r.SetOptimizer(new SGD(), Loss.MSE);
        return r;


        var nn = new NeuralNetwork(10);
        nn.Add(new FullyConnected(100, af: ActivationFunction.LeakyRelu));
        nn.Add(new FullyConnected(10, af: ActivationFunction.LeakyRelu));
        nn.Build();
        nn.SetOptimizer(new SGD (), Loss.MSE);
        return nn;

    }

    void Update() {
        sw.Start();
        c.Feed(loss = nn.Train(inputs, labels, batchSize)[0]);
        sw.Stop();

        if (delay > 0) {
            delay *= .99f;
            delay += .01f * sw.ElapsedMilliseconds;
        }
        else {
            delay =  sw.ElapsedMilliseconds;
        }
        sw.Reset();
    }
}
