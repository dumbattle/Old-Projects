using System.Collections.Generic;
using UnityEngine;
using DumbML;


namespace Flappy {
    public class Game {
        public static Vector2 gameSize;
        public static Vector2 blockSize;
        public event GameUpdate OnUpdate;
        public event GameUpdate OnReset;

        public Vector2 offset;
        public float speed = .1f;
        public Bird bird;
        public List<Block> blocks = new List<Block>();

        float spawnTimer = 0;
        public float score = 0;

        Channel scoreChannel;

        public Game(Vector2 offset) {
            scoreChannel = Channel.New("Score");
            scoreChannel.autoYRange = true;
            this.offset = offset;
            var bg = FlappyBird.GetBackground();
            bg.transform.position = offset + gameSize / 2;
            bg.transform.localScale = gameSize;
            bird = new BirdPG(this);
            bird.Reset();
        }

        public void Next() {
            OnUpdate?.Invoke();
            spawnTimer -= .1f;
            score += .1f;
            if (spawnTimer <= 0) {
                CreateBlock();
                spawnTimer = 10;
            }

            bird.Next();
            SetWorldPosition(bird);
            var b = GetClosestBlocks();
        }


        (Block bottom, Block top) GetClosestBlocks() {
            Block top = null;
            Block bottom = null;

            for (int i = 0; i < blocks.Count; i += 2) {
                var x = blocks[i].x;

                if (x > 1 - blockSize.x / 2) {
                    return (blocks[i], blocks[i + 1]);
                }
            }

            return (top, bottom);

        }
        public void BirdCollision () {
            Reset();
        }
        public void Reset() {
            OnReset?.Invoke();
            scoreChannel.Feed(score);
            score = 0;
            spawnTimer = 0;
            bird.Reset();
        }
        void CreateBlock() {
            //get height
            float bottom = Random.Range(1, gameSize.y - 1 - blockSize.y);
            Block b1 = new Block(0, bottom,this);
            Block b2 = new Block(bottom + blockSize.y, gameSize.y,this);
            SetWorldPosition(b1);
            SetWorldPosition(b2);
        }

        public void SetWorldPosition (IGameObject obj) {
            obj.obj.transform.position =  obj.position + offset;
        }


        public Tensor GetState() {
            /// bird height
            /// bird velocity
            /// closest block
            /// block height
            Tensor result = new Tensor(4);
            var closest = GetClosestBlocks();

            result[0] = bird.height / gameSize.y;
            result[1] = bird.velocity / 10;
            result[2] = closest.top.x / gameSize.x;
            result[3] = closest.bottom.top / gameSize.y;
            return result;
        }
    }
    public delegate void GameUpdate();

    public interface IGameObject {
        GameObject obj { get; }
        Vector2 position { get; }
    }
}