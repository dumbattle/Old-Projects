using System.Linq;

namespace DumbML {
    public class Trainer {
        public Optimizer opt;
        public Operation loss;
        public Tensor l;

        public Placeholder targetPH;


        public Model model;

        Placeholder[] inputs;


        public Trainer(Model m, Optimizer o, Loss l){

            model = m;

            opt = o;
            o.InitializeGradients(m.forward.GetNewGradients());

            inputs = (Placeholder[])m.inputs.Clone();

            targetPH = new Placeholder("Target Placeholder", m.outputShape);

            loss = l.Compute(m.forward, targetPH);
            loss.Optimize();
            this.l = new Tensor(loss.shape);
        }
        public Trainer(Operation op, Optimizer o, Loss l) : this(new Model(op), o,l) {}


        public Tensor Train(Tensor[][] inputs, Tensor[] targets, int batchSize = 32, bool shuffle = true) {
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
        public Tensor Train(Tensor[] inputs, Tensor[] targets, int batchSize = 32, bool shuffle = true) {
            Tensor totalLoss = new Tensor(loss.shape);

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
                loss.Backwards(opt);
                count++;
                if (count >= batchSize) {
                    opt.Update();
                    opt.ZeroGrad();
                    count = 0;
                }
            }

            return totalLoss / inputs.Length;
        }

        void SetInputs(params Tensor[] input) {
            for (int i = 0; i < input.Length; i++) {
                inputs[i].SetVal(input[i]);
            }
        }
    }
}