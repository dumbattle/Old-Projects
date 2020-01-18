using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class ConvTester : MonoBehaviour {
    Operation op;
    Optimizer opt;
    Channel channel;
    void Start() {
        channel = Graph.GetChannel("Loss");
        channel.autoYRange = true;

        Placeholder inputPH = new Placeholder(10, 10, 3);
        Variable weightD = new Tensor(() => Random.value, 3, 3,3);
        Variable weightP = new Tensor(() => Random.value, 3,2);

        op = new Conv2DDepth(inputPH, weightD);
        op = new Conv2DPoint(op, weightP);
        op = new Sigmoid(op);
        op = new FlattenOp(op);

        //Variable bias = new Tensor(op.shape);

        //op += bias;

        Placeholder targetPH = new Placeholder(op.shape);

        op = Loss.MSE.Compute(op, targetPH);

        Tensor i = Tensor.Random(inputPH.shape);
        Tensor o = Tensor.Random(targetPH.shape);

        inputPH.SetVal(i);
        targetPH.SetVal(o);

        opt = new Adam(op.GetNewGradients());



    }
    private void Update() {
        var loss = op.Eval();
        print(loss);
        channel.Feed(loss[0]);
        op.Backwards(opt);

        opt.Update();
        opt.ZeroGrad();
    }

}
