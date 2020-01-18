using UnityEngine;
using System;

namespace TD {
    public class Creep : IPoolable {
        static class CreepPool {
            public static ObjectPool<Creep> Pool;

            static CreepPool() {
                Func<Creep> f = () => {
                    var obj = UnityEngine.Object.Instantiate(TDObjectHolder.current.creepObj);
                    obj.SetActive(false);
                    return new Creep(obj);
                };


                Pool = new ObjectPool<Creep>(1, f);
            }
            public static Creep Get(Game game) {
                var c = Pool.Get();
                c.game = game;
                c.Alive = false;
                return c;
            }

            public static void Return(Creep c) {
                Pool.Return(c);
            }
        }

        public static Creep Get(Game g) => CreepPool.Get(g);
        public static void Return(Creep c) => CreepPool.Return(c);

        public bool Active { get; set; }



        public float pos = 0;
        public bool Alive = false;
        public float health;
        public CreepStats stats;

        Game game;
        public GameObject obj;
        HexCoord currentHex;

        Creep(GameObject obj) {
            this.obj = obj;
            stats = new CreepStats();
        }

        public void Update() {
            pos += stats.speed * Game.DELTA_TIME;

            if (pos >= game.map.trackLength - 1) {
                Die();
                return;
            }
            UpdatePosition();
        }

        void UpdatePosition() {
            int p1 = (int)pos;
            int p2 = p1 + 1;

            HexCoord h1 = game.map.track[p1];
            HexCoord h2 = game.map.track[p2];

            obj.transform.position = Vector2.Lerp(game.HexToCartesion(h1), game.HexToCartesion(h2), pos - p1);
            obj.transform.up = game.HexToCartesion(h2) - game.HexToCartesion(h1);
            var newHex = game.CartesionToHex(obj.transform.position);

            if (newHex != currentHex) {
                game.creepMap.Move(this, currentHex, newHex);
                currentHex = newHex;
            }
        }

        public void Spawn(CreepStats stats) {
            this.stats = stats;

            pos = 0;
            obj.SetActive(true);
            Alive = true;
            currentHex = game.map.TrackStart;
            game.creepMap.Add(this, currentHex);
            health = stats.health;

        }

        public void Die() {
            Alive = false;
            obj.SetActive(false);
            game.creepMap.Remove(this, currentHex);
            game.totalCreepDist += pos;
        }

        public void TakeDamage(float damage) {
            health -= damage;
            if (health <= 0) {
                Die();
            }
        }
    }

    public class CreepStats {
        public Gene speed = new Gene(4,1,10, .5f);
        public Gene health = new Gene(10, 5, 50, .5f);
        
        public CreepStats Mutate() {
            CreepStats result = new CreepStats() {
                speed = new Gene(speed),
                health = new Gene(health),
            };

            result.speed.Mutate();
            result.health.Mutate();
            return result;
        }
    }
}