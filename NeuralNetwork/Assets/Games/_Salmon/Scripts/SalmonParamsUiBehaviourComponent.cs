using UnityEngine;

namespace Swimming {
    public abstract class SalmonParamsUiBehaviourComponent : MonoBehaviour { 
        public abstract void InitParams(SalmonGame g);
        public abstract void UpdateGame(SalmonGame g);
    }
}