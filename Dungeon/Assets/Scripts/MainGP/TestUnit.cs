using System.Collections.Generic;
using UnityEngine;
using LPE.SpacePartition;
using LPE;
using LPE.Triangulation;


namespace MainGP {

    public class TestUnit : Unit {
        GameObject obj;

        public TestUnit(WorldData w, GameObject objSrc, Vector2 pos) : base(w, .25f, 25,1) {
            obj = GameObject.Instantiate(objSrc);
            obj.SetActive(true);
            instanceData. shape.position = pos;
            instanceData.shape.OnShapeUpdate += () => obj.transform.position = new Vector3(instanceData.shape.position.x, 0, instanceData.shape.position.y);
        }

        public override void Update() {
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * .01f;
            UnitController.Move(this, world, input);
            world.unitData.UnitMoved(this);
        }

    }

    public class TestEnemy : Unit {
        List<DelaunayTriangle> aStarResult = new List<DelaunayTriangle>();
        List<Vector2> path = new List<Vector2>();
        GameObject obj;

        public TestEnemy(WorldData w, GameObject objSrc, Vector2 pos) : base(w, .25f, 25, 2) {
            //obj = GameObject.Instantiate(objSrc);
            //obj.SetActive(true);
            instanceData.shape.position = pos;
            //instanceData.shape.OnShapeUpdate += () => obj.transform.position = new Vector3(instanceData.shape.position.x, 0, instanceData.shape.position.y);
        }

        public override void Update() {
            Unit closestEnemy = null;
            float dist = -1;
            bool visable = false;

            path.Clear();
            world.unitData.Iterate(GetClosestEnemy);


            if (closestEnemy == null) {
                return;
            }
            var dir = (path[1] - instanceData.shape.position).normalized * .01f;



            UnitController.Move(this, world, dir);
            world.unitData.UnitMoved(this);



            void GetClosestEnemy(Unit u) {
                // check team
                if (u.instanceData.team == instanceData.team) {
                    return;
                }

                aStarResult.Clear();
                world.environment.triangulation_WH.AStar(instanceData.shape.position, u.instanceData.shape.position, aStarResult, instanceData.shape.radius);
                DelaunayAlgorithms.Funnel(aStarResult, instanceData.shape.position, u.instanceData.shape.position, path, instanceData.shape.radius);

                var l = path.PathLength();
                var v = path.Count == 2;



                if (closestEnemy == null) {
                    closestEnemy = u;
                    dist = l;
                    visable = v;
                }
                else if (visable) {
                    if (v && l < dist) {
                        closestEnemy = u;
                        dist = l;
                    }
                }
                else {
                    if (v) {
                        closestEnemy = u;
                        dist = l;
                        visable = true;
                    }
                    else if (l < dist) {
                        closestEnemy = u;
                        dist = l;
                    }
                }
            }
        }


        public void Gizmos() {
            for (int i = 1; i < path.Count; i++) {
                UnityEngine.Gizmos.DrawLine(
                    new Vector3(path[i - 1].x, 0, path[i - 1].y),
                    new Vector3(path[i].x, 0, path[i].y));
            }
        }
    }

}
