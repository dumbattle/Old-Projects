using System.Collections.Generic;
using UnityEngine;

namespace DumbML {
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
            if (s == 0) {
                s = 1;
            }
            int count = operands[0].value.Length / s;

            for (int i = 0; i < count; i++) {
                int start = s * i;
                double sum = 0;
                for (int k = 0; k < s; k++) {
                    int ind = start + k;
                    sum += rv[ind];
                }

                for (int k = 0; k < s; k++) {
                    int ind = start + k;
                    rv[ind] = (float)(rv[ind]/sum);
                }
            }
            return c.result;
        }
        protected override Tensor[] _BackwardsPass(Tensor e) {
            var err = c.error;


            var rv = c.result.value;

            int s = operands[0].Shape[operands[0].Shape.Length - 1];
            if (s == 0) {
                s = 1;
            }
            int count = operands[0].value.Length / s;


            for (int i = 0; i < count; i++) {
                int start = s * i;
                for (int k = 0; k < s; k++) {
                    int indK = start + k;

                    double t = 0; // float gives rounding errors

                    for (int j = 0; j < s; j++) {
                        int indJ = start + j;
                        float derivative = j != k ? -rv[indJ] * rv[indK] : rv[indK] * (1 - rv[indJ]);
                        t += e.value[indJ] * derivative;
                    }
                    err.value[indK] = (float)t;
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

}