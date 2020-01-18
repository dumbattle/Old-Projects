using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject target;
    public GameObject pivot;

    void LateUpdate() {
        pivot.transform.position =  target.transform.position;
    }
}
