using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public float spinSpeed;
    public float bobSpeed;
    public float bobHeight;

    public GameObject meshObject;

    float bobTimer;

    float x, z, yb;

    void Start() {
        transform.Rotate(0, Random.Range(0, 360), 0);
        x = meshObject.transform.position.x;
        z = meshObject.transform.position.z;
        yb = meshObject.transform.position.y;

        bobTimer = Random.Range(0f, 7f);
    }

    public virtual void Update() {
        meshObject.transform.Rotate(0, spinSpeed, 0);

        bobTimer += MapManager.timeStep * bobSpeed;

        float y = Mathf.Sin(bobTimer) / 2 + .5f + yb;
        meshObject.transform.position = new Vector3(x, y, z);
    }

    //public virtual bool Interact(UnitController unit) { }
}
