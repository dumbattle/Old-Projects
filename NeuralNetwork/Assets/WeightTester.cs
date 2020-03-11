using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;


public class WeightTester : MonoBehaviour {
    int inputSize = 10;
    RingBuffer<RLExperience> buffer = new RingBuffer<RLExperience>(1000);

    Model m;
    Tensor state;
    Operation logProb, preSoft;
    Placeholder actionMask;
    Optimizer o;

    int a0 = 0, a1 = 0, a2 = 0;

    void Start() {
        m = GetModel();
        state = new Tensor(() => Random.value, inputSize);
    }

    void Update() {
        Train();
        var exp = GetAction();

        switch (exp.action) {
            case 0:
                exp.reward = 1;
                a0++;
                break;
            case 1:
                exp.reward = -2;
                a1++;
                break;
            case 2:
                exp.reward = -2;
                a2++;
                break;
        }

        buffer.Add(exp);
        print("============");
        print(preSoft.value);
        print(exp.output);

    }

    void Train() {
        if (buffer.Count < 32) {
            return;
        }

        var sample = buffer.GetRandom(32);

        var inputs = new Tensor[32];
        var masks = new Tensor[32];


        for (int i = 0; i < 32; i++) {
            inputs[i] = sample[i].state;
            masks[i] = new Tensor(3);
            //masks[i][sample[i].action] = -sample[i].reward;

            var r = -sample[i].reward;


            if (r < 0) {
                //positive reward - encourage action
               masks[i] = new Tensor(3);
                masks[i][sample[i].action] = r;
            }
            else {
                //negative reward - encourage other actions
                //                - log(prob) is assymmetric, this helps stay in line with symmetry
                //r /= sample[i].output.Size - 1;
                masks[i] = new Tensor(() => -r, 3);
                masks[i][sample[i].action] = 0;
            }
        }

        for (int i = 0; i < 32; i++) {
            //m.SetInputs(inputs[i]);
            actionMask.SetVal(masks[i]);
            logProb.Eval();
            logProb.Backwards(o);
        }

        o.Update();
        o.ZeroGrad();
    }

    RLExperience GetAction () {
        var output = m.Compute(state);
        int action = output.Sample();

        return new RLExperience(state, output, action);
    }
    Model GetModel () {

        Operation op = new InputLayer(inputSize).Build();
        op = new FullyConnected(10, ActivationFunction.Sigmoid).Build(op);
        preSoft = op = new FullyConnected(3, ActivationFunction.Sigmoid).Build(op);

        op = op.Softmax();

        actionMask = new Placeholder(op.shape);
        logProb = new Log(op) * actionMask;

        var g = new Gradients(op.GetOperations<Variable>());
        o = new Adam(g);

        return new Model(op);
    }

}
