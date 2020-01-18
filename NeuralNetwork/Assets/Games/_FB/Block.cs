using UnityEngine;

namespace Flappy {
    public class Block : IGameObject {
        public GameObject obj { get; }
        public Vector2 position { get => new Vector2(x, (top + bottom) / 2); }

        public float bottom;
        public float top;
        public float x;
        Game g;
        public Block(float bottom, float top, Game g) {
            this.g = g;
            this.bottom = bottom;
            this.top = top;

            obj = FlappyBird.GetBlock();
            obj.transform.localScale = new Vector2(Game.blockSize.x, top - bottom);
            x = Game.gameSize.x - 1;
            g.OnUpdate += Next;
            g.OnReset += Die;
            g.blocks.Add(this);
        }
        void Die() {
            g.OnUpdate -= Next;
            g.OnReset -= Die;
            g.blocks.Remove(this);
            Object.Destroy(obj);
            x = 1000; 
        }
        void Next() {
            x -= g.speed;
            g.SetWorldPosition(this);
            if (x < - Game.blockSize.x) {
                Die();
            }
            if (BirdCollision()) {
                g.BirdCollision();
            }
        }

        bool BirdCollision (){
            if (x < 1 + Game.blockSize.x / 2 && x > 1 - Game.blockSize.x / 2) {
                var birdHeight = g.bird.height;

                if (birdHeight >= bottom && birdHeight <= top) {
                    return true;
                }
            }


            return false;
        }

    }


}