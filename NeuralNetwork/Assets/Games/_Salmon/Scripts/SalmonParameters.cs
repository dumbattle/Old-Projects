using UnityEngine;

namespace Swimming {
    [System.Serializable]
    public struct EnvironmentParameters {

        [Min(0)]
        public float swimSpeed;
        [Min(0)]
        public float moveSpeed;
        [Min(1)]
        public int obstaclePeriod;
        public float dieScore;
        public float aliveScore;

    }
    [System.Serializable]
    public class SalmonParameters {
        public EnvironmentParameters envParams;

        [Min(0)]
        public float playerDiameter = 1;

        [Min(1)]
        public float playableHeight = 5;

        [Min(1)]
        public float playableWidth = 5;

        [Range(0, 1)]
        public float swimDecay;
    }
}