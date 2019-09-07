using System;
using System.Threading.Tasks;
using UnityEngine;
using DumbML;
using System.Diagnostics;

public class TensorTest : MonoBehaviour {
    void Start() {
        int v = 1;
        Tensor t1 = new Tensor(() => v++, 3, 3,3);

        print(t1);
        print(t1.Extend(0, 1));
        print(t1.Extend(1, 1));
        print(t1.Extend(2, 1));
    }

    Tensor Extend(Tensor t, int index, int length) {
        int[] shape = (int[])t.Shape.Clone();
        shape[index] += length;
        Tensor result = new Tensor(shape);


        int[] indices = new int[t.Rank];

      
        int tind = 0;

        indices[t.Rank - 1] = -1;

        for (int i = 0; i < result._value.Length; i++) {
            indices[t.Rank - 1]++;
            int j = t.Rank - 1;
            for (; j >= 0; j--) {
                if (indices[j] >= shape[j]) {
                    if (j != 0) {
                        indices[j - 1]++;
                    }
                    indices[j] = 0;
                }
                else {
                    break;
                }
            }


            if (indices[index] >= t.Shape[index]) {
                continue;
            }

            result._value[i] = t._value[tind];
            tind++; 
        }
        return result;
    }
}

