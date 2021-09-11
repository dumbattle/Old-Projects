using System.Collections;
using UnityEngine;


namespace Astroids {

    public class AstroidsACMain : AstroidMainBase {
        public DumbML.ModelWeightsAsset weights;

        [Header("Objects")]

        [Header("Settings")]
        public int gameSpeed;


        [TextArea(10,10)]
        public string actor;
        Player player;

        GameData game;



        void Start() {
            InitEnvironment();
            player = new ACPlayer(weights);

            game = new GameData();
            game.mapSize = Parameters.mapSize;
            game.playerPos = new Vector2(0, 0);
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
            if (game.playerPos.x > Parameters.mapSize.x / 2) {
                ResetGame();
                return;
            }
            if (game.playerPos.x < Parameters.mapSize.x / -2) {
                ResetGame();
                return;
            }
            if (game.playerPos.y > Parameters.mapSize.y / 2) {
                ResetGame();
                return;
            }
            if (game.playerPos.y < Parameters.mapSize.y / -2) {
                ResetGame();
                return;
            }
            if (Random.value < Parameters.astroidSpawnChance) {
                var a = Astroid.Get();

                Vector2 spawnLocation = new Vector2();
                Vector2 dir = new Vector2();
                int side = Random.Range(0, 4);
                const float DRIFT = 1;
                switch (side) {
                    case 0:
                        spawnLocation = new Vector2(Parameters.mapSize.x * Random.Range(-1f, 1f) / 2, Parameters.mapSize.y / 2 + 2);
                        dir = new Vector2(Random.Range(-DRIFT, DRIFT), -1).normalized;
                        break;
                    case 1:
                        spawnLocation = new Vector2(Parameters.mapSize.x / 2 + 2, Parameters.mapSize.y * Random.Range(-1f, 1f) / 2);
                        dir = new Vector2(-1, Random.Range(-DRIFT, DRIFT)).normalized;
                        break;
                    case 2:
                        spawnLocation = new Vector2(Parameters.mapSize.x * Random.Range(-1f, 1f) / 2, -Parameters.mapSize.y / 2 - 2);
                        dir = new Vector2(Random.Range(-DRIFT, DRIFT), 1).normalized;
                        break;
                    case 3:
                        spawnLocation = new Vector2(-Parameters.mapSize.x / 2 - 2, Parameters.mapSize.y * Random.Range(-1f, 1f) / 2);
                        dir = new Vector2(1, Random.Range(-DRIFT, DRIFT)).normalized;
                        break;
                }

                a.pos = spawnLocation;
                a.dir = dir;
                a.dist = 0;
                a.size = Random.Range(Parameters.astroidSize.x, Parameters.astroidSize.y);
                game.astroids.Add(a);
            }

            for (int i = 0; i < game.astroids.Count; i++) {
                Astroid a = game.astroids[i];
                a.pos += a.dir * Parameters.AstroidrSpeed;

                if ((a.pos - game.playerPos).sqrMagnitude < (a.size + Parameters.playerSize) * (a.size + Parameters.playerSize) / 4) {
                    ResetGame();
                    break;
                }

                a.dist++;
                if (a.dist > 100 * Parameters.mapSize.x * Parameters.mapSize.y) {
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
