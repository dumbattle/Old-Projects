using UnityEngine;
using DumbML;


namespace Flappy {
    public abstract class Bird : IGameObject {
        public GameObject obj { get;}
        public Vector2 position { get => new Vector2(1, height); }

        public float height;
        public float velocity = 0;
        public Game g;


        public Bird(Game g) {
            obj = FlappyBird.GetBird();
            this.g = g;
        }

        public virtual void Next() {
            velocity -= .1f;
            height += velocity / 10f;
            if (height <= 0 || height >= Game.gameSize.y) {
                g.BirdCollision();
            }
        }
        public virtual void Reset() {


            velocity = 0;
            height = Game.gameSize.y / 2;
        }
        public void Jump() {
            velocity = 2;
        }
    }

    public class BirdRL : Bird {
        public FlappyAgent ai;

        RLExperience exp;

        public BirdRL(Game g) : base(g) {
            ai = new FlappyAgent();
        }

        public override void Reset() {
            if (exp != null) {
                exp.reward = -1;
                ai.AddExperience(exp);
            }
            exp = null;

            base.Reset();
        }


        public override void Next() {
            ai.Train(32, 1);

            var t = g.GetState();

            if (exp != null) {
                exp.reward = 1;
                exp.nextState = t;
                ai.AddExperience(exp);
            }

            exp = ai.GetAction(t);

            if (exp.action == 0) {
                Jump();
            }

            FlappyBird.main.output = ai.Result.ToString();
            FlappyBird.main.advantage = ai.a.result.ToString();
            FlappyBird.main.value = ai.v.result.ToString();
            base.Next();
        }  
    }


    public class FlappyAgent : RLAgent {
        public Operation a, v;

        public FlappyAgent(int expBufferSize = 10000) : base(expBufferSize) {
            Operation op = new InputLayer(4).Build();
            op = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            op = Attention(op);
            op = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            op = Attention(op);

            a = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            a = Attention(a);
            a = new FullyConnected(2,null).Build(op);

            v = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            v = Attention(v);
            v = new FullyConnected(1,null).Build(op);

            op = v - new Sum(a) / 2;
            op = new Append(op, op);

            op += a;
            Build(op);
            SetOptimizer(new Adam(), Loss.MSE);            
        }

        Operation GetInner(Operation input) {
            var op = new FullyConnected(10, ActivationFunction.Sigmoid).Build(input);
            op = new FullyConnected(10, ActivationFunction.Sigmoid).Build(op);

            return op;
        }

        Operation Attention(Operation input) {
            var atn = new FullyConnected(input.shape[0], ActivationFunction.Sigmoid, false).Build(input);

            return input * atn;
        }
    }

    public class BirdPG : Bird {
        FlappyPG ai;

        public BirdPG(Game g) : base(g) {
            ai = new FlappyPG();
        }

        public override void Next() {
            var state = g.GetState();

            var policy = ai.Compute(state);
            var action = Sample(policy);

            if (action == 0) {
                Jump();
            }

            FlappyBird.main.output = ai.Result.ToString();
            FlappyBird.main.advantage = ai.a.result.ToString();
            FlappyBird.main.value = ai.v.result.ToString();
            base.Next();

        }

        public int Sample(Tensor t) {
            int result = 0;
            for (int i = 0; i < t.Shape[0]; i++) {
                if (t[i] > result) {
                    result = 1;
                }
            }
            return result;
        }
    }



    public class FlappyPG : Model {
        public Operation a, v;

        public FlappyPG() {
            Operation op = new InputLayer(4).Build();
            op = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            op = new FullyConnected(10, ActivationFunction.Relu).Build(op);

            a = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            a = new FullyConnected(2, null).Build(op);

            v = new FullyConnected(10, ActivationFunction.Relu).Build(op);
            v = new FullyConnected(1, null).Build(op);

            op = v - new Sum(a) / 2;
            op = new Append(op, op);

            op += a;
            Build(op);
        }
    }

}