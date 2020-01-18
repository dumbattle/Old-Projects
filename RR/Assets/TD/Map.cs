using System.Collections.Generic;
using UnityEngine;

namespace TD {
    public class Map {
        public HexMap<TileType> map;
        public List<HexCoord> track;
        public int trackLength = 0;

        public HexCoord TrackStart => track[0];
        public HexCoord TrackEnd=> track[trackLength - 1];
        public TileType this[HexCoord h] { get => map[h]; set => map[h] = value; }

        HexGrid grid;
        Game game;
        public Map(Game game) {
            this.game = game;
            grid = new HexGrid(game.radius);
            map = new HexMap<TileType>(game.radius);
            SetTrack();
        }

        public void SetTrack() {
            int attempts = 0;

            track = new List<HexCoord>();

            var head = game.rng.Hex(map.Radius);
            AddTrack(head);

            while (attempts < 100 && trackLength < grid.area / 3f) {
                attempts++;

                var next = HexCoord.Directions[game.rng.Int(0, 6)] + head;

                //don't overlap (or go backwards)
                if (!map.IsInRange(next) || (map[next] == TileType.Track)) {
                    continue;
                }

                //don't touch other parts of track
                bool touching = false;

                foreach (var n in next.GetNeighbors()) {
                    if (n == head) {
                        continue;
                    }
                    if (map.IsInRange(n) && map[n] == TileType.Track) {
                        touching = true;
                        break;
                    }
                }

                if (touching) {
                    continue;
                }

                AddTrack(next);
                head = next;
            }



            void AddTrack(HexCoord h) {

                trackLength++;
                map[h] = TileType.Track;
                track.Add(h);
                var obj = Object.Instantiate(TDObjectHolder.current.trackTile, game.HexToCartesion(h), Quaternion.identity);
                obj.SetActive(true);
            }
        }

        public HexCoord GetTowerLocation() {
            List<HexCoord> possible = new List<HexCoord>();

            foreach (var h in new HexGrid(map.Radius)) {
                if (map[h] != TileType.None) {
                    continue;
                }
                bool touchTrack = false;
                foreach (var n in h.GetNeighbors()) {
                    if (map.IsInRange (n) && map[n] == TileType.Track) {
                        touchTrack = true;
                        break;
                    }
                }
                if (touchTrack) {
                    possible.Add(h);
                }
            }

            return possible[game.rng.Int(0, possible.Count)];
        }
    }
}