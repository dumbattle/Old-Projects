using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MovementModule {
    Vector3 prevPosition;
    Collider2D[] collisionDummy = new Collider2D[1];
    ContactFilter2D filter;
    float radius;
    protected override void Initialize() {
        base.Initialize();
        filter = new ContactFilter2D();
        filter.layerMask = 1 << 8;
        filter.useTriggers = true;
        radius = collider.bounds.extents.x;
    }
    void FixedUpdate() {
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");

        if (x != 0 || y != 0) {
            Vector2 dir = new Vector2(x, y).normalized;
            Vector2 newPosition = (Vector2)transform.position + dir * speed * Time.fixedDeltaTime;

            int a = Physics2D.OverlapCircleNonAlloc(newPosition, radius, collisionDummy, 1 << 8);
            if (a == 0) {
                rb.MovePosition(newPosition);
            }

            controller.Animation.SetDirection(dir);
        }
    }
}