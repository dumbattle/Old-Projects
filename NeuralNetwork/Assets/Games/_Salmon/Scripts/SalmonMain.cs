using Flappy;
using LPE;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Swimming {
    public class SalmonMain : MonoBehaviour {
        public SalmonParamsUiManagerBehaviour uiManager;
        public SalmonParameters parameters;
        public ObstacleEntry[] obstaclesEntries;
        public SalmonObjectBehaviour playerObj;
        Obstacle[] obstacles;
        public int steps;
        public float highScore;
        public float movingAvgScore;
        public float score;
        [Space]
        public int gameSpeed = 1;

        List<ActiveObstacles> activeObstacles = new List<ActiveObstacles>();

        SalmonGame game;

        SalmonTrainer trainer;

        SalmonAC ai = new SalmonAC();
        Stopwatch timer = new Stopwatch();
        float nextTargetScore = 100;

        string debugString = "";
        void Start() {
            Application.targetFrameRate = 30;
            obstacles = new Obstacle[obstaclesEntries.Length];
            for (int i = 0; i < obstaclesEntries.Length; i++) {
                ObstacleEntry e = obstaclesEntries[i];
                e.Init();
                obstacles[i] = e.obstacle;
            }

            playerObj.transform.localScale = new Vector3(parameters.playerDiameter, parameters.playerDiameter, 1);
            game = new SalmonGame(parameters, obstacles);
            game.Reset();


            trainer = new SalmonTrainer(game,ai);
            uiManager.Init(game);
        }

        void Update() {
            uiManager.UpdateParams(game);

           
            int targetFrames = 1;

            if(uiManager.spedUp) {
                targetFrames = 1000;
            }
            if(uiManager.paused) {
                targetFrames = 0;
            }


            timer.Restart();
            for (int i = 0; i < targetFrames; i++) {
                if (timer.ElapsedMilliseconds > 1f/30f) {
                    gameSpeed = i;
                    break;
                }
                steps++;
                var s = game.score;
                bool done = trainer.Next();
                if (done) {
                    if (s > highScore) {
                        highScore = s;
                    }
                    movingAvgScore = movingAvgScore * 0.99f + 0.01f * s;


                    if (movingAvgScore >= nextTargetScore) {
                        var s2 = $"Achieved average score of '{nextTargetScore}' after '{steps}' steps\n";
                        debugString += s2;
                        print(debugString);
                        print(s2);
                        nextTargetScore *= 2;
                    }
                }
            }
            score = game.score;
            timer.Stop();
            //game.Update(
            //    Input.GetKeyDown(KeyCode.UpArrow) ? MoveInput.up :
            //    Input.GetKeyDown(KeyCode.DownArrow) ? MoveInput.down :
            //    MoveInput.none);
            DrawGame();
        }


        void DrawGame() {
            playerObj.SetWorldPosition(new Vector2(parameters.playerDiameter, game.playerPosition), steps);
            foreach (var ao in activeObstacles) {
                ao.obj.gameObject.SetActive(false);
                obstaclesEntries[ao.index].pool.Return(ao.obj);
            }
            activeObstacles.Clear();
            foreach ((int index, Vector2 pos) in game.activeObstacles) {
                var e = obstaclesEntries[index];

                var obj = e.pool.Get();

                obj.gameObject.SetActive(true);
                activeObstacles.Add(new ActiveObstacles() { index = index, obj = obj });
                obj.SetWorldPosition(pos, steps);
            }
        }



        struct ActiveObstacles {
            public int index;
            public SalmonObjectBehaviour obj;
        }


        [System.Serializable]
        public class ObstacleEntry {
            public ObjectPool<SalmonObjectBehaviour> pool;
            public SalmonObjectBehaviour go;
            public Obstacle obstacle;


            public void Init() {
                pool = new ObjectPool<SalmonObjectBehaviour>(() => Instantiate(go));
                go.gameObject.SetActive(false);
            }
        }
    }
}