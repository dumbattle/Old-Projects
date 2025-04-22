using UnityEngine;


namespace MysteryDungeon {
    public class SpriteController : MonoBehaviour {
        int direction = 0;
        AnimationState state = AnimationState.idle;
        public UnitAnimator anim;

        int f = 0;


        void Update() {
            Next();
            f++;
        }

        void Next() {
            var s = anim.Next(state, direction, f);

            if (s != null) {
                GetComponent<SpriteRenderer>().sprite = s;
            }
        }

        public void SetState(AnimationState s) {
            state = s;
            f = 0;
        }
        public void SetDirection (int d) {
            direction = d;
        }
    }

    public enum AnimationState {
        idle,
        walking
    }
}

