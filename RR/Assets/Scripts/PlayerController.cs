using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    Rigidbody rb;
    public GameObject cameraPivot;
    public float speed = 10;
    public float turnSpeed = 1;
    Vector2 mousePos = new Vector2(0, 0);
    public WheelCollider[] Wheels;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        MovePlayer();

        RotatePlayer();
    }

    private void RotatePlayer() {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            transform.Rotate(new Vector3(0, -turnSpeed, 0), Space.Self);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            transform.Rotate(new Vector3(0, turnSpeed, 0), Space.Self);
        }


        if (Input.GetKey(KeyCode.UpArrow)) {
            if (cameraPivot.transform.localEulerAngles.x > 330 || cameraPivot.transform.localEulerAngles.x <= 180) {
                cameraPivot.transform.Rotate(transform.right, -turnSpeed, Space.World);
            }
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            if (cameraPivot.transform.localEulerAngles.x > 180 || cameraPivot.transform.localEulerAngles.x <= 30) {
                cameraPivot.transform.Rotate(transform.right, turnSpeed, Space.World);
            }

        }
    }


    private void MovePlayer() {
        Vector3 direction = new Vector3(0, 0);
        //float s = 0;
        if (Input.GetKey(KeyCode.W)) {
            //s += speed;
            direction += transform.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            direction -= transform.forward;
            //s -= speed;
        }

        //if (s == 0) {

        //    foreach (var w in Wheels) {
        //        w.brakeTorque = 10000;
        //    }
        //}
        //else {

        //    foreach (var w in Wheels) {
        //        w.brakeTorque = 0;
        //        w.motorTorque = s;
        //    }
        //}
        rb.MovePosition(transform.position + direction * Time.deltaTime * speed);
    }
}

