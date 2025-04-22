using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static UnitController selected;

    void LateUpdate() {
        if(selected != null) {
            transform.position = selected.transform.position;
        }

        
    }
}
