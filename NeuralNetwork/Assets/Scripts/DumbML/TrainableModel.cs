using System.Linq;

namespace DumbML {
    public class TrainableModel : Model {
        public Optimizer opt;
        public Operation loss;
        public Tensor l;

        public Placeholder targetPH;


        public TrainableModel() { }
        public TrainableModel(Operation op) : base(op) { }
        public void SetOptimizer(Optimizer o, Loss l) {
            targetPH = new Placeholder(outputShape);

            loss = l.Compute(forward, targetPH);

            o.InitializeGradients(forward.GetNewGradients());
            opt = o;
            this.l = new Tensor(loss.shape);
        }

        public Tensor Train(Tensor[][] inputs, Tensor[] targets, int batchSize = 32) {
            Tensor totalLoss = new Tensor(loss.shape);
            l.PointWise((a) => 1f / batchSize, true);
            opt.ZeroGrad();
            int count = 0;

            for (int i = 0; i < inputs.Length; i++) {
                if (inputs[i] == null || targets[i] == null) {
                    continue;
                }
                SetInputs(inputs[i]);

                targetPH.SetVal(targets[i]);

                totalLoss.Add(loss.Eval(), true);
                loss.Backwards(opt, l);


                count++;
                if (count >= batchSize) {
                    opt.Update();
                    opt.ZeroGrad();
                    count = 0;
                }
            }

            return totalLoss / inputs.Length;
        }

        public Tensor Train(Tensor[] inputs, Tensor[] targets, int batchSize = 32, bool shuffle = true) {
            Tensor totalLoss = new Tensor(loss.shape);
            l.PointWise((a) => 1f / batchSize, true);
            opt.ZeroGrad();
            int count = 0;

            var indexes = Enumerable.Range(0, inputs.Length).ToArray();

            if (shuffle) {
                indexes.Shuffle();
            }

            for (int i = 0; i < inputs.Length; i++) {
                var input = inputs[indexes[i]];
                var target = targets[indexes[i]];

                if (input == null || target == null) {
                    continue;
                }
                SetInputs(input);

                targetPH.SetVal(target);

                totalLoss.Add(loss.Eval(), true);
                loss.Backwards(opt, l);


                count++;
                if (count >= batchSize) {
                    opt.Update();
                    opt.ZeroGrad();
                    count = 0;
                }
            }

            return totalLoss / inputs.Length;
        }
    }
}