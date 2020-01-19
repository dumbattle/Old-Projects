using UnityEngine;
using DumbML;


namespace Flappy {
    public class Bird : IGameObject {
        public GameObject obj { get; }
        public Vector2 position { get => new Vector2(1, height); }

        public float height;
        public float velocity = 0;

        public FlappyAgent ai;

        Game g;

        public Bird(Game g) {
            this.g = g;
            obj = FlappyBird.GetBird();
            ai = GetAI();
        }
        public void Reset() {
            if (exp != null) {
                exp.reward = -1;
                ai.AddExperience(exp);
            }
            ai.exploration = Random.Range(-.05f, .05f);
            exp = null;

            velocity = 0;
            height = Game.gameSize.y / 2;
        }

        RLExperience exp;

        public void Next() {
            ai.Train(32, 1);

            var t = g.GetTensor();

            if (exp != null) {
                exp.reward = 1;
                exp.nextState = t;
                ai.AddExperience(exp);
            }

            exp = ai.GetAction(t);

            Debug.Log($"{exp.output} Value: {ai.v.result} Advantage: {ai.a.result}");

            if (exp.action == 0) {
                Jump();
            }
            velocity -= .1f;
            height += velocity / 10f;
            if (height <=0 || height >= Game.gameSize.y) {
                g.BirdCollision();
            }
        }



        public void Jump() {
            velocity = 2;
        }

        public FlappyAgent GetAI() {
           return new FlappyAgent();
   
        }
        
    }


    public class FlappyAgent : RLAgent {
        public Operation a, v;
        public FlappyAgent(int expBufferSize = 1000) : base(expBufferSize) {
            //NeuralNetwork result = new NeuralNetwork(4);
            //result.Add(new FullyConnected(10, ActivationFunction.Tanh));
            //result.Add(new FullyConnected(10, ActivationFunction.Tanh));
            //result.Add(new FullyConnected(10, ActivationFunction.Tanh));
            //result.Add(new FullyConnected(10, ActivationFunction.Tanh));
            //result.Add(new FullyConnected(2));

            //var op = result.Build();

            Operation op = new InputLayer(4).Build();
            op = new FullyConnected(100, ActivationFunction.LeakyRelu).Build(op);
            op = new FullyConnected(100, ActivationFunction.LeakyRelu).Build(op);

            a = new FullyConnected(100, ActivationFunction.LeakyRelu).Build(op);
            a = new FullyConnected(2,null).Build(op);

            v = new FullyConnected(100, ActivationFunction.LeakyRelu).Build(op);
            v = new FullyConnected(1,null).Build(op);

            op = v - new Sum(a) / 2;
            op = new Append(op, op);

            op += a;
            Build(op);
            SetOptimizer(new SGD(.001f), Loss.MSE);

        }
    }
}