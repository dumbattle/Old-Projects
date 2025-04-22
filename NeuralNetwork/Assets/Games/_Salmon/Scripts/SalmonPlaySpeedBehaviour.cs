using UnityEngine;
using UnityEngine.UI;

namespace Swimming {
    public class SalmonPlaySpeedBehaviour : MonoBehaviour { 
        public Button playPauseButton;
        public Image playPauseImage;
        public Button speedUpButton;
        public Image speedUpImage;

        public Sprite playSprite;
        public Sprite pauseSprite;
        public Sprite[] speedUpSprites;


        bool playPausedClicked;
        bool speedUpClicked;
        int speedUpTimer;
        int speedUpIndex;


        public bool paused { get; private set;} = false;
        public bool spedUp { get; private set;} = false;

        void Awake() {
            playPauseButton.onClick.AddListener(() => playPausedClicked = true);
            speedUpButton.onClick.AddListener(() => speedUpClicked = true);
        }
        void Update() {
            if (playPausedClicked) {
                paused = !paused;
                playPauseImage.sprite = paused ? playSprite : pauseSprite;
            }
            if (speedUpClicked) {
                spedUp = !spedUp;
                speedUpImage.sprite = speedUpSprites[0];
                speedUpIndex = 0;
            }

            if (spedUp) {
                speedUpTimer--;
                if (speedUpTimer <= 0) {
                    speedUpTimer = 5;
                    speedUpIndex = (speedUpIndex + 1) % speedUpSprites.Length;

                    speedUpImage.sprite = speedUpSprites[speedUpIndex];
                }
            }
        }

        private void LateUpdate() {
            playPausedClicked = false;
            speedUpClicked = false;
        }
    }
}