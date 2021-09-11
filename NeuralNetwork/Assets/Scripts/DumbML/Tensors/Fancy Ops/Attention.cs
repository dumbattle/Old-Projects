using UnityEngine;


namespace DumbML {
    public static class Attention {
        public static (Operation output, Operation attentionWeights) ScaledDotProduct(Operation k, Operation v, Operation q) {
            var kShape = k.shape;

            var permutation = new int[kShape.Length];

            for (int i = 0; i < permutation.Length; i++) {
                permutation[i] = i;
            }

            permutation[permutation.Length - 1] = permutation.Length - 2;
            permutation[permutation.Length - 2] = permutation.Length - 1;

            Operation t = new Transpose(k, permutation);

            Operation qk = new MatrixMult(q, t);
            qk = new DivideByConst(qk, Mathf.Sqrt(k.shape[k.shape.Length - 1]));

            Operation attentionWeights = new Softmax(qk);
            Operation output = new MatrixMult(attentionWeights, v);

            return (output, attentionWeights);

        }

    }
}