using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TD {
    public abstract class Tower : IPoolable {
        static ObjectPool<MGTower> mgPool;


        public static MGTower GetMGTower(Game game) {
            var mg = mgPool.Get();
            mg.game = game;
            return mg;
        }

        public static void Return(MGTower mg) {
            mgPool.Return(mg);
        }


        static Tower() {
            Func<MGTower> f = () => {
                var obj = UnityEngine.Object.Instantiate(TDObjectHolder.current.MGTowerObj);
                obj.SetActive(false);
                return new MGTower(obj);
            };


            mgPool = new ObjectPool<MGTower>(1, f);
        }

        public bool Active { get; set; }
        public abstract void Update();
    }

    
    public class MGTower : Tower {
        public Game game;

        GameObject obj;
        List<HexCoord> visibleTiles;

        MGStats stats;
        float coolDownTimer = 0;

        public MGTower(GameObject obj) {
            this.obj = obj;
        }

        
        public override void Update() {
            if (!Active) {
                return;
            }
            coolDownTimer -= Game.DELTA_TIME;

            var target = GetTarget();

            if (target == null) {
                return;
            }
            ShootAtTarget(target);
        }

        public void Spawn (HexCoord h, MGStats stats) {
            this.stats = stats;
            //check tile is valid
            if (game.map[h] != TileType.None) {
                throw new ArgumentException($"Cannot spawn MG Tower at {h}. Tile occupied: {game.map[h]}");
            }

            //set tile in map
            game.map[h] = TileType.Tower;

            //set gameobject
            obj.SetActive(true);
            obj.transform.position = game.HexToCartesion(h);
            var o = UnityEngine.Object.Instantiate(TDObjectHolder.current.towerTile, game.HexToCartesion(h), Quaternion.identity);
            o.SetActive(true);

            //get visible tiles
            visibleTiles = new List<HexCoord>();
            foreach (var t in game.map.track) {
                if ((t - h).ManhattanDist() <= stats.range) {
                    visibleTiles.Add(t);
                }
            }
        }

        Creep GetTarget () {
            for (int i = visibleTiles.Count - 1; i >= 0; i--) {
                var l = game.creepMap[visibleTiles[i]];
                if (l.Count == 0) {
                    continue;
                }

                Creep closestToExit = l.First.Value;

                foreach (var c in l) {
                    if (c.pos > closestToExit.pos) {
                        closestToExit = c;
                    }
                }
                return closestToExit;
            }
            
            return null;
        }

        bool ShootAtTarget(Creep c) {
            if (coolDownTimer > 0) {
                return false;
            }
            //look at target
            obj.transform.up = c.obj.transform.position - obj.transform.position;

            //laser beam anim
            LaserBeam.Fire(game, obj.transform.position, c.obj.transform.position, Color.green, 0);
            //apply effects
            c.TakeDamage(stats.damage);
            coolDownTimer = stats.cooldown;
            return c.health <= 0;
        }
    }

    public class MGStats {
        public int range = 2;
        public float cooldown = 1;
        public float damage = 1;
    }

    public class LaserBeam : IPoolable {
        public static ObjectPool<LaserBeam> Pool;

        static LaserBeam() {
            Func<LaserBeam> f = () => {
                var obj = UnityEngine.Object.Instantiate(TDObjectHolder.current.LaserObj);
                obj.SetActive(false);
                return new LaserBeam(obj);
            };

            Pool = new ObjectPool<LaserBeam>(1, f);
        }
        public static LaserBeam Get() {
            return Pool.Get();
        }

        public bool Active { get; set; }
        GameObject obj;

        LaserBeam (GameObject obj) {
            this.obj = obj;
        }

        public static void Fire(Game g, Vector2 start, Vector2 end, Color color, float duration) {
            float width = .05f;
            LaserBeam lb = Get();
            lb.obj.SetActive(true);

            Transform t = lb.obj.GetComponent<Transform>();
            SpriteRenderer sr = lb.obj.GetComponent<SpriteRenderer>();

            var mid = (start + end) / 2;
            var dir = end - start;
            var height = dir.magnitude;

            t.position = mid;
            t.localScale = new Vector3(width, height, 1);
            t.up = dir;


            g.AddUpdate(Decay(duration));
            sr.color = color;
            IEnumerator Decay(float d) {
                float timer = d;
                while (timer > 0) {
                    timer -= Game.DELTA_TIME;
                    sr.color = new Color(color.r, color.g, color.b, timer / d);
                    yield return null;
                }
                lb.obj.SetActive(false);
                Pool.Return(lb);
            }
        }

        
    }
}