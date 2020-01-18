using UnityEngine;


namespace MysteryDungeon {
    [CreateAssetMenu(fileName = "New Unit Animator", menuName = "Mystery Dungeon/Unit Animator")]
    public class UnitAnimator : ScriptableObject {
        public SpriteAnim idleDL, idleDR, idleL, idleR, idleUL, idleUR;

        [Space]
        public SpriteAnim moveDL;
        public SpriteAnim moveDR, moveL, moveR, moveUL, moveUR;


        public Sprite Next(AnimationState state, int direction,int frames) {
            SpriteAnim a = null;

            if (state == AnimationState.walking && direction == 0) {
                return moveUR.Next(frames);
            }
            if (state == AnimationState.walking && direction == 1) {
                return moveR.Next(frames);
            }
            if (state == AnimationState.walking && direction == 2) {
                return moveDR.Next(frames);
            }
            if (state == AnimationState.walking && direction == 3) {
                return moveDL.Next(frames);
            }
            if (state == AnimationState.walking && direction == 4) {
                return moveL.Next(frames);
            }
            if (state == AnimationState.walking && direction == 5) {
                return moveUL.Next(frames);
            }



            if (state == AnimationState.idle && direction == 0) {
                return idleUR.Next(frames);
            }
            if (state == AnimationState.idle && direction == 1) {
                return idleR.Next(frames);
            }
            if (state == AnimationState.idle && direction == 2) {
                return idleDR.Next(frames);
            }
            if (state == AnimationState.idle && direction == 3) {
                return idleDL.Next(frames);
            }
            if (state == AnimationState.idle && direction == 4) {
                return idleL.Next(frames);
            }
            if (state == AnimationState.idle && direction == 5) {
                return idleUL.Next(frames);
            }


            return a?.Next(frames);
        }
    }
}

