using UnityEngine;

namespace TD {
    public class TD : MonoBehaviour {
        public const Orientation ORIENTATION = Orientation.horizontal;
        //public Game game;
        HexMap<Game> games;

        Channel speedGraph, healthGraph;
        void Start() {
            int seed = Random.Range(-999, 999);
            games = new HexMap<Game>(1);
            foreach (var h in new HexGrid(1)) {
                games[h] = new Game(h.ToCartesian(radius: 10));
                games[h].Start(seed);
            }
            speedGraph = Graph.GetChannel("speed");
            healthGraph = Graph.GetChannel("health");
            Graph.SetYRange(0, 51);
            //game = new Game(Vector2.zero);
            //game.Initialize(0);
        }

        void Update() {
            bool finished = true;
            foreach (var g in games) {
                g.Update();
                if (!g.Finished) {
                    finished = false;
                }
            }

            if (finished) {
                Game best = games[(0, 0)];

                //get best
                foreach (var g in games) {
                    if (g.totalCreepDist > best.totalCreepDist) {
                        best = g;
                    }
                }
                print(best.totalCreepDist);
                speedGraph.Feed((best.creepStats[0].speed + best.creepStats[1].speed + best.creepStats[2].speed) / 3);
                healthGraph.Feed((best.creepStats[0].health + best.creepStats[1].health + best.creepStats[2].health) / 3);
                foreach (var g in games) {
                    if (g == best) {
                        g.Restart();
                        continue;
                    }
                    g.SetCreepStats(best.GetCreepMutations());
                    g.Restart();
                }
            }
        }
    }
}