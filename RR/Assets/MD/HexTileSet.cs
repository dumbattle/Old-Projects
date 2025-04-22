using UnityEngine;

namespace MysteryDungeon {
    [CreateAssetMenu(fileName = "New Tile Set", menuName = "Tile Set/Hex")]
    public class HexTileSet : ScriptableObject {
        //public float height;

        public Sprite floor;
        public Sprite wall;

        float _h = -1;

        private void OnEnable() {
            _h = -1;
        }
        public float Height {
            get {
                if (_h >= 0) {
                    return _h;
                }

                float w = floor.textureRect.width;
                float h = floor.textureRect.height;

                float h2 = w * 2 / HexCoord.ROOT3;
                _h = (h - h2) / h2;
                return _h;
            }
        }
    }
}