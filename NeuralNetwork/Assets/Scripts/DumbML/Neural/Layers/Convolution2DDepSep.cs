
namespace DumbML {
    public class Convolution2DDepSep : Layer {
        Variable weightDepth, weightPoint;

        public (int x, int y) stride, filterSize;
        bool pad;
        int outputChannels;

        ActivationFunction af;

        public Convolution2DDepSep(int outputChannels, (int x, int y) filterSize = default, ActivationFunction af = null, (int x, int y) stride = default, bool pad = false) {
            this.outputChannels = outputChannels;
            this.filterSize = (filterSize.x > 0 ? filterSize.x : 3, filterSize.y > 0 ? filterSize.y : 3);
            this.stride = (stride.x > 0 ? stride.x : 1, stride.y > 0 ? stride.y : 1);
            this.pad = pad;
            this.af = af ?? ActivationFunction.None;
        }
        public override Operation Build(Operation input) {
            weightDepth = new Tensor(() => RNG.Normal(), filterSize.x, filterSize.y, input.shape[2]);
            weightPoint = new Tensor(() => RNG.Normal(), input.shape[2], outputChannels);

            weightDepth.SetName($"{weightDepth.shape.ContentString()} Depthwise filter");
            weightPoint.SetName($"{weightPoint.shape.ContentString()} Pointwise filter");

            Operation op = new Conv2DDepth(input, weightDepth, stride, pad);
            op = new Conv2DPoint(op, weightPoint);

          
            op = af.Activate(op);

            return forward = op;
        }
    }
}