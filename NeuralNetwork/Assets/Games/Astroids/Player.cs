using UnityEngine;
using DumbML;
using System.Collections.Generic;
using LPE;

public static class P {
    static Unity.Profiling.ProfilerMarker pm = new Unity.Profiling.ProfilerMarker("Test");

    public static void S() {
        pm.Begin();
    }
    public static void E() {
        pm.End();
    }
}

namespace Astroids {

    public abstract class Player {
        public abstract void Update(GameData game);
        public abstract void EndGame(GameData game);
    }

    public class HumanPlayer : Player {
        public override void Update(GameData game) {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            input = input.normalized * Parameters.PlayerSpeed;

            game.playerPos += input;
        }
        public override void EndGame(GameData game) {
        }

    }
    public class ACPlayer : Player {
        public string aout;
        Tensor[] inputCache = new Tensor[2];

        VariableStack1D astroidInput = new VariableStack1D(Parameters.AstroidDataSize);
        List<Tensor> atList = new List<Tensor>();
        public AstroidAC ai;
        RLExperience exp;
        ObjectPool<Tensor> atPool = new ObjectPool<Tensor>(() => new Tensor(Parameters.AstroidDataSize));

        List<Tensor> activeAT = new List<Tensor>();
        ObjectPool<Tensor> pPool = new ObjectPool<Tensor>(() => new Tensor(2));

        List<Tensor> activePT = new List<Tensor>();
        ModelWeightsAsset weights;
        Channel scoreChannel;
        Channel avgScoreChannel;

        Variable[] variables;
        int score;
        float avgsSore;
        int episodeCount = 0;

        public ACPlayer(ModelWeightsAsset weights) {
            ai = new AstroidAC();
            ai.Build();
            variables = ai.GetVariables();
            if (weights?.HasData ?? false) {
                ai.SetWeights(weights.Load());
            }
            this.weights = weights;

            scoreChannel = Channel.New("Score");
            scoreChannel.autoYRange = true;
            avgScoreChannel = Channel.New("Average Score");
            avgScoreChannel.autoYRange = true;

            score = 0;
            avgsSore = 0;
        }

        public override void Update(GameData game) {
            if (exp != null) {
                exp.reward = 0;
                ai.AddExperience(exp);
                score++;
            }
            Tensor pp = pPool.Get();
            activePT.Add(pp);
            pp[0] = game.playerPos.x / game.mapSize.x;
            pp[1] = game.playerPos.y / game.mapSize.y;

            atList.Clear();
            foreach (var a in game.astroids) {
                if (!game.AstroidInMap(a)) {
                    continue;
                }
                var t = atPool.Get();
                t[0] = a.pos.x / game.mapSize.x;
                t[1] = a.pos.y / game.mapSize.y;
                t[2] = a.dir.x;
                t[3] = a.dir.y;
                t[4] = a.size;
                atList.Add(t);
                activeAT.Add(t);
            }
            astroidInput.SetValue(atList);
            astroidInput.Eval();
            inputCache[0] = pp;
            inputCache[1] = astroidInput.value;
            P.S();
            exp = ai.SampleAction(inputCache);
            P.E();
            Vector2 dir = new Vector2(0, 0);
            switch (exp.action) {
                case 0:
                    dir = new Vector2(0, 1);
                    break;
                case 1:
                    dir = new Vector2(1, 1).normalized;
                    break;
                case 2:
                    dir = new Vector2(1, 0);
                    break;
                case 3:
                    dir = new Vector2(1, -1).normalized;
                    break;
                case 4:
                    dir = new Vector2(0, -1);
                    break;
                case 5:
                    dir = new Vector2(-1, -1).normalized;
                    break;
                case 6:
                    dir = new Vector2(-1, 0);
                    break;
                case 7:
                    dir = new Vector2(-1, 1).normalized;
                    break;
            }


            game.playerPos += dir * Parameters.PlayerSpeed;
            //ai.criticModel.forward.Eval();
            aout = ai.attn.value?.ToString();
        }
        public override void EndGame(GameData game) {
            if (exp != null) {
                exp.reward = -1;
                ai.AddExperience(exp);
                exp = null;
            }

            ai.EndTrajectory();
            foreach (var at in activeAT) {
                atPool.Return(at);
            }
            activeAT.Clear();
            foreach (var t in activePT) {
                pPool.Return(t);
            }
            activePT.Clear();
            weights?.Save(variables);
            scoreChannel.Feed(score);
            float g = avgsSore == 0 ? 0 : .999f;
            avgsSore = avgsSore * g + score * (1 - g);
            avgScoreChannel.Feed(avgsSore);
            avgScoreChannel.SetRange(scoreChannel.yMin, scoreChannel.yMax);
            score = 0;
        }
    }
    public class AstroidAC : ActorCritic {
        public Operation testOP;
        public Operation attn;
        public AstroidAC() {
            discount = .9f;
        }
        protected override Operation[] Input() {
            Operation playerPos = new InputLayer(2).Build().SetName("Player Position Input");          // [2]
            Operation astroidInput = new Placeholder("Astroid Input", -1, Parameters.AstroidDataSize); // [x, a]

            return new[] { playerPos, astroidInput };
        }

        protected override Operation Actor(Operation[] input) {
            Operation playerPos = input[0];          // [2]
            Operation astroidInput = input[1];       // [x, a]
            int ksize = 8;
            int vsize = 8;
            int qSize = 8;
            Operation aKey = new FullyConnected(ksize).Build(astroidInput); // [x, ksize]
            Operation aVal = new FullyConnected(vsize).Build(astroidInput); // [x, vsize]

            Operation[] qInput = new Operation[qSize];
            for (int i = 0; i < qSize; i++) {
                qInput[i] = new FullyConnected(ksize).Build(playerPos);
            }
            Operation q = new Stack1D(qInput);                              // [qsize, ksize]
            var (op, attn) = Attention.ScaledDotProduct(aKey, aVal, q);
            this.attn = attn;

            op = new FlattenOp(op);
            op = new FullyConnected(64, ActivationFunction.Sigmoid).Build(op); // [qsize, vsize]
            op = new Append(playerPos, op);


            op = new FullyConnected(64, ActivationFunction.Sigmoid).Build(op);
            op = new FullyConnected(64, ActivationFunction.Sigmoid).Build(op);
            op = new FullyConnected(9).Build(op);
            op = new Softmax(op);

            return op;
        }

        protected override Operation Critic(Operation[] input) {
            Operation playerPos = input[0];          // [2]
            Operation astroidInput = input[1];       // [x, a]
            int ksize = 8;
            int vsize = 8;
            int qSize = 8;
            Operation aKey = new FullyConnected(ksize, bias: false).Build(astroidInput); // [x, ksize]
            Operation aVal = new FullyConnected(vsize, bias: false).Build(astroidInput); // [x, vsize]

            Operation[] qInput = new Operation[qSize];
            for (int i = 0; i < qSize; i++) {
                qInput[i] = new FullyConnected(ksize).Build(playerPos);
            }
            Operation q = new Stack1D(qInput);                              // [qsize, ksize]
            var (op, attn) = Attention.ScaledDotProduct(aKey, aVal, q);

            op = new FlattenOp(op);
            op = new FullyConnected(32, ActivationFunction.Sigmoid).Build(op); // [qsize, vsize]
            op = new Append(playerPos, op);


            op = new FullyConnected(32, ActivationFunction.Sigmoid).Build(op);
            op = new FullyConnected(16, ActivationFunction.Sigmoid).Build(op);
            var f = new FullyConnected(1);
            op = f.Build(op);
            f.bias.SetName("Critic bias");

            //return new Constant(new Tensor(0))
            return op;
        }

        public override Operation AuxLoss() {
            return null;
            Operation entropy = new Sum(attn * new Log(attn)) * new Constant(Tensor.FromArray(new float[] { -.00001f }));
            testOP = entropy;
            return entropy;
        }
        protected override Optimizer Optimizer() {
            return new Adam(); ;
        }
    }

}
