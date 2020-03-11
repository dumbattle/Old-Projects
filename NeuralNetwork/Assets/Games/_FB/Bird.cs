using UnityEngine;
using System.Collections.Generic;

namespace Flappy {
    public abstract class Bird : IGameObject {
        public GameObject obj { get;}
        public Vector2 position { get => new Vector2(1, height); }

        public float height;
        public float velocity = 0;
        public Game g;


        public Bird(Game g) {
            obj = FlappyBird.GetBird();
            this.g = g;
        }

        public virtual void Next() {
            velocity -= .1f;
            height += velocity / 10f;
            if (height <= 0 || height >= Game.gameSize.y) {
                g.BirdCollision();
            }
        }

        public virtual void Reset() {
            velocity = 0;
            height = Game.gameSize.y / 2;
        }

        public void Jump() {
            velocity = 2;
        }
    }
}