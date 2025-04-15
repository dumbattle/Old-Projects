using UnityEngine;

namespace Swimming {
    public class PlayerSalmonObjectBehaviour : SalmonObjectBehaviour { 
        public float rotationMultiplier;
        public float rotationSpeed;


        public override void SetWorldPosition(Vector2 pos, int step) {
            var prevPos = transform.position;
            transform.position = pos;

            var deltaY = pos.y - prevPos.y;
            var currentRotation = (transform.rotation.eulerAngles.z + 180) % 360;
            var targetRotation = deltaY * rotationMultiplier + 180;
            var sign = targetRotation - currentRotation;
    
            if (sign == 0) {
                return;
            }

            var nextRotation = currentRotation + rotationSpeed * Mathf.Sign(sign);

            if (Mathf.Sign(targetRotation - nextRotation) == Mathf.Sign(sign)) {
                transform.rotation = Quaternion.Euler(0, 0, nextRotation - 180);
            }
            else {
                transform.rotation = Quaternion.Euler(0, 0, targetRotation - 180);
            }
        }
    }
}