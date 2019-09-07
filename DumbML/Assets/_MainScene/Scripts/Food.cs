using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item {
    public float calories = 10;

    public float lifeTime = 10;

    float timer = 0;

    public override void Update() {
        base.Update();
        timer += MapManager.timeStep;

        if (timer > lifeTime) {
            Destroy(gameObject);
            MapManager.current.SpawnFood();
        }
    }

}
