using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;


public class RandomTestStuffMain : MonoBehaviour {


    void Start() {

        Operation input = Tensor.FromArray(new float[] { 1000, 2, 1, -1, -2, 1, 1, 1, 1 });
        {
            var sf = new Softmax(input);
            sf.Eval();
            Gradients g = new Gradients(input);
            sf.Backwards(g);
            print(sf.value);

            //print(g[input]);
        }
        {
            var sf = (input).Softmax();
            sf.Eval();
            Gradients g = new Gradients(input);
            sf.Backwards(g);
            print(sf.value);

            //print(g[input]);
        }

    }
    private void Update() {
    }


}