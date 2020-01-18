using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static CameraController main;
    [SerializeField]
    GameObject pivot, destination;

    public CameraFollowMode followMode;

    bool followTarget;
    GameObject target;


    void Awake() {
        main = this;
    }

    void LateUpdate() {
        if (followTarget && target != null) {
            destination.transform.position = target.transform.position;
        }

        if (pivot != null) {
                switch (followMode) {
                case CameraFollowMode.lerp:
                    pivot.transform.position = Vector3.Lerp(pivot.transform.position, destination.transform.position, .1f);
                    break;
                case CameraFollowMode.tight:
                    pivot.transform.position = destination.transform.position;
                    break;
            }
        }
    }



    public void SetPosition(Vector3 pos) {
        destination.transform.position = pos;
        followTarget = false;
    }
    public void SetPosition(Vector3 pos, CameraFollowMode mode) {
        destination.transform.position = pos;
        followMode = mode;
        followTarget = false;
    }



    public void MovePivot(Vector3 direction) {
        destination.transform.position = pivot.transform.position + direction;
        followTarget = false;
    }
    public void MovePivot(Vector3 direction, CameraFollowMode mode) {
        destination.transform.position = pivot.transform.position + direction;
        followMode = mode;
        followTarget = false;
    }



    public void SetTarget (GameObject target) {
        this.target = target;
        followTarget = true;

    }
    public void SetTarget (GameObject target, CameraFollowMode mode) {
        this.target = target;
        followTarget = true;
        followMode = mode;
    }
}

public enum CameraFollowMode {
    lerp,
    tight
}