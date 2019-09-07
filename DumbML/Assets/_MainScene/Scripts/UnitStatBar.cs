using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatBar : MonoBehaviour {
    public LineRenderer lr;


    void Start() {
        if (lr == null) {
            lr = GetComponent<LineRenderer>();
        }
    }


    //void LateUpdate() {
    //    //transform.forward = -(Camera.main.transform.forward);
    //    transform.LookAt(Camera.main.transform);
    //}

    public void Draw (float amount) {
        amount = amount.Clamp(0, 1) * 2 - 1;

        lr.SetPosition(1, new Vector3(-amount, 0, 0));
    }
    
}
