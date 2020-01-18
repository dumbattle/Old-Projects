using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public GameObject pivot1,pivot2;

    public float spinSpeed = 10;
    public float scrollSpeed = 10;
    float dist= 10;


     void Start() {        
            pivot2.transform.localEulerAngles = new Vector3(Mathf.Clamp(pivot2.transform.localEulerAngles.x, 300,350),0, 0);
    }

    void Update() {
        if (Input.GetMouseButton(0)) {
            var h = Input.GetAxis("Mouse X") * spinSpeed ;
            pivot1.transform.Rotate(Vector3.up, h,Space.World);

            var v = Input.GetAxis("Mouse Y") * spinSpeed ;
            pivot2.transform.Rotate(Vector3.right, v, Space.Self);
            pivot2.transform.localEulerAngles = new Vector3(Mathf.Clamp(pivot2.transform.localEulerAngles.x, 300,350),0, 0);
        }
        if (Input.GetMouseButton(2)) {
            var h = -Input.GetAxis("Mouse X") * transform.right;
            var v = -Input.GetAxis("Mouse Y") * transform.up;

            var d = h + v;
            d = new Vector3(d.x, 0, d.z);
            pivot1.transform.Translate(d,Space.World);
        }
            var scroll = Input.mouseScrollDelta.y * scrollSpeed ;
        dist -= scroll * .1f;
    }




    void LateUpdate() {
        transform.localPosition = new Vector3(0, 0, dist);
        transform.LookAt(pivot1.transform);
    }
}