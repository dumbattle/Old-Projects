using System;
using System.Collections.Generic;

namespace DumbML {
    public class Model {
        public int[] outputShape => forward?.shape;
        public Operation forward { get; set; }

        public Placeholder[] inputs;
        public Tensor Result => forward.value;

        public Model() { }
        public Model(Operation op) {
            var inputs = op.GetOperations<Placeholder>().ToArray();
            op.Optimize();
            Build(op, inputs);
        }
        public Model(Operation op, params Placeholder[] inputs) {
            Build(op, inputs);
        }



        protected void Build(Operation op) {
            var inputs = op.GetOperations<Placeholder>();
            Build(op, inputs.ToArray());
        }
        protected void Build(Operation op, params Placeholder[] inputs) {
            forward = op;
            this.inputs = inputs;
        }

        public Tensor Compute(params Tensor[] input) {
            SetInputs(input);

            return forward.Eval().Copy();
        }

        protected void SetInputs(params Tensor[] input) {
            for (int i = 0; i < input.Length; i++) {
                inputs[i].SetVal(input[i]);
            }
        }
        public Variable[] GetVariables() {
            return forward.GetVariables();
        }

        public Model Copy() {
            var op = forward.Copy();
            return new Model(op);
        }

        public Tensor[] GetWeights() {
            var vars = forward.GetVariables();
            Tensor[] result = new Tensor[vars.Length];

            for (int i = 0; i < result.Length; i++) {
                result[i] = vars[i].Value.Copy();
            }
            return result;
        }
        public void SetWeights(Tensor[] weights) {
            var v = GetVariables();

            if (v.Length != weights.Length) {
                throw new ArgumentException($"Number of weights does not match number of variables. Got: {weights.Length} Expected: {v.Length}");
            }

            for (int i = 0; i < v.Length; i++) {
                if (!weights[i].CheckShape(v[i].shape)) {
                    throw new ArgumentException($"Cannot set weight, wrong shape. Got: {weights[i].Shape.TOSTRING()} Expected: {v[i].shape} Index: {i}");
                }
            }
            for (int i = 0; i < v.Length; i++) {
                v[i].Value = weights[i].Copy();
            }
        }
    }


    public abstract class ActorCritic {
        RingBuffer<RLExperience> trajectory;
        float discount = .9f;

        public Model actorModel { get; private set; }
        public Model criticModel { get; private set; }
        protected Model combinedAC;

        Operation loss;
        List<Placeholder> inputPH;
        Placeholder actionMask, rewardPH;

        Optimizer o;
        Gradients g;

        public ActorCritic(int maxTrajectorySize = 1000) {
            trajectory = new RingBuffer<RLExperience>(maxTrajectorySize);
        }

        public void Build() {
            Operation input = Input();
            Operation a = Actor(input);
            Operation c = Critic(input);
            combinedAC = new Model(a + c);
            a.SetName("__ACTOR__");
            a.SetName("__CRITIC__");

            actorModel = new Model(a);
            criticModel = new Model(c);


            rewardPH = new Placeholder("t",1);

            var adv = rewardPH - c;
            actionMask = new Placeholder("tB",a.shape);


            var aloss = new Log(a * actionMask) * new BroadcastScalar(-1 * adv.Detach(), a.shape);
            var cLoss = Loss.MSE.Compute(rewardPH, c);

            loss = new Sum(aloss) + cLoss;

            g = new Gradients(loss.GetOperations<Variable>());
            o = Optimizer();
            o.InitializeGradients(g);
            inputPH = loss.GetOperations<Placeholder>(condition: (x) => x != rewardPH && x != actionMask);
        }

        protected abstract Operation Input();
        protected abstract Operation Actor(Operation input);
        protected abstract Operation Critic(Operation input);

        protected virtual Optimizer Optimizer() {
            return new SGD();
        }


        public virtual void AddExperience(RLExperience exp) {
            trajectory.Add(exp);
        }
        public virtual void EndTrajectory() {
            float score = 0;

            for (int i = trajectory.Count - 1; i >= 0; i--) {
                var exp = trajectory[i];

                score *= discount;
                score += exp.reward;
                exp.reward = score;
            }

            TrainAll();
            ClearTrajectory();
        }
        public void ClearTrajectory() {
            trajectory.Clear();
        }

        public RLExperience SampleAction(params Tensor[] state) {
            Tensor output = actorModel.Compute(state);

            int action = output.Sample();
            RLExperience result = new RLExperience(state, output, action);
            return result;
        }

        void TrainAll() {
            int size = trajectory.Count;
            int batchSize = 32;

            var inputs = new Tensor[size][];
            var masks = new Tensor[size];

            for (int i = 0; i < size; i++) {
                inputs[i] = trajectory[i].state;

                masks[i] = new Tensor(actorModel.outputShape);
                masks[i][trajectory[i].action] = 1;
            }

            int batchCount = 0;
            for (int i = 0; i < size; i++) {
                batchCount++;
                var r = trajectory[i].reward;

                for (int j = 0; j < inputPH.Count; j++) {
                    inputPH[j].SetVal(inputs[i][j]);
                }
                rewardPH.value[0] = r;
                actionMask.SetVal(masks[i]);

                loss.Eval();
                loss.Backwards(o);
                if (batchCount >= batchSize) {
                    o.Update();
                    o.ZeroGrad();
                    batchCount = 0;
                }
            }

            o.Update();
            o.ZeroGrad();
        }

        public Variable[] GetVariables()
        {
            return combinedAC.GetVariables();
        }
        public Tensor[] GetWeights() {
            return combinedAC.GetWeights();
        }
        public void SetWeights(Tensor[] weights) {
            combinedAC.SetWeights(weights);
        }
    }

}