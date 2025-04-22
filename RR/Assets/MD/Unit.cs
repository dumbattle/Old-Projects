using UnityEngine;
using System;


namespace MysteryDungeon {
    public class Unit : IPoolable {
        static ObjectPool<Unit> pool;        
        static Unit() {
            Func<Unit> f = () => {
                var g = MDObjHolder.GetUnit();
                return new Unit(g);
            };
            pool = new ObjectPool<Unit>(10, f);
        }
        public static Unit Get(Game g) {
            var u = pool.Get();
            u.game = g;
            u.playerControlled = false;
            return u;
        }



        public bool Active { get; set; }
        public bool playerControlled = false;

        public Tile tile;
        public HexCoord pos => tile.position;

        public Game game;
        public GameObject obj;
        public UnitAI ai;

        SpriteController spriteController;
        //Animator anim;


        public Unit(GameObject obj) {
            this.obj = obj;
            spriteController = obj.GetComponent<SpriteController>();
        }

        public void Spawn(HexCoord h) {
            tile = game.map[h];
            tile.SetUnit(this);

            obj.transform.position = game.HexToCartesion(pos);
            obj.SetActive(true);
        }
        public void SetPosition(HexCoord h) {
            tile?.RemoveUnit();
            tile = game.map[h];
            tile.SetUnit(this);
        }



        public void IdleAnim() {
            spriteController.SetState(AnimationState.idle);
        }
        public void MoveAnim() {
            spriteController.SetState(AnimationState.walking);
        }


        public void SetDirection (HexCoord dir) {
            for (int i = 0; i < 6; i++) {
                if(dir == HexCoord.Directions[i]) {
                    spriteController.SetDirection(i);

                }
            }

        }
    }
}

