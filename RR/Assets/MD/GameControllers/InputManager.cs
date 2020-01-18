using UnityEngine;


namespace MysteryDungeon {
    public static class InputManager {
        public static bool IsDragging { get; private set; }
        public static bool MouseClick { get; private set; }


        public static Vector3 MousePosition { get; private set; }
        public static Vector2 MouseDrag { get; private set; }

        static Vector3 mouseDownPos;

        static float minDragDist = .1f * .1f;
        static Vector3 MouseScreenPos;

        static Camera _mainCam;
        static Camera mainCamera => _mainCam ?? (_mainCam = Camera.main);

        public static void Update() {
            MouseClick = false;

            if (Input.GetMouseButtonDown(0)) {
                mouseDownPos = MousePosition;
            }

            if (Input.GetMouseButton(0)) {
                if (IsDragging) {
                    MouseDrag = mainCamera.ScreenToWorldPoint(Input.mousePosition) - mainCamera.ScreenToWorldPoint(MouseScreenPos);
                }
                else {
                    var d = MouseWorldPos() - mouseDownPos;
                    if (d.sqrMagnitude >= minDragDist) {
                        IsDragging = true;
                        MouseDrag = d;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (!IsDragging) {
                    MouseClick = true;
                }
                IsDragging = false;
            }

            MousePosition = MouseWorldPos();
            MouseScreenPos = Input.mousePosition;
        }


        static Vector3 MouseWorldPos() {
            //why do I need to SetZ mouse position? I dont know
            return mainCamera.ScreenToWorldPoint(Input.mousePosition.SetZ(0)).SetZ(0);
        }
        
    }
}