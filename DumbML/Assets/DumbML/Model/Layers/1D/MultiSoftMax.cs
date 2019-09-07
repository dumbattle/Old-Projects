using System;
using System.Collections.Generic;
using FullSerializer;

namespace DumbML {
    /// <summary>
    /// Input: 1D Tensor
    /// Output: Same as Input
    /// </summary>
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class MultiSoftMax : Layer {
        [fsProperty]
        public int[] sliceSizes;
        [fsProperty]
        public Tensor[] slices;


        public MultiSoftMax(params int[] sliceSizes) {
            this.sliceSizes = sliceSizes;
            outputShape = new int[] { 0 };

            foreach (var i in sliceSizes) {
                outputShape[0] += i;
            }

            slices = new Tensor[sliceSizes.Length];
        }

        public override void Build(Layer prevLayer) {
            if (prevLayer.outputShape.Length != 1) {
                throw new WrongShapeException("Input to MultiSoftMax Layer must be 1D");
            }

            if (IsBuilt) {
                if (prevLayer.outputShape[0] != outputShape[0]) {
                    throw new WrongShapeException("Input to MultiSoftMax Layer is wrong size. Expected " + outputShape[0] + " Got " + inputShape[0]);
                }
                return;
            }

            inputShape = prevLayer.outputShape;
            if (inputShape[0] != outputShape[0]) {
                throw new WrongShapeException("Input to MultiSoftMax Layer is wrong size. Expected " + outputShape[0] + " Got " + inputShape[0]);
            }

            IsBuilt = true;
        }

        public override Tensor Compute(Tensor input) {
            if (!input.CheckShape(inputShape)) {
                throw new WrongShapeException("Input to MultiSoftMax wrong shape. Expected " + inputShape[0] + " Got " + input.Shape[0]);
            }
            IEnumerator<float> ie = input.GetEnumerator();
            IEnumerator<float>[] ies = new IEnumerator<float>[slices.Length];

            //go through the whole input
            Func<float> exp =
                () => {
                    ie.MoveNext();
                    return (float)Math.Exp(ie.Current);
                };


            //each slice only gets their slice from the input
            for (int i = 0; i < slices.Length; i++) {
                //exponent
                slices[i] = new Tensor(exp, sliceSizes[i]);

                //divide by sum
                float sum = 0;

                foreach (float f in slices[i]) {
                    sum += f;
                }
                slices[i] /= sum;

                //get enumerators so we can combine them
                ies[i] = slices[i].GetEnumerator();
            }

            int currentindex = 0;
            ie = ies[0];

            Func<float> initializer =
                () => {
                    while (!ie.MoveNext()) {
                        currentindex++;
                        ie = ies[currentindex];
                    }

                    return ie.Current;
                };


            Tensor result = new Tensor(initializer, outputShape);
            return result;
        }


        public override (Tensor, JaggedTensor) Backwards(Context context, Tensor error) {
            Tensor inputError = context.input.SameShape();
            int startInd = 0;

            //for each slice...
            for (int s = 0; s < slices.Length; s++) {

                //...iterate through output slice
                for (int j = startInd; j < startInd + sliceSizes[s]; j++) {
                    float e = error[j];

                    //...iterate through input slice
                    for (int k = startInd; k < startInd + sliceSizes[s]; k++) {
                        float derivative =
                            j == k
                            ? output[k] * (1 - output[j])
                            : -output[j] * output[k];

                        inputError[k] += e * derivative;
                    }
                }


                startInd += sliceSizes[s];
            }

            return (inputError, JaggedTensor.Empty);
        }

        public override void Update(JaggedTensor gradients) { }
    }
}