using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnimation : AnimationModule {
    public Vector2 Forward;

    public override void SetDirection(Vector2 direction) {
        this.direction = direction;

        transform.up = Forward;
        float angle = Vector2.SignedAngle(Forward, direction);
        transform.Rotate(0,0,angle);    
    }
}
