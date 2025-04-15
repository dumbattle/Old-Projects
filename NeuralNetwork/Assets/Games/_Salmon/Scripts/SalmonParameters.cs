using UnityEngine;

namespace Swimming {
    [System.Serializable]
    public class SalmonParameters {
        [Min(0)]
        public float playerDiameter = 1;

        [Min(1)]
        public float playableHeight = 5;

        [Min(1)]
        public float playableWidth = 5;

        [Min(0)]
        public float swimSpeed;
        [Min(0)]
        public float moveSpeed;
        [Range(0, 1)]
        public float swimDecay;

        [Min(1)]
        public int obstaclePeriod;

        [Header("Score")]
        public float dieScore;
        public float aliveScore;
        public float idleBonus;
    }
}