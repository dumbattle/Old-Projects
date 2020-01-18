using UnityEngine;
using System;
using System.Collections.Generic;

namespace MysteryDungeon {    
    public abstract class UnitAI {        
        public abstract GameAction Next();

        public Unit u;
        public Game game => u.game;



        public static UnitAI GetAi (Unit u, AIType type) {
            switch (type) {
                case AIType.player:
                    return new PlayerControlled(u);
                default:
                    return new Default(u);
            }
        }

        class PlayerControlled : UnitAI {

            HexCoord destination;
            bool destSet = false;


            public PlayerControlled(Unit u) {
                this.u = u;
            }

            public override GameAction Next() {
                if (destSet) {
                    if (u.pos == destination) {
                        destSet = false;
                        return null;
                    }
                    else {
                        var pa = game.map.PathFind(u.pos, destination, u);
                        var dir = pa.First.Next.Value - pa.First.Value;

                        if (game.map[dir + u.pos].occupant != null) {
                            destSet = false;
                            return null;
                        }

                        pa.RemoveFirst();

                        return GameAction.Move(u, dir).AddEffect(HighlightPath(pa, Color.blue)); ;
                    }
                }

                var dest = game.CartesionToHex(InputManager.MousePosition);
                if (!game.map.IsInRange(dest) || game.map[dest].type == Tile.Type.wall) {
                    return null;
                }
                var path = game.map.PathFind(u.pos, dest, u);

                if (path != null) {
                    foreach (var h in path) {
                        if (game.map.IsInRange(h)) {
                            game.tileHighlighter.HighlightTile(h, Color.green);
                        }
                    }
                }

                if (InputManager.MouseClick) {
                    if (dest != u.pos && game.map.IsInRange(dest)) {
                        destSet = true;
                        destination = dest;

                        var h = path.First.Next.Value - path.First.Value;

                        if (game.map[h + u.pos].occupant == null) {
                            path.RemoveFirst();
                            CameraController.main.SetTarget(u.obj, CameraFollowMode.lerp);
                            return GameAction.Move(u, h).AddEffect(HighlightPath(path, Color.blue));
                        }
                    }
                }

                if (Input.GetKey(KeyCode.A)) {
                    return new GameAction.SPIN(u);
                }

                return null;


                Action HighlightPath(LinkedList<HexCoord> p, Color c) {
                    return () => {
                        if (p != null) {
                            foreach (var h in p) {
                                game.tileHighlighter.HighlightTile(h, c);
                            }
                        }
                    };
                }
            }
        }

        class Default : UnitAI {
            HexCoord? destination;

            public Default(Unit u) {
                this.u = u;
            }

            public override GameAction Next() {
                if (u.pos == destination) {
                    destination = null;
                }

                if (destination == null) {
                    destination = game.map.GetSpawnableTile();
                    return destination == null ? GameAction.None : Next();
                }
                               
                var pa = game.map.PathFind(u.pos, destination.Value, u);

                if (pa == null) {
                    Debug.Log($"{u.pos}, {destination}");
                }

                var dir = pa.First.Next.Value - pa.First.Value;

                if (game.map[dir + u.pos].occupant != null) {
                    return GameAction.None;
                }

                return GameAction.Move(u, dir);
            }
        }
    }

    public enum AIType {
        player,
        normal
    }
}