using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlightController : MonoBehaviour {
    public float speed = 10;
    Camera cam;
    float tilt = 0;
    HexCoord currentHex;
    private void Awake() {
        currentHex = (0, 0);
    }
    private void Start() {
        cam = Camera.main;
    }
    void Update() {
        transform.position += transform.forward * speed * Time.deltaTime;
        Turn();
    }

    void Turn() {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        transform.Rotate(Vector3.up, h, Space.World);
        transform.Rotate(Vector3.right, -v);

        float scale = h == 0 ? 2 : 1;
        tilt = Mathf.Lerp(tilt, h * -20, Time.deltaTime * scale);

        cam.transform.localRotation = Quaternion.Euler(0,0,tilt);

        var o = HexTester.current.OppositeOrientation();
        var r = HexTester.current.mapRadius * HexCoord.ROOT3;

        float dist = (currentHex.ToCartesian(o, r) - Position()).magnitude;

        if (dist > HexTester.current.mapRadius * 1.2f) {
            var hex = HexCoord.FromCartesian(Position(), r, o);
            var dir = hex - currentHex;
            HexTester.current.MoveMap(dir);
            currentHex = hex;
        }
        //print($"{hex} {dist}");

    }
    public Vector2 Position() {
        return new Vector2(transform.position.x, transform.position.z);
    }
}