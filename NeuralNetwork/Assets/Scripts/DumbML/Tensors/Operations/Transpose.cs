using System.Collections.Generic;
using UnityEngine;

namespace DumbML {

    public class Transpose : Operation {
        List<int> perm = new List<int>();

        List<int> index = new List<int>();
        int[] permutation;
        int[] offsets;


        public Transpose(Operation op, int[] permutation) : base(null, op) {
            if (permutation == null) {
                permutation = new int[op.shape.Length];

                for (int i = 0; i < permutation.Length; i++) {
                    permutation[i] = i;
                }
                permutation[permutation.Length - 1] = permutation.Length - 2;
                permutation[permutation.Length - 2] = permutation.Length - 1;
            }
            ValidatePermute(op.shape, permutation);

            this.permutation = permutation;
            offsets = new int[op.shape.Length];
            GetPermutation(op.shape, permutation);
            shape = perm.ToArray();
        }


        protected override void _Compute(Tensor[] operands, TensorCache result) {
            GetPermutation(operands[0].Shape, permutation);
            result.SetShape(perm);
  
            //init index
            index.Clear();
            for (int i = 0; i < perm.Count; i++) {
                index.Add(0);
            }
            //var result = c.result;

            for (int i = 0; i < operands[0].value.Length; i++) {
                var val = operands[0].value[i];

                int ind = 0;
                for (int j = index.Count - 1; j >= 0; j--) {
                    ind += index[j] * offsets[j];
                }

                result.tensor.value[ind] = val;

                // get index
                index[index.Count - 1]++;
                for (int j = index.Count - 1; j >= 1; j--) {
                    if (index[j] == operands[0].Shape[j]) {
                        var k = j - 1;
                        index[k]++;
                        index[j] = 0;
                    }
                    else {
                        break;
                    }
                }
            }
        }

        protected override void _BackwardsPass(Tensor e, Tensor[] result) {

            index.Clear();
            for (int i = 0; i < perm.Count; i++) {
                index.Add(0);
            }

            for (int i = 0; i < result[0].value.Length; i++) {
                int ind = 0;
                for (int j = index.Count - 1; j >= 0; j--) {
                    ind += index[j] * offsets[j];
                }


                result[0].value[i] += e.value[ind];


                // get index
                index[index.Count - 1]++;
                for (int j = index.Count - 1; j >= 1; j--) {
                    if (index[j] == result[0].Shape[j]) {
                        var k = j - 1;
                        index[k]++;
                        index[j] = 0;
                    }
                    else {
                        break;
                    }
                }
            }
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Transpose(inner[0]._Copy(track), permutation);
        }

        void GetPermutation(int[] src, int[] permute) {

            perm.Clear();

            for (int i = 0; i < src.Length; i++) {
                perm.Add(src[permute[i]]);
            }

            int[] multipliers = new int[perm.Count];

            int scale = 1;
            for (int i = multipliers.Length - 1; i >= 0; i--) {
                multipliers[i] = scale;
                scale *= perm[i];
            }

            for (int i = 0; i < offsets.Length; i++) {
                offsets[permutation[i]] = multipliers[i];
            }



        }

        private static void ValidatePermute(int[] src, int[] permute) {
            // validate permute
            if (permute.Length != src.Length) {
                throw new System.ArgumentException($"Permute length invalid. Expeted: {src.Length} Got: {permute.Length}");
            }

            // matk each index with a negative
            for (int i = 0; i < permute.Length; i++) {
                var ind = Mathf.Abs(permute[i]);
                ind %= permute.Length;
                if (permute[ind] == 0) {
                    // zero can't be negative, so replace it with the length temporarily
                    permute[ind] = -permute.Length;
                }
                else {
                    permute[ind] *= -1;
                }
            }

            // check each value is negative
            for (int i = 0; i < permute.Length; i++) {
                if (permute[i] >= 0) {
                    throw new System.ArgumentException($"Invalid permutation: {permute.ContentString()}");
                }
                if (permute[i] == -permute.Length) {
                    permute[i] = 0;
                }
                else {
                    permute[i] *= -1;
                }
            }
        }

        class Cache {
            public Tensor result;
            public Tensor error;
            public int[] offsets;
        }
    }

}