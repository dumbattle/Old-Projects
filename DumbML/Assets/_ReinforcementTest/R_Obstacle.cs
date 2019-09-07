using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_Obstacle : MonoBehaviour {
    public float speed = 5;

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        transform.Translate(new Vector3(0, -speed * ReinforcementTester.timeStep, 0));

        if (transform.position.y < -10) {
            Destroy(gameObject);
        }
    }
}
