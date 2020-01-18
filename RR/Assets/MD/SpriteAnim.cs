using UnityEngine;


namespace MysteryDungeon {

    [CreateAssetMenu(fileName = "New Sprite Anim", menuName = "Mystery Dungeon/Sprite Animation")]
    public class SpriteAnim : ScriptableObject {
        public int framesPerSprite = 1;
        public Sprite[] sprites;

        int length => framesPerSprite * sprites.Length;


        public Sprite Next(int frames) {
            frames %= length;
            if (frames % framesPerSprite != 0) {
                return null;
            }
            int s = frames / framesPerSprite;
            return sprites[s];
        }
    }
}

