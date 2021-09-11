using System.Collections.Generic;
using UnityEngine;

namespace DumbML {

    public class Softmax : Operation {
        List<int> shapeList = new List<int>(); // used to search dict


        public Softmax(Operation op) : base(op.shape, op) {

        }

        protected override void _Compute(Tensor[] operands, TensorCache result) {

            // get shape
            shapeList.Clear();
            foreach (var i in operands[0].Shape) {
                shapeList.Add(i);
            }
            result.SetShape(shapeList);


            // compute softmax
            var rv = result.tensor.value;
            var iv = operands[0].value;

            int s = operands[0].Shape[operands[0].Shape.Length - 1];
            if (s == 0) {
                s = 1;
            }
            int count = iv.Length / s;

            // subtract max for stability
            for (int i = 0; i < count; i++) {
                int start = s * i;
                float max = iv[start];
                for (int k = 0; k < s; k++) {
                    int ind = start + k;
                    if (iv[ind] > max) {
                        max = iv[ind];
                    }
                }
                for (int k = 0; k < s; k++) {
                    int ind = start + k;

                    rv[ind] = Mathf.Exp(iv[ind] - max);
                }
            }

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
        }
        protected override void _BackwardsPass(Tensor e, Tensor[] result) {

            var rv = value.value;

            int s = inner[0].value.Shape[inner[0].value.Shape.Length - 1];
            if (s == 0) {
                s = 1;
            }
            int count = inner[0].value.value.Length / s;


            for (int i = 0; i < count; i++) {
                int start = s * i;
                for (int k = 0; k < s; k++) {
                    int indK = start + k;

                    double t = 0; // float gives rounding errors

                    for (int j = 0; j < s; j++) {
                        int indJ = start + j;
                        double derivative = j != k ? -rv[indJ] * (double)rv[indK] : rv[indK] * (1.0 - rv[indJ]);
                        t += e.value[indJ] * derivative;
                    }
                    result[0].value[indK] += (float)t;
                }
            }
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