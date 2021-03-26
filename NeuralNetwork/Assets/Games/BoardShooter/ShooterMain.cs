using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DumbML;

namespace Shooter {
    public class ShooterMain : MonoBehaviour {
        public static ShooterMain main;
        public string Player1;
        public string Player2;

        public int currentTrain = 1;
        public int winCount = 1;
        public int gameSpeed = 1;

        Game g;

        private void Awake() {
            main = this;
        }

        private void Start() {
            g = new Game(16,16);
        }
        private void Update() {
            for (int i = 0; i < gameSpeed; i++) {
                g.Next();
            }
        }

        private void OnDrawGizmos() {
            if (g == null) {
                return;
            }

            g.DrawGizmoMap();
        }
    }




    public class Game {
        public float pathTime = 5;
        (int x, int y) player1, player2;

        AI ai1, ai2;
        RLExperience r1, r2;
        Tensor map;
        int width, height;
        int train1 = 1;
        int winCount = 0;

        public Game(int x, int y) {
            map = new Tensor(x, y);
            ai1 = new AI(x, y);
            ai2 = new AI(x, y);

            ai1.Build();
            ai2.Build();
            width = x;
            height = y;
            Init();
            winCount = 0;
            train1 = 1;
            ShooterMain.main.currentTrain = 1;
        }


        public void Init() {
            ShooterMain.main.winCount =winCount;

            map.PointWise((a) => 0, true);
            player1 = (1, 1);
            player2 = (width - 2, height - 2);
            pathTime = 5;
            r1 = null;
            r2 = null;
        }

        public void Next() {
            pathTime += .1f;
            map.PointWise((a) => { var A = a; A -= 1f / pathTime; if (A < 0) { A = 0; } return A; }, true);

            Player1Act();
            if (!Check1()) {
                r1.reward = -1;
                r2.reward = 1;
                AddExp();

                End(2);
                return;
            }

            Player2Act();
            if (!Check2()) {
                r1.reward = 1;
                r2.reward = -1;
                AddExp();
                
                End(1);
                return;
            }

            AddExp();
        }

        void End(int winner) {
                ai1.EndTrajectory();
                ai2.EndTrajectory();

            Init();
            return;
            if (train1 == 1) {
                ai1.EndTrajectory();
                ai2.ClearTrajectory();
                if (winner == 1) {
                    winCount++;
                }
                else {
                    winCount = 0;
                }

            }
            else {
                ai2.EndTrajectory();
                ai1.ClearTrajectory();
                if (winner == 2) {
                    winCount++;
                }
                else {
                    winCount = 0;
                }
            }
            if(winCount == 10) {
                train1 *= -1;
                winCount = 0;
                ShooterMain.main.currentTrain = train1 == 1 ? 1 : 2;
            }
            Init();
        }
        void AddExp() {
                ai1.AddExperience(r1);
                ai2.AddExperience(r2);
        }

        void Player1Act() {
            Tensor state1 = new Tensor(width, height, 3);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    state1[x, y, 2] = map[x, y];
                }
            }

            state1[player1.x, player1.y, 0] = 1;
            state1[player2.x, player2.y, 1] = 1;

            r1 = ai1.SampleAction(state1);
            map[player1.x, player1.y] = 1;
            switch (r1.action) {
                case 0:
                    player1.y += 1;
                    break;
                case 1:
                    player1.x += 1;
                    break;
                case 2:
                    player1.y -= 1;
                    break;
                case 3:
                    player1.x -= 1;
                    break;
            }
            ShooterMain.main.Player1 = r1.output.ToString();
        }

        void Player2Act() {
            Tensor state = new Tensor(width, height, 3);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    state[x, y, 2] = map[x, y];
                }
            }

            state[player2.x, player2.y, 0] = 1;
            state[player1.x, player1.y, 1] = 1;

            r2 = ai2.SampleAction(state);
            map[player2.x, player2.y] = 1;
            switch (r2.action) {
                case 0:
                    player2.y += 1;
                    break;
                case 1:
                    player2.x += 1;
                    break;
                case 2:
                    player2.y -= 1;
                    break;
                case 3:
                    player2.x -= 1;
                    break;
            }
            ShooterMain.main.Player2 = r2.output.ToString();
        }


        bool Check1() {
            return IsInRange(player1.x, player1.y) && map[player1.x, player1.y] == 0 && player1 != player2;
        }

        bool Check2() {
            return IsInRange(player2.x, player2.y) && map[player2.x, player2.y] == 0 && player1 != player2;
        }



        bool IsInRange(int x, int y) {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        public void DrawGizmoMap() {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Gizmos.color = Color.Lerp(Color.black, Color.yellow, map[x, y]);
                    Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
                }
            }


            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector3(player1.x, player1.y), Vector3.one);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(player2.x, player2.y), Vector3.one);
        }
    }

    public class AI : ActorCritic {
        (int x, int y) size;
        public AI(int x, int y) {
            size = (x, y);
        }

        protected override Operation Input() {
            Operation inp = new InputLayer(size.x, size.y, 3).Build();
            Operation x = new Convolution2D(8, (3, 3), ActivationFunction.Tanh, (1, 1), false).Build(inp);
            x = new Convolution2D(8, (3, 3), ActivationFunction.Tanh, (1, 1), false).Build(x);
            x = new Convolution2D(8, (3, 3), ActivationFunction.Tanh, (1, 1), false).Build(x);
            x = new Convolution2D(8, (3, 3), ActivationFunction.Tanh, (1, 1), false).Build(x);
            x = new Convolution2D(1, (3, 3), ActivationFunction.Tanh, (1, 1), false).Build(x);
            x = new Flatten().Build(x);

            Operation y = new Convolution2D(8, (3, 3), ActivationFunction.Tanh, (1, 1), true).Build(inp);
            y = new Convolution2D(8, (3, 3), ActivationFunction.Tanh, (1, 1), true).Build(y);
            y = new Convolution2D(8, (3, 3), ActivationFunction.Tanh, (1, 1), true).Build(y);
            y = new Convolution2D(8, (3, 3), ActivationFunction.Tanh, (1, 1), true).Build(y);
            y = new Convolution2D(1, (3, 3), ActivationFunction.Tanh, (1, 1), true).Build(y);
            y = new Flatten().Build(y);

            Operation result = new Append(x, y);
            result = new FullyConnected(64, ActivationFunction.Sigmoid, false).Build(result);
            return result;
        }

        protected override Operation Actor(Operation input) {
            Operation x = new FullyConnected(4, bias: false).Build(input);
            x = x.Softmax();
            return x;
        }

        protected override Operation Critic(Operation input) {
            Operation x = new FullyConnected(1, bias: true).Build(input);
            return x;
        }

        protected override Optimizer Optimizer() {
            return new SGD();
        }
    }
}
