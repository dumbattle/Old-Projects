using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MysteryDungeon {
    public class Map : HexMap<Tile> {
        //public const float TILE_HEIGHT = .137f;
        Game game;


        public Map(int radius, Game g) : base(radius) {
            game = g;
            foreach (var h in grid) {
                this[h] = new Tile.Floor(h);
            }

            HexCoord[] walls = new HexCoord[] {
                (0,0),
                (0,1),
                (0,2),
                (0,3),
                (0,4),
                (0,-1),
                (0,-2),
                (0,-3),
                (0,-4),
            };

            foreach (var w in walls) {
                this[w] = new Tile.Wall(w);
            }

            foreach (var h in grid) {
                var obj = MDObjHolder.GetEmptySprite();
                
                var sr = obj.GetComponent<SpriteRenderer>();
                bool isFloor = this[h].type == Tile.Type.floor;

                int height = isFloor ? 0 : 1;

                this[h].height = height;

                var sprite = isFloor ? MDObjHolder.main.tileSet.floor : MDObjHolder.main.tileSet.wall;
                sr.sprite = sprite;

                obj.transform.position = game.HexToCartesionRaw(h) + new Vector3(0, MDObjHolder.main.tileSet.Height * (height - .5f));
                sr.sortingOrder = -h.y;

                game.sorter.Sort(obj, height);
            }
        }


        public HexCoord? GetSpawnableTile() {
            int attempt = 0;
            while (attempt < 100) {
                attempt++;

                var h = grid.RandomCoord();

                if (this[h].type == Tile.Type.floor && this[h].occupant == null) {
                    return h;
                }
            }
            return null;
        }
        public Vector3 GetHeight(HexCoord h) {
            return new Vector3(0, MDObjHolder.main.tileSet.Height * this[h].height);            
        }
        public LinkedList<HexCoord> PathFind(HexCoord start, HexCoord dest, Unit u) {
            if(start == dest) {
                var r = new LinkedList<HexCoord>();
                r.AddFirst(start);
                return r;
            }
            HexMap<int> dist = new HexMap<int>(Radius);
            Queue<HexCoord> queue = new Queue<HexCoord>();


            queue.Enqueue(start);
            dist[start] = 1;

            while (queue.Count > 0) {
                var n = queue.Dequeue();

                var finished = Next(n);
                if (finished) {
                    return GetPath();
                }
            }
            return null;



            LinkedList<HexCoord> GetPath() {
                LinkedList<HexCoord> result = new LinkedList<HexCoord>();
                HexCoord n = dest;
                int safety = 1000;
                result.AddFirst(dest);
                
                while (safety-- > 0) {
                    var nei = n.GetNeighbors()/*.ToList()*/;
                    //System.Comparison<HexCoord> a = (hh1, hh2) => { return (int)(dest.SqrDist(hh1) - dest.SqrDist(hh2)); };
                    //nei.Sort(a);
                    foreach (var h in nei) {
                        if (h == start) {
                            result.AddFirst(h);
                            return result;
                        }

                        if (!IsInRange(h)) {
                            continue;
                        }

                        if (dist[h] == 0 && h != start) {
                            continue;
                        }

                        if (dist[h] < dist[n]) {
                            n = h;
                        }
                    }
                    result.AddFirst(n);
                }
                throw new System.InvalidOperationException("Infinite Loop!");
            }


            bool Next(HexCoord node) {
                var d = dist[node] + 1;
                List<HexCoord> nnn = new List<HexCoord>(6);

                foreach (var n in node.GetNeighbors()) {
                    if (!IsInRange(n)) {
                        continue;
                    }

                    if (n == dest) {
                        if (d < dist[n] || dist[n] == 0) {
                            dist[n] = d;
                        }
                        return true;
                    }

                    if (!this[n].CanUnitEnter(u)) {
                        continue;
                    }
                    if (dist[n] != 0 && d >= dist[n]) {
                        continue;
                    }

                    nnn.Add(n);
                    queue.Enqueue(n);
                    dist[n] = d;
                }
                //System.Comparison<HexCoord> a = (hh1, hh2) => { return (int)(dest.SqrDist(hh1) - dest.SqrDist(hh2)); };
                //nnn.Sort(a);
                //foreach (var n in nnn) {
                //    queue.Enqueue(n);
                //}
                return false;
            }
        }  
    }
}