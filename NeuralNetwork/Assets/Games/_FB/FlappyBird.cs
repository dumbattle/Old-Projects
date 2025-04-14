using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;
using System.Diagnostics;
namespace Flappy {
    public class FlappyBird : MonoBehaviour {
        public static FlappyBird main;
        public static ModelWeightsAsset AiAsset => main.aiAsset;
        public ModelWeightsAsset aiAsset;
        public GameObject bird;
        public GameObject background;
        public GameObject block;

        public float height = 10;
        public Vector2 gameSize;
        public Vector2 blockSize;

        public string output, value, advantage, actor, critic;
        public int steps;
        public float highScore;
        public float score;
        [Space]
        public int gameSpeed = 1;
        Game game;
        Stopwatch timer = new Stopwatch();

        void Start() {
            main = this;
            Game.gameSize = gameSize;
            Game.blockSize = blockSize;
            background.SetActive(false);
            bird.SetActive(false);
            block.SetActive(false);
            game = new Game(-gameSize / 2);
            steps = 0;
        }

        void Update() {
            timer.Restart();
            for (int i = 0; i < gameSpeed; i++) {
                if (timer.ElapsedMilliseconds > 1000) {
                    gameSpeed = i;
                    break;
                }
                steps++;
                game.Next();
                if (game.score > highScore) {
                    highScore = game.score;
                }

            }
            timer.Stop();
            score = game.score;
        }

        public static GameObject GetBird() {
            GameObject o = Instantiate(main.bird);
            o.SetActive(true);
            return o;
        }
        public static GameObject GetBackground() {
            GameObject o = Instantiate(main.background);
            o.SetActive(true);
            return o;
        }
        public static GameObject GetBlock() {
            GameObject o = Instantiate(main.block);
            o.SetActive(true);
            return o;
        }
    }


}