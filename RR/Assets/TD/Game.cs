using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace TD {
    public class Game {
        public const float DELTA_TIME = 1f / 45f;
        public RNG rng;
        public Map map;
        public CreepMap creepMap;

        public int radius = 5;

        Vector2 center;

        LinkedList<Creep> creeps = new LinkedList<Creep>();
        LinkedList<Tower> towers = new LinkedList<Tower>();
        LinkedList<IEnumerator> invokes = new LinkedList<IEnumerator>();

        public MGStats[] mgstats;
        public CreepStats[] creepStats;

        bool allCreepsSpawned = false;
        public bool Finished = false;

        public float totalCreepDist = 0;

        public Game(Vector2 center) {
            this.center = center;


            creepStats = new[] {
                new CreepStats().Mutate(),
                new CreepStats().Mutate(),
                new CreepStats().Mutate()
            };
            mgstats = new[] {
                new MGStats() { range = 5, cooldown = .01f, damage = .1f },
                new MGStats() { range = 2, cooldown = .01f, damage = .1f },
                new MGStats() { range = 2, cooldown = .01f, damage = .1f }
            };
        }
        public void Update() {
            if (allCreepsSpawned && creeps.Count == 0) {
                Finished = true;
                return;
            }
            HandleInvoke();
            UpdateCreeps();
            Updatetowers();
        }

        void HandleInvoke() {
            LinkedListNode<IEnumerator> node = invokes.First;

            while(node != null) {
                var next = node.Next;
                var finished = !node.Value.MoveNext();

                if (finished) {
                    invokes.Remove(node);
                }

                node = next;
            }
        }
        void UpdateCreeps() {
            LinkedListNode<Creep> c = creeps.First;

            while (c != null) {
                var next = c.Next;
                if (c.Value.Alive) {
                    c.Value.Update();
                }
                else {
                    Creep.Return(c.Value);
                    creeps.Remove(c);
                }
                c = next;
            }
        }
        void Updatetowers() {
            foreach (var t in towers) {
                t.Update();
            }
        }

        public void Start(int seed) {
            rng = new RNG(seed);
            map = new Map(this);
            creepMap = new CreepMap(this);

            totalCreepDist = 0;
            allCreepsSpawned = false;
            Finished = false;

            SpawnCreep();
            SpawnMGS();
        }
        public void Restart() {
            rng = new RNG(rng.seed);

            totalCreepDist = 0;
            allCreepsSpawned = false;
            Finished = false;

            SpawnCreep();
        }
        
        void SpawnCreep() {
            for (int i = 0; i < creepStats.Length; i++) {
                int ii = i;
                Invoke(() => {
                    Creep c = Creep.Get(this);
                    c.Spawn(creepStats[ii]);
                    creeps.AddLast(c);
                }, i);
            }
            Invoke(() => allCreepsSpawned = true, creepStats.Length);
        }
        void SpawnMGS() {

            for (int i = 0; i < 5; i++) {
                var h = map.GetTowerLocation();

                MGTower mg = Tower.GetMGTower(this);
                mg.Spawn(h, mgstats[0]);
                towers.AddFirst(mg);

            }
        }

        public CreepStats[] GetCreepMutations() {
            CreepStats[] cs = new CreepStats[3];

            cs[0] = creepStats[0].Mutate();
            cs[1] = creepStats[1].Mutate();
            cs[2] = creepStats[2].Mutate();

            return cs;
        }

        public void SetCreepStats (CreepStats[] newStats) {
            creepStats = newStats;
        }
        public void AddUpdate(IEnumerator ie) {
            invokes.AddLast(ie);
        }
        public void Invoke(Action a, float time) {
            invokes.AddLast(ie());

            IEnumerator ie() {
                float timer = 0;

                while (timer < time) {
                    timer += DELTA_TIME;
                    yield return null;
                }
                a();
            }
        }        
        public void InvokeRepeating(Action a, float period) {
            invokes.AddLast(ie());

            IEnumerator ie() {
                float timer = 0;

                while (true) {
                    timer += DELTA_TIME;
                    if (timer >= period) {
                        a();
                        timer = 0;
                    }
                    yield return null;
                }
            }
        }


        public Vector2 HexToCartesion(HexCoord h) {
            return h.ToCartesian(TD.ORIENTATION, HexCoord.ROOT3 / 2) + center;
        }
        public HexCoord CartesionToHex(Vector2 position) {
            return HexCoord.FromCartesian(position - center, HexCoord.ROOT3 / 2, TD.ORIENTATION);
        }
    }
}