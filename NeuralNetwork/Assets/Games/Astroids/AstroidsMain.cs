using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LPE;


namespace Astroids {
    public class AstroidsMain : MonoBehaviour {
        public DumbML.ModelWeightsAsset weights;
        public Vector2 mapSize;
        [Range(0, 1f)]
        public float astroidSpawnChance;
        [Min(.01f)]
        public Vector2 astroidSize;
        public float playerSize = 1;

        [Header("Objects")]
        public GameObject background;
        public GameObject playerObj;
        public GameObject astroidSrc;

        [Header("Settings")]
        public int gameSpeed;


        [TextArea(10,10)]
        public string actor;
        GameData game;
        ObjectPool<GameObject> astroidPool;
        List<GameObject> activeAstroids = new List<GameObject>();
        Player player;




        void Start() {
            player = new ACPlayer(weights);
            astroidPool = new ObjectPool<GameObject>(() => Instantiate(astroidSrc));

            game = new GameData();
            game.mapSize = mapSize;
            game.playerPos = new Vector2(0, 0);
            background.transform.localScale = new Vector3(mapSize.x, mapSize.y, 1);
            astroidSrc.SetActive(false);
        }

        void Update() {
            for (int i = 0; i < gameSpeed; i++) {
                GameUpdate();
            }
            actor = ((ACPlayer)player).aout;
            DisplayGame();
        }

        private void GameUpdate() {

            player.Update(game);
            if (game.playerPos.x > mapSize.x / 2) {
                ResetGame();
                return;
            }
            if (game.playerPos.x < mapSize.x / -2) {
                ResetGame();
                return;
            }
            if (game.playerPos.y > mapSize.y / 2) {
                ResetGame();
                return;
            }
            if (game.playerPos.y < mapSize.y / -2) {
                ResetGame();
                return;
            }
            if (Random.value < astroidSpawnChance) {
                var a = Astroid.Get();

                Vector2 spawnLocation = new Vector2();
                Vector2 dir = new Vector2();
                int side = Random.Range(0, 4);
                const float DRIFT = 1;
                switch (side) {
                    case 0:
                        spawnLocation = new Vector2(mapSize.x * Random.Range(-1f, 1f) / 2, mapSize.y / 2 + 2);
                        dir = new Vector2(Random.Range(-DRIFT, DRIFT), -1).normalized;
                        break;
                    case 1:
                        spawnLocation = new Vector2(mapSize.x / 2 + 2, mapSize.y * Random.Range(-1f, 1f) / 2);
                        dir = new Vector2(-1, Random.Range(-DRIFT, DRIFT)).normalized;
                        break;
                    case 2:
                        spawnLocation = new Vector2(mapSize.x * Random.Range(-1f, 1f) / 2, -mapSize.y / 2 - 2);
                        dir = new Vector2(Random.Range(-DRIFT, DRIFT), 1).normalized;
                        break;
                    case 3:
                        spawnLocation = new Vector2(-mapSize.x / 2 - 2, mapSize.y * Random.Range(-1f, 1f) / 2);
                        dir = new Vector2(1, Random.Range(-DRIFT, DRIFT)).normalized;
                        break;
                }

                a.pos = spawnLocation;
                a.dir = dir;
                a.dist = 0;
                a.size = Random.Range(astroidSize.x, astroidSize.y);
                game.astroids.Add(a);
            }

            for (int i = 0; i < game.astroids.Count; i++) {
                Astroid a = game.astroids[i];
                a.pos += a.dir * Parameters.AstroidrSpeed;

                if ((a.pos - game.playerPos).sqrMagnitude < (a.size + playerSize) * (a.size + playerSize) / 4) {
                    ResetGame();
                    break;
                }

                a.dist++;
                if (a.dist > 100 * mapSize.x * mapSize.y) {
                    game.astroids.RemoveAt(i);
                    i--;
                }
            }
        }
        void ResetGame() {
            player.EndGame(game);
            game.playerPos = new Vector2(0, 0);
            foreach (var a in game.astroids) {
                Astroid.Return(a);
            }
            game.astroids.Clear();
        }
        void DisplayGame() {
            playerObj.transform.position = game.playerPos;
            foreach (var a in activeAstroids) {
                a.SetActive(false);
                astroidPool.Return(a);
            }
            activeAstroids.Clear();
            foreach (var a in game.astroids) {
                var obj = astroidPool.Get();
                obj.SetActive(true);
                activeAstroids.Add(obj);

                obj.transform.position = a.pos;
                obj.transform.localScale = new Vector3(a.size, a.size);
            }
        }


    }
}
