using System.Collections.Generic;
using LPE;


namespace DumbML {
    public abstract class ActorCritic {
        ObjectPool<RLExperience> xpPool;
        RingBuffer<RLExperience> trajectory;
        List<RLExperience> usedXP = new List<RLExperience>();
        float discount = .9f;

        public Model actorModel { get; private set; }
        public Model criticModel { get; private set; }
        public Model combinedAC;

        public Operation loss;
        List<Placeholder> inputPH;
        Placeholder actionMask, rewardPH;

        Optimizer o;
        Gradients g;

        TensorPool tensorPool = new TensorPool();

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


            rewardPH = new Placeholder("t", 1);

            var adv = rewardPH - c;
            actionMask = new Placeholder("tB", a.shape);


            var aloss = new Log(a * actionMask) * new BroadcastScalar(-1 * adv.Detach(), a.shape);
            var cLoss = Loss.MSE.Compute(rewardPH, c);

            loss = new Sum(aloss) + cLoss;

            g = new Gradients(loss.GetOperations<Variable>());
            o = Optimizer();
            o.InitializeGradients(g);
            inputPH = loss.GetOperations<Placeholder>(condition: (x) => x != rewardPH && x != actionMask);

            xpPool = new ObjectPool<RLExperience>(NewXP);
            trainMask = new Tensor(actorModel.outputShape);

            RLExperience NewXP() {
                var ta = new Tensor[inputPH.Count];

                return new RLExperience(ta, new Tensor(actorModel.outputShape), -1);
            }
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

            foreach (var xp in usedXP) {
                xpPool.Return(xp);
            }
            usedXP.Clear();
            tensorPool.ReturnAll();
        }

        public RLExperience SampleAction(params Tensor[] state) {
            Tensor output = actorModel.Compute(state);

            int action = output.Sample();
            RLExperience result = xpPool.Get();

            for (int i = 0; i < state.Length; i++) {
                var t = tensorPool.Get(state[i].Shape);
                t.CopyFrom(state[i]);
                result.state[i] = t;
            }


            result.output.CopyFrom(output);
            result.action = action;
            usedXP.Add(result);

            return result;
        }

        Tensor trainMask;
        void TrainAll() {
            int size = trajectory.Count;
            int batchSize = 32;

            int batchCount = 0;
            for (int i = 0; i < size; i++) {
                trainMask.SetValuesToZero();
                trainMask[trajectory[i].action] = 1;
                batchCount++;
                var r = trajectory[i].reward;

                for (int j = 0; j < inputPH.Count; j++) {
                    inputPH[j].SetVal(trajectory[i].state[j]);
                }

                rewardPH.value[0] = r;
                actionMask.SetVal(trainMask);

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

        public Variable[] GetVariables() {
            return combinedAC.GetVariables();
        }
        public Tensor[] GetWeights() {
            return combinedAC.GetWeights();
        }
        public void SetWeights(Tensor[] weights) {
            combinedAC.SetWeights(weights);
        }
    }

    class TensorPool {
        Dictionary<List<int>, ObjectPool<Tensor>> poolDict = new Dictionary<List<int>, ObjectPool<Tensor>>(new ShapeComparer());
        List<Tensor> active = new List<Tensor>();
        List<int> shapeCache = new List<int>();

        public Tensor Get(int[] shape) {
            shapeCache.Clear();
            shapeCache.AddRange(shape);

            Tensor result;

            // get tensor from pool
            if (poolDict.ContainsKey(shapeCache)) {
                result = poolDict[shapeCache].Get();
            }
            else {
                var p = A(shape);

                poolDict.Add(new List<int>(shape), p);

                result = p.Get();
            }

            active.Add(result);
            return result;
        }

        public void ReturnAll() {

            foreach (var t in active) {
                shapeCache.Clear();
                shapeCache.AddRange(t.Shape);

                poolDict[shapeCache].Return(t);
            }
            active.Clear();
        }


        ObjectPool<Tensor> A(int[] shape) {
            var p = new ObjectPool<Tensor>(() => new Tensor(shape));
            // this delegate creates garbage at the start of the function
            // so we put this into a different funtion so that garbage is minimized
            return p;
        }
    }

}
public static class P {
    static Unity.Profiling.ProfilerMarker pm = new Unity.Profiling.ProfilerMarker("Test2");

    public static void S() {
        pm.Begin();
    }
    public static void E() {
        pm.End();
    }
}