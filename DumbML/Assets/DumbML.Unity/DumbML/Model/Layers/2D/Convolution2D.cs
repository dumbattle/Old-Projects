using System;
using FullSerializer;

namespace DumbML {
    public class Conv2DBuilder {
        (int x, int y) filterSize;
        int numOutputChannels;
        (int x, int y) stride = default;
        Func<float> wi = null;
        ActivationFunction af = null;
        bool pad = false;
        bool bias = false;


        public Conv2DBuilder FilterSize((int x, int y) filterSize) {
            this.filterSize = filterSize;
            return this;
        }
        public Conv2DBuilder FilterSize(int x, int y) {
            this.filterSize = (x, y);
            return this;
        }
        public Conv2DBuilder Channels(int numOutputChannels) {
            this.numOutputChannels = numOutputChannels;
            return this;
        }
        public Conv2DBuilder Stride((int x, int y) stride) {
            this.stride = stride;
            return this;
        }
        public Conv2DBuilder Stride(int x, int y) {
            this.stride = (x, y);
            return this;
        }
        public Conv2DBuilder WeightInitializer(Func<float> weightInitializer) {
            this.wi = weightInitializer;
            return this;
        }
        public Conv2DBuilder AF(ActivationFunction af) {
            this.af = af;
            return this;
        }
        public Conv2DBuilder Pad(bool pad) {
            this.pad = pad;
            return this;
        }
        public Conv2DBuilder Bias(bool bias) {
            this.bias = bias;
            return this;
        }


        public Convolution2D Build() {
            return new Convolution2D(filterSize, numOutputChannels, stride, wi, af, pad,bias);
        }
    }
    [fsObject(MemberSerialization = fsMemberSerialization.OptIn)]
    public class Convolution2D : NeuralNetwork {
        public Convolution2D((int x, int y) filterSize, int numOutputChannels, (int x, int y) stride = default, Func<float> weightInitializer = null, ActivationFunction af = null, bool pad = false, bool bias = false) {
            if (bias) {
                //Add(new FullConv(filterSize, numOutputChannels, stride, weightInitializer, null, pad));
                Add(new Conv2DDepthwise(filterSize, stride, weightInitializer, null, pad));
                Add(new Conv2DPointwise(numOutputChannels, weightInitializer));
                Add(new Bias());
                Add(new ActivationLayer(af));
            }
            else {
                Add(new Conv2DDepthwise(filterSize, stride, weightInitializer, null, pad));
                Add(new Conv2DPointwise(numOutputChannels, weightInitializer, af));
                //Add(new FullConv(filterSize, numOutputChannels, stride, weightInitializer, af, pad));
            }
        }


        public override void Build(Layer prevLayer) {
            inputShape = prevLayer.outputShape;
            inputLayer = new InputLayer(inputShape);

            Layer l = prevLayer;

            for (int i = 0; i < Layers.Count; i++) {
                Layers[i].Build(l);
                l = Layers[i];
            }

            IsBuilt = true;
            this.outputShape = l.outputShape;
        }

        public Tensor Visualize(NeuralNetwork nn, int index) {
            NeuralNetwork _nn = new NeuralNetwork(nn.inputShape);
            BuildNetwork(nn);

            nn = _nn;
            _nn.Build();


            nn.Trainable = false;
            Optimizer o = new SGD(_nn);

            var s = o.NewSession();

            Tensor input = new Tensor(() => .5f, _nn.inputShape);
            Tensor target = new Tensor(() => float.NaN, _nn.outputShape);

            for (int x = 0; x < outputShape[0]; x++) {
                for (int y = 0; y < outputShape[1]; y++) {
                    target[x, y, index] = 1;
                }
            }

            for (int i = 0; i < 5; i++) {
                var output = s.Forward(input);
                var err = s.ComputeLoss(target);
                var e = s.Backward(err.Item2);

                input = input.PointWise(e.Item1, (a, b) => a - b.Sign() * .1f);
            }
            return input;


            bool BuildNetwork(NeuralNetwork n) {

                foreach (var l in n.Layers) {
                    if (l.ID == ID) {
                        _nn.Add(l);
                        return true;
                    }

                    NeuralNetwork inner = l as NeuralNetwork;

                    if (inner != null) {
                        ;
                        if (BuildNetwork(inner)) {
                            return true;
                        }
                    }
                    else {
                        _nn.Add(l);
                    }
                }
                return false;
            }
        }
    }
}