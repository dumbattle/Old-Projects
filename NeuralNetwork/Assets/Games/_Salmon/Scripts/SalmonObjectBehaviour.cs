using UnityEngine;

namespace Swimming {
    public class SalmonObjectBehaviour : MonoBehaviour { 
        public virtual void SetWorldPosition(Vector2 pos, int step) {
            transform.position = pos;
        }
    }
}