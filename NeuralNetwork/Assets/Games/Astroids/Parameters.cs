using UnityEngine;
namespace Astroids {
    public static class Parameters {
        public const float PlayerSpeed = .1f;
        public const float AstroidrSpeed = .09f;
        public const int AstroidDataSize = 5; // pos(2)  + dir(2) + size(1)


        public static readonly Vector2 mapSize = new Vector2(10,10);
        public const float astroidSpawnChance = .05f;
        public static readonly Vector2 astroidSize = new Vector2(.5f, .75f);
        public const float playerSize = 1;
    }
}
