using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;


public class RandomTestStuffMain : MonoBehaviour {


    void Start() {
        Operation a = new Tensor(() => Random.Range(0, 6), 5);
        Operation b = new Tensor(() => Random.Range(0, 6), 5);

        Operation sma = a.Softmax();
        Operation smb = b.Softmax();

        Operation ab = new Stack1D(a, b);
        Operation smab = new Softmax(ab);

        sma.Eval();
        smb.Eval();
        smab.Eval();


        print($"a: {a.value}");
        print($"b: {b.value}");
        print($"ab: {ab.value}");

        print($"sma: {sma.value}");
        print($"smb: {smb.value}");
        print($"smab: {smab.value}");

        //Operation sm2 = new Softmax(a);


        //sm.Eval();
        //sm2.Eval();

        //Gradients g1 = new Gradients(a);
        //sm.Backwards(g1);
        //Gradients g2 = new Gradients(a);
        //sm2.Backwards(g2);

        //print(a.value);
        //print(sm.value);
        //print(sm2.value);
        //print(sm.value.CompareData(sm2.value));
        //print(g1[a]);
        //print(g2[a]);
    }
    private void Update() {
    }
}



public class Softmax : Operation {
    Dictionary<List<int>, Cache> dict = new Dictionary<List<int>, Cache>(new ShapeComparer());
    List<int> shapeList = new List<int>(); // used to search dict
    Cache c;  // store during forward pass so we don't have to retrieve it during backwards pass
    Tensor[] errArr = new Tensor[1];


    public Softmax(Operation op) : base(op.shape, op) {

    }


    protected override Tensor _Compute(Tensor[] operands) {
        // get shape
        shapeList.Clear();
        foreach (var i in operands[0].Shape) {
            shapeList.Add(i);
        }

        // get cache
        c = null;
        if (dict.ContainsKey(shapeList)) {
            c = dict[shapeList];
        }
        else {
            // create new entry
            c = new Cache();
            c.result = new Tensor(shapeList);
            c.error = new Tensor(shapeList);

            dict.Add(new List<int>(shapeList), c);
        }


        // compute softmax
        var rv = c.result.value;
        var iv = operands[0].value;

        
        for (int i = 0; i < iv.Length; i++) {
            rv[i] = Mathf.Exp(iv[i]);
        }

        int s = operands[0].Shape[operands[0].Shape.Length - 1];
        int count = operands[0].value.Length / s;

        // can use threading
        for (int i = 0; i < count; i++) {
            int start = s * i;
            float sum = 0;
            for (int k = 0; k < s; k++) {
                int ind = start + k;
                sum += rv[ind];
            }

            for (int k = 0; k < s; k++) {
                int ind = start + k;
                rv[ind] /= sum;
            }
        }
        return c.result;
    }
    protected override Tensor[] _BackwardsPass(Tensor e) {
        var err = c.error;


        var rv = c.result.value;

        int s = operands[0].Shape[operands[0].Shape.Length - 1];
        int count = operands[0].value.Length / s;


        for (int i = 0; i < count; i++) {
            int start = s * i;
            for (int k = 0; k < s; k++) {
                int indK = start + k;

                double t = 0; // float gives rounding errors

                for (int j = 0; j < s; j++) {
                    int indJ = start + j;
                    float derivative = j != k ? -rv[indJ] * rv[indK] : rv[indK] * (1 - rv[indJ]);
                    t -= e.value[indJ] * derivative;
                }
                err[indK] = (float)t;
            }

        }
        errArr[0] = err;
        return errArr;
    }
    public override Operation Copy(Dictionary<Operation, Operation> track) {
        return new Softmax(inner[0]._Copy(track));
    }

    class Cache {
        public Tensor result;
        public Tensor error;

    }
}
