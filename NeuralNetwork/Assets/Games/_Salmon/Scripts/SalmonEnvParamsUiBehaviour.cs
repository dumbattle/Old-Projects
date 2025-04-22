using UnityEngine;
using UnityEngine.UI;

namespace Swimming {
    //public class SalmonObstacleListUiBehaviour : SalmonParamsUiBehaviourComponent { 
    
    //}
    public class SalmonEnvParamsUiBehaviour : SalmonParamsUiBehaviourComponent { 
        public Slider scrollSpeedSlider;
        public Slider swimSpeedSlider;
        public Slider obstacleSpeedSlider;
        public Slider aliveScoreSlider;
        public Slider dieScoreSlider;
        public Button applyButton;

        bool applyClicked;
        
        void Awake() {
            applyButton.onClick.AddListener(() => applyClicked = true);
        }
        void LateUpdate() {
            applyClicked = false;
        }


        public override void InitParams(SalmonGame g) {
            scrollSpeedSlider.value = Mathf.InverseLerp(0.1f, 1f, g.parameters.envParams.swimSpeed) * 100;
            swimSpeedSlider.value = Mathf.InverseLerp(0.1f, .5f, g.parameters.envParams.moveSpeed) * 100;
            aliveScoreSlider.value = g.parameters.envParams.aliveScore;
            dieScoreSlider.value = g.parameters.envParams.dieScore;
        }
        public override void UpdateGame(SalmonGame g) {
            if (!applyClicked) { 
                return;
            }
            var currentParams = g.parameters;
            var env = currentParams.envParams;

            bool updated = GetUpdatedParameters(ref env);

            if (updated) {
                currentParams.envParams = env;
            }
        }

        bool GetUpdatedParameters(ref EnvironmentParameters p) {
            bool result = false;
            var scroll = GetDisplayedScrollSpeed();
            var swim = GetDisplayedSwimSpeed();
            var aScore = GetDisplayedAliveScore();
            var dScore = GetDisplayedDieScore();

            if (scroll != p.swimSpeed) {
                result = true;
                p.swimSpeed = scroll;
            }
            if (swim != p.moveSpeed) {
                result = true;
                p.moveSpeed = swim;
            }
            if (aScore != p.aliveScore) {
                result = true;
                p.aliveScore = aScore;
            }
            if (dScore != p.dieScore) {
                result = true;
                p.dieScore = dScore;
            }
            return result;
        }

        float GetDisplayedScrollSpeed() {
            return Mathf.Lerp(0.1f, 1f, scrollSpeedSlider.value  / 100f);
        }
        float GetDisplayedSwimSpeed() {
            return Mathf.Lerp(0.1f, .5f, swimSpeedSlider.value  / 100f);
        }
        // Constant value? if you want to influence density, we can set scroll speed
        //int GetDisplayedObstaclePeriod() {
        //    return Mathf.Lerp(0.1f, .5f, swimSpeedSlider.value  / 100f);
        //}
        float GetDisplayedAliveScore() {
            return aliveScoreSlider.value;
        }
        float GetDisplayedDieScore() {
            return dieScoreSlider.value;
        }
    }
}