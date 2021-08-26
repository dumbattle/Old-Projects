using System.Collections.Generic;
using UnityEngine;

namespace DumbML {
    public class Transpose : Operation {
        static List<int> perm = new List<int>();

        List<int> index = new List<int>();
        int[] permutation;
        Dictionary<List<int>, Cache> valueDict = new Dictionary<List<int>, Cache>(new ShapeComparer());
        Cache c;
        Tensor[] errArr = new Tensor[1];
        public Transpose(Operation op, int[] permutation) : base(null, op) {
            ValidatePermute(op.shape, permutation);

            this.permutation = permutation;
            GetPermutation(op.shape, permutation);
            shape = perm.ToArray();
        }

        protected override Tensor _Compute(Tensor[] operands) {
            GetPermutation(operands[0].Shape, permutation);
            c = null;

            if (valueDict.ContainsKey(perm)) {
                c = valueDict[perm];
            }
            else {
                c = new Cache();
                c.result = new Tensor(perm);
                c.error = new Tensor(operands[0].Shape);

                int[] multipliers = new int[perm.Count];
                int[] o = new int[perm.Count];

                int scale = 1;
                for (int i = multipliers.Length - 1; i >= 0; i--) {
                    multipliers[i] = scale;
                    scale *= perm[i];
                }

                for (int i = 0; i < o.Length; i++) {
                    o[permutation[i]] = multipliers[i];
                }


                c.offsets = o;
                valueDict.Add(new List<int>(perm), c);
            }


            //init index
            index.Clear();
            for (int i = 0; i < perm.Count; i++) {
                index.Add(0);
            }

            var result = c.result;

            var offsets = c.offsets;
            for (int i = 0; i < operands[0].value.Length; i++) {
                var val = operands[0].value[i];

                int ind = 0;
                for (int j = index.Count - 1; j >= 0; j--) {
                    ind += index[j] * offsets[j];
                }

                result.value[ind] = val;

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

            return result;
        }

        protected override Tensor[] _BackwardsPass(Tensor e) {
            var offsets = c.offsets;
            var err = c.error;

            for (int i = 0; i < operands[0].value.Length; i++) {
                int ind = 0;
                for (int j = index.Count - 1; j >= 0; j--) {
                    ind += index[j] * offsets[j];
                }


                err.value[i] = e.value[ind];


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
            errArr[0] = err;
            return errArr;
        }

        public override Operation Copy(Dictionary<Operation, Operation> track) {
            return new Transpose(inner[0]._Copy(track), permutation);
        }

        static void GetPermutation(int[] src, int[] permute) {

            perm.Clear();

            for (int i = 0; i < src.Length; i++) {
                perm.Add(src[permute[i]]);
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