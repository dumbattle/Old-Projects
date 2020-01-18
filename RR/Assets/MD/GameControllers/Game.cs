using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace MysteryDungeon {
    public class Game {
        public static Game main;
        public Map map;
        public UnitManager unitManager;
        public TurnManager turnManager;
        public TileHighlighter tileHighlighter;
        public HeightSorter sorter;
        public int radius;




        public Game(int radius) {
            this.radius = radius;
            main = this;
            unitManager = new UnitManager(this);
            turnManager = new TurnManager(this);
            tileHighlighter = new TileHighlighter(this);
            sorter = new HeightSorter(this);
        }




        public void Start() {
            map = new Map(radius, this);
            var p = unitManager.SpawnLast(true);
            unitManager.SpawnLast();
            unitManager.SpawnLast();
            CameraController.main.SetPosition(p.obj.transform.position, CameraFollowMode.tight);
        }


        LinkedList<IEnumerator> invokes = new LinkedList<IEnumerator>();

        public void Update() {
            tileHighlighter.Clear();

            InputManager.Update();

            if (InputManager.IsDragging) {
                var drag = InputManager.MouseDrag;
                CameraController.main.MovePivot(-drag, CameraFollowMode.tight);
            }

            HandleInvoke();
            turnManager.Update();
        }


        void HandleInvoke() {
            LinkedListNode<IEnumerator> node = invokes.First;

            while (node != null) {
                var next = node.Next;
                var finished = !node.Value.MoveNext();

                if (finished) {
                    invokes.Remove(node);
                }

                node = next;
            }
        }

        public void Invoke(Action a, int numFrames) {
            invokes.AddLast(ie());

            IEnumerator ie() {
                int timer = 1;

                while (timer < numFrames) {
                    timer++;
                    yield return null;
                }
                a();
            }
        }


        void SetCamera(Vector3 pos) {
            CameraController.main.SetPosition(Camera.main.ScreenToWorldPoint(pos.SetZ(10)).SetZ(0), CameraFollowMode.tight);
            //CameraController.main.pivot.transform.position = Camera.main.ScreenToWorldPoint(pos.SetZ(10)).SetZ(0);
        }
        

        public Vector3 HexToCartesion(HexCoord h) {
            return h.ToCartesian(MysteryDungeon.orientation, HexCoord.ROOT3 / 2) + (Vector2)map.GetHeight(h);
        }
        public Vector3 HexToCartesionRaw(HexCoord h) {
            return h.ToCartesian(MysteryDungeon.orientation, HexCoord.ROOT3 / 2);
        }
        public HexCoord CartesionToHex(Vector3 v) {
            return HexCoord.FromCartesian(v, HexCoord.ROOT3 / 2, MysteryDungeon.orientation);
        }
    }


    public class HeightSorter {
        public Game game;

        Dictionary<int, GameObject> sortingGroups = new Dictionary<int, GameObject>();

        public HeightSorter(Game g) {
            game = g;
        }

        public void Sort(GameObject obj, int height) {
            if (!sortingGroups.ContainsKey(height)) {
                var sg = MDObjHolder.GetSortingObj();
                sg.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder = height;
                sortingGroups.Add(height, sg);
            }

            var parent = sortingGroups[height];
            obj.transform.parent = parent.transform;
        }


    }
}