using UnityEngine;

namespace Swimming {
    public class SalmonParamsUiManagerBehaviour : MonoBehaviour {
        [SerializeField] SalmonPlaySpeedBehaviour playSpeedUI;
        [SerializeField] SalmonParamsUiBehaviourComponent envParams;
        SalmonParamsUiBehaviourComponent currentOpenMenu;


        public bool paused => playSpeedUI.paused;
        public bool spedUp => playSpeedUI.spedUp;
        private void Awake() {
            currentOpenMenu = envParams;
        }

        public void Init(SalmonGame g) {
            envParams.InitParams(g); 
        }
        public void UpdateParams(SalmonGame g) {
            currentOpenMenu?.UpdateGame(g);
        }
    }
}