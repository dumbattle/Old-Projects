using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;
namespace Flappy {
    public class FlappyBird : MonoBehaviour {
        public static FlappyBird main;
        public GameObject bird;
        public GameObject background;
        public GameObject block;

        public float height = 10;
        public Vector2 gameSize;
        public Vector2 blockSize;

        public string output, value, advantage;
        Game game;


        void Start() {
            main = this;
            Game.gameSize = gameSize;
            Game.blockSize = blockSize;
            background.SetActive(false);
            bird.SetActive(false);
            block.SetActive(false);
            game = new Game(-gameSize / 2);
            StartCoroutine(Next());
        }

        IEnumerator Next() {
            while (true) {
                game.Next();
                yield return null;
            }
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