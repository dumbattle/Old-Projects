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
    public class ACPlayer : Player{
        public string aout;
        Tensor[] inputCache = new Tensor[2];

        VariableStack1D astroidInput = new VariableStack1D(Parameters.AstroidDataSize);
        List<Tensor> atList = new List<Tensor>();
        AstroidAC ai;
        RLExperience exp;
        ObjectPool<Tensor> atPool = new ObjectPool<Tensor>(() => new Tensor(Parameters.AstroidDataSize));

        List<Tensor> activeAT = new List<Tensor>();
        ObjectPool<Tensor> pPool = new ObjectPool<Tensor>(() => new Tensor(2));

        List<Tensor> activePT = new List<Tensor>();
        ModelWeightsAsset weights;
        Channel scoreChannel;


        int score;


        public ACPlayer(ModelWeightsAsset weights) {
            ai = new AstroidAC();
            ai.Build();

            if (weights?.HasData ?? false) {
                ai.SetWeights(weights.Load());
            }
            this.weights = weights;

            scoreChannel = Channel.New("Score");
            scoreChannel.autoYRange = true;

            score = 0;
        }

        public override void Update(GameData game) {
            if (exp != null) {
                exp.reward = 1;
                ai.AddExperience(exp);
                score++;
            }
            Tensor pp = pPool.Get();
            activePT.Add(pp);
            pp[0] = game.playerPos.x / game.mapSize.x;
            pp[1] = game.playerPos.y / game.mapSize.y;

            atList.Clear();
            foreach (var a in game.astroids) {
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
            inputCache[0] = pp;
            inputCache[1] = astroidInput.value;
            exp = ai.SampleAction(inputCache);
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
            //aout = ai.testOP.value.ToString();
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
            weights?.Save(ai.GetVariables());
            scoreChannel.Feed(score);
            score = 0;
        }
    }
    public class AstroidAC : ActorCritic {
        public Operation testOP;
        protected override Operation Input() {
            Operation playerPos = new InputLayer(2).Build().SetName("Player Position Input");          // [2]
            Operation astroidInput = new Placeholder("Astroid Input", -1, Parameters.AstroidDataSize); // [x, a]

            //int asize = 8;
            //int bsize = 8;

            //Operation a = new FullyConnected(asize, ActivationFunction.Sigmoid, bias: false).Build(astroidInput); // [x, asize]
            //Operation b = new FullyConnected(bsize, ActivationFunction.Sigmoid, bias: false).Build(astroidInput); // [x, bsize]
            //a = new Transpose(a, new[] { 1, 0 }); // [asize, x]

            //Operation c = new MatrixMult(a, b); // [asize, bsize]

            //c = new FlattenOp(c);

            //Operation op = new Append(c, playerPos);
            //return op;
            int ksize = 16;
            int vsize = 17;
            int qSize = 3;
            Operation aKey = new FullyConnected(ksize, /*ActivationFunction.Tanh,*/ bias: false).Build(astroidInput); // [x, ksize]
            Operation aVal = new FullyConnected(vsize, /*ActivationFunction.Tanh, */bias: false).Build(astroidInput); // [x, vsize]

            //Operation aKeyWeights = new Tensor(() => RNG.Normal(), Parameters.AstroidDataSize, ksize);
            //Operation aKey = new MatrixMult(astroidInput, new Tanh(aKeyWeights));
            //Operation aValWeights = new Tensor(() => RNG.Normal(), Parameters.AstroidDataSize, vsize);
            //Operation aVal = new MatrixMult(astroidInput, new Tanh(aValWeights));

            Operation[] qInput = new Operation[qSize];
            for (int i = 0; i < qSize; i++) {
                //var qWeights = new Tensor(() => RNG.Normal(), 2, ksize);
                //qInput[i] = new MatrixMult(playerPos, new Tanh(qWeights));
                qInput[i] = new FullyConnected(ksize).Build(playerPos);
            }
            Operation q = new Stack1D(qInput);                              // [qsize, ksize]
            var (op, attn) = Attention.ScaledDotProduct(aKey, aVal, q);
            testOP = op;

            op = new FullyConnected(16, ActivationFunction.Sigmoid).Build(op); // [qsize, vsize]
            op = new FlattenOp(op);
            op = new Append(playerPos, op);
            return op;
        }

        protected override Operation Actor(Operation input) {
            Operation op = new FullyConnected(32, ActivationFunction.Sigmoid).Build(input);
            op = new FullyConnected(9, ActivationFunction.Sigmoid).Build(op);
            op = new FullyConnected(9).Build(op);
            op = op.Softmax();

            return op;
        }

        protected override Operation Critic(Operation input) {
            Operation op = new FullyConnected(16, ActivationFunction.Sigmoid).Build(input);
            var f = new FullyConnected(1);
            op = f.Build(op);
            f.bias.SetName("Critic bias");

            //return new Constant(new Tensor(0))
            return op;
        }

        protected override Optimizer Optimizer() {
            return new SGD(); ;
        }
    }

}
