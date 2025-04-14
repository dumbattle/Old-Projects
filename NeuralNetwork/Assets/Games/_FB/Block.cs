using LPE;
using UnityEngine;

namespace Flappy {
    public class Block : IGameObject {
        static ObjectPool<Block> pool = new ObjectPool<Block>(() => new Block());

        public static Block Get(float bottom, float top, Game g) {
            var result = pool.Get();
            result.g = g;
            result.bottom = bottom;
            result.top = top;
            result.obj.SetActive(true);
            result.obj.transform.localScale = new Vector2(Game.blockSize.x, top - bottom);
            result.x = Game.gameSize.x - 1;
            g.blocks.Add(result);
            return result;
        }


        public GameObject obj { get; }
        public Vector2 position { get => new Vector2(x, (top + bottom) / 2); }

        public float bottom;
        public float top;
        public float x;
        Game g;

        public Block() { 
            obj = FlappyBird.GetBlock();
        }

        public void Die() {
            g.blocks.Remove(this);
            x = 1000; 
            pool.Return(this);
            obj.SetActive(false);
        }
        public void Next() {
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