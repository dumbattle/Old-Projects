using UnityEngine;
using System;

namespace MysteryDungeon {
    public partial class GameAction {
        public enum Type {
            None,
            Move
        }

        public static readonly GameAction None = new NONE();

        public Type type { get; protected set; }
        public bool stackable { get; protected set; }

        public event Action OnUpdate;
        public Unit unit;



        public static GameAction Move(Unit u, HexCoord direction) {
            return new MOVE(u, direction);
        }


        public bool Update() {
            OnUpdate?.Invoke();
            var done =  AnimUpdate();
            if (done) {
                OnUpdate = null;
            }
            return done;
        }

        public virtual bool AnimUpdate() {
            unit.IdleAnim();
            return true; //is this action complete?
        }

        public GameAction AddEffect(Action a) {
            OnUpdate += a;
            return this;
        }

        private GameAction() { }
        



        public static bool Stackable(GameAction a, GameAction b) {
            if (a == None) {
                return true;
            }
            if (b == None) {
                return true;
            }

            if (a is MOVE ma && b is MOVE mb && ma.unit != mb.unit) {
                return true;
            }

            return false;
        }

        class NONE : GameAction {
            public NONE() {
                type = Type.None;
            }
        }

        public class SPIN : GameAction {
            const int NUM_ROTATIONS = 3;
            const int FRAME_PER_ROTATION = 15;

            float f = 0;
            Unit u;
            Vector3 center;
            public SPIN(Unit u) {
                this.u = u;
                center = Game.main.HexToCartesion(u.pos);
            }

            public override bool AnimUpdate() {
                f++;
                if (f >= NUM_ROTATIONS * FRAME_PER_ROTATION) {
                    u.obj.transform.position = center;
                    return true;
                }
                float r = Mathf.Sin(f / NUM_ROTATIONS / FRAME_PER_ROTATION * Mathf.PI);
                float x = Mathf.Sin(f * 2 * Mathf.PI / FRAME_PER_ROTATION);
                float y = Mathf.Cos(f * 2 * Mathf.PI / FRAME_PER_ROTATION);

                u.obj.transform.position = center + new Vector3(x * r, y * r);
                return false;
            }
        }
    }

}