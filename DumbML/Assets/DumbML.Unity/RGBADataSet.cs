using UnityEngine;
using DumbML;

namespace DumbML.Unity {

    [CreateAssetMenu(fileName = "New RGBA Data Set", menuName = "DumbML/RGBA Data Set")]
    public class RGBADataSet : ScriptableObject {
        public Sprite[] Images;

        public Tensor[] LoadImages() {
            Tensor[] result = new Tensor[Images.Length];

            for (int i = 0; i < Images.Length; i++) {
                Sprite img = Images[i];
                Texture2D tex = img.texture;
                Tensor t = new Tensor((int)img.textureRect.width, (int)img.textureRect.height, 4);

                var offset = img.textureRect;
                for (int x = 0; x < t.Shape[0]; x++) {
                    for (int y = 0; y < t.Shape[1]; y++) {
                        Color c = tex.GetPixel(x + (int)offset.x, y + (int)offset.y);
                        t[x, y, 0] = c.r;
                        t[x, y, 1] = c.g;
                        t[x, y, 2] = c.b;
                        t[x, y, 3] = c.a;

                    }
                }
                result[i] = t;
            }
            return result;
        }
    }

}