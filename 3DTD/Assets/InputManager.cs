using System;
using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public static bool Click;
    public int clickFrameThreshold = 10;
    public float dragThreshold = .1f;

    void Start() {
        StartCoroutine(UpdateClick());
    }


    IEnumerator UpdateClick() {
        while (true) {
            if (Input.GetMouseButtonDown(0)) {
                float x = Screen.width;
                float y = Screen.height;

                var startPos = Input.mousePosition;
                startPos = new Vector3(startPos.x / x, startPos.y / y);

                for (int i = 0; i < clickFrameThreshold; i++) {

                    var newPos = Input.mousePosition;
                    newPos = new Vector3(newPos.x / x, newPos.y / y);
                    if ((startPos - newPos).sqrMagnitude > dragThreshold * dragThreshold) {
                        break;
                    }
                    if (Input.GetMouseButtonUp(0)) {
                        Click = true;
                        break;
                    }
                    yield return null;
                }
            }

            yield return null;
            Click = false;
        }
    }

}
