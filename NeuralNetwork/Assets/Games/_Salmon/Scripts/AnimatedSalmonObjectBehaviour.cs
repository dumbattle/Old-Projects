using UnityEngine;

namespace Swimming {
    public class AnimatedSalmonObjectBehaviour : SalmonObjectBehaviour {
        [SerializeField] SpriteRenderer sr;
        [SerializeField] FrameEntry[] frames;

        public override void SetWorldPosition(Vector2 pos, int step) {
            transform.position = pos;

            int totalFrameTime = 0;

            foreach (var f in frames) {
                totalFrameTime += f.duration;
            }

            step = step % totalFrameTime;

            foreach (var f in frames) {
                step -= f.duration;
                if (step < 0) {
                    sr.sprite = f.sprite;
                    break;
                }
            }
        }

        [System.Serializable]
        struct FrameEntry {
            public Sprite sprite;
            public int duration;
        }
    }
}