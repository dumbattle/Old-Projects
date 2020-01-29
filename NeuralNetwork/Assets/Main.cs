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

    Trainer trainer;
    Model nn;

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
            labels[i] = new Tensor(() => Random.Range(.5f, 1f), nn.outputShape);
        }


    }


    Model CreateModel() {
        Operation h1, h2, h3, h4;
        Operation op = new InputLayer(20).Build();

        h1 = op = new FullyConnected(100, ActivationFunction.LeakyRelu, bias: false).Build(op);
        h2 = op = new FullyConnected(100, ActivationFunction.LeakyRelu, bias: false).Build(op);

        var a = h3 = new FullyConnected(100, ActivationFunction.LeakyRelu, bias: false).Build(op);
        a = new FullyConnected(2, bias: false).Build(a);

        var v = h4 = new FullyConnected(100, ActivationFunction.Sigmoid).Build(op);
        v = new FullyConnected(1, bias: false).Build(v);

        v = v - new Sum(a) / 2;
        v = new Append(v, v);

        op = a + v;

        op.Optimize();

        var m = new Model(op);
        trainer = new Trainer(m, new SGD(.01f), new CustomLoss(h1, h2, h3, h4));
        return m;



    }
    class CustomLoss : Loss {
        Operation[] hidden;
        public CustomLoss(params Operation[] ops) {
            hidden = ops;
        }
        public override Operation Compute(Operation op, Placeholder labels) {
            var l = MSE.Compute(op, labels);

            foreach (var h in hidden) {
                int size = 1;
                foreach (var d in h.shape) {
                    size *= d;
                }


                var s = new Sum(new Square(h)) / (size * 100);
                l += s;
            }
            return l;
        }
    }

    void Update() {
        sw.Start();
        c.Feed(loss = trainer.Train(inputs, labels, batchSize)[0]);
        sw.Stop();

        //print("--------------------------------------");
        //print(nn.Compute(inputs[0]));
        //print(nn.Compute(inputs[1]));
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
