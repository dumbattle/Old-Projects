using System.Collections.Generic;
using UnityEngine;

namespace AntSim {
    public class AntSim : MonoBehaviour {
        public const Orientation orientation = Orientation.horizontal;
        public const float DeltaTime = 1f / 45f;
        public static AntSim Main;

        public FoodField foodField;
        public GameObject hexTile;
        public int numScouts = 1;
        public int mapRadius = 1;
        public float scale = 1;
        public int numAnts;
        public float foodSpawnRate;

        float spawnCounter = 0;

        public HexGrid grid { get; private set; }
        public AntColony[] colonies;

        public static AntMap antMap;

        void Awake() {
            Main = this;
            antMap = new AntMap(mapRadius);
            Graph.YMin = -.1f;
            Graph.YMax = 1.1f;
            Graph.channel[0].SetActive("ga");
            Graph.channel[1].SetActive("gb");
            Graph.channel[2].SetActive("gc");
            Graph.channel[3].SetActive("gd");
            Graph.channel[4].SetActive("ge");
            Graph.channel[5].SetActive("gf");
            Graph.channel[6].SetActive("gg");
            Graph.channel[7].SetActive("gh");
        }
        void Start() {
            BuildMap();
            for (int i = 0; i < 10; i++) {
                SpawnFood();
            }
            //   o  o
            // o  o   o
            //   o  o
            colonies = new[] {
                //new AntColony(new HexCoord(0, 0)),
                new AntColony(new HexCoord(0, mapRadius / 2)),
                new AntColony(new HexCoord(mapRadius / 2, 0)),
                new AntColony(new HexCoord(mapRadius / 2, -mapRadius / 2)),
                new AntColony(new HexCoord(0, -mapRadius / 2)),
                new AntColony(new HexCoord(-mapRadius / 2, 0)),
                new AntColony(new HexCoord(-mapRadius / 2, mapRadius / 2))
            };

            foreach (var c in colonies) {
                c.stats.Mutate();
                c.Initialize();
                c.food = 100;
                print($"Start:\n{c.stats}");
            }
        }

        float roundTimer = 0;
        private void Update() {
            roundTimer += DeltaTime;
            spawnCounter += DeltaTime;
            if (spawnCounter >= foodSpawnRate) {
                SpawnFood();
                spawnCounter = 0;
            }
            foreach (var c in colonies) {
                c.Update();
            }

            int dead = 0;
            int total = 0;
            foreach (var c in colonies) {
                if (!c.alive) {
                    dead++;
                }
                total += c.population;
                if (total> 300) {
                    dead = 10;
                }
                if (c.population > 200) {
                    dead = 10;
                }
            }

            if (dead >= colonies.Length - 1 || roundTimer >= 120) {
                NewGame();
            }
        }

        void NewGame() {
            roundTimer = 0;
            int best = 0;
            //get winner
            for (int i = 1; i < colonies.Length; i++) {
                if (colonies[i].population > colonies[best].population) {
                    best = i;
                }
            }

            Graph.channel[0].Feed(colonies[best].stats.ga);
            Graph.channel[1].Feed(colonies[best].stats.gb);
            Graph.channel[2].Feed(colonies[best].stats.gc);
            Graph.channel[3].Feed(colonies[best].stats.gd);
            Graph.channel[4].Feed(colonies[best].stats.ge);
            Graph.channel[5].Feed(colonies[best].stats.gf);
            Graph.channel[6].Feed(colonies[best].stats.gg);
            Graph.channel[7].Feed(colonies[best].stats.gh);

            //kill colonies
            for (int i = 0; i < colonies.Length; i++) {
                colonies[i].Kill();
            }

            //set stats
            print($"Winner:\n{colonies[best].stats}");
            for (int i = 0; i < colonies.Length; i++) {
                //winner doen't change
                if (i == best) {
                    continue;
                }

                //copy stats
                var newStats = colonies[best].stats.Copy();
                //mutate stats
                newStats.Mutate();
                //set stats
                colonies[i].stats = newStats;
            }
            for (int i = 0; i < colonies.Length; i++) {
                colonies[i].Initialize();
            }
            foodField.Clear();
            for (int i = 0; i < 10; i++) {
                SpawnFood();
            }
        }
        void BuildMap() {
            hexTile.transform.localScale = new Vector3(mapRadius, mapRadius) * 2 * scale;

            grid = new HexGrid(mapRadius);
            foodField.Initialize(mapRadius);

        }
        void SpawnFood() {
            const int BASE_THRESH = 3; // don't spawn next to base
            const int EDGE_THRESH = 3; // don't spawn next to edge
            const int FOOD_THRESH = 2; // don't spawn next to other food
            var h = GetRandCoord();


            if (h.x == 0 && h.y == 0) {
                return;
            }
            foodField.SpawnFood(h);
            HexCoord GetRandCoord(int depth = 0) {
                var result = grid.RandomCoord();
                if (depth > 100) return new HexCoord(0, 0);

                if (result.ManhattanDist() <= BASE_THRESH) {
                    return GetRandCoord(depth + 1);
                }

                if (result.ManhattanDist() >= mapRadius - EDGE_THRESH) {
                    return GetRandCoord(depth + 1);
                }

                foreach (var hex in new HexGrid(FOOD_THRESH, result)) {
                    if (foodField[hex] != null) {
                        return GetRandCoord(depth + 1);
                    }
                }

                return result;
            }
        }
        public Vector3 ToCartesion(HexCoord h) {
            return h.ToCartesian(orientation, scale);
        }
    }
}