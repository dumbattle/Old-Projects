using UnityEngine;

namespace Swimming {
    [System.Serializable]
    public class Obstacle {
        [Min(0)]
        public Vector2 size;
        public float speed;

        [Min(0)]
        public float minDistFromTop;
        [Min(0)]
        public float minDistFromBottom;
    }
}