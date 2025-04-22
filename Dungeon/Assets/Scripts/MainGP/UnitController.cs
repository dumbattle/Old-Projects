using System.Collections.Generic;
using UnityEngine;
using LPE.SpacePartition;


namespace MainGP {
    public static class UnitController {
        static List<RectangleShape> rects = new List<RectangleShape>();

        public static void Move(Unit u, WorldData w, Vector2 dir) {
            u.instanceData.shape.position += dir;

            var cir = u.instanceData.shape;

            // check collisions
            rects.Clear();
            w.environment.walls.QueryItems(cir.AABB(), rects);
            w.environment.holes.QueryItems(cir.AABB(), rects);


            if (w.environment.borderD.CheckCollision(cir)) {
                rects.Add(w.environment.borderD);
            }
            if (w.environment.borderU.CheckCollision(cir)) {
                rects.Add(w.environment.borderU);
            }
            if (w.environment.borderR.CheckCollision(cir)) {
                rects.Add(w.environment.borderR);
            }
            if (w.environment.borderL.CheckCollision(cir)) {
                rects.Add(w.environment.borderL);
            }

            // get correction
            float cx = 0;
            float cy = 0;
            foreach (var r in rects) {
                var cv = cir.CheckCollisionWithCorrection(r);

                cx = Mathf.Abs(cx) > Mathf.Abs(cv.x) ? cx : cv.x;
                cy = Mathf.Abs(cy) > Mathf.Abs(cv.y) ? cy : cv.y;
            }

            cx *= 1.0001f;
            cy *= 1.0001f;

            if (cx == 0 || cy == 0) {
                // simple case
                cir.position += new Vector2(cx, cy);
                cir.UpdateShape();
            }
            else {
                //check that x-correction is sufficient
                if (CheckX()) {
                    return;
                }
                //check that y-correction is sufficient

                if (CheckY()) {
                    return;
                }
                // use both

                cir.position += new Vector2(cx, 0);
                cir.UpdateShape();

                bool CheckX() {
                    cir.position += new Vector2(cx, 0);

                    bool success = true;

                    foreach (var r in rects) {
                        if (cir.CheckCollision(r)) {
                            success = false;
                            break;
                        }
                    }

                    if (success) {
                        cir.UpdateShape();
                        return true;
                    }
                    return false;
                }
                bool CheckY() {
                    cir.position += new Vector2(-cx, cy);

                    bool success = true;

                    foreach (var r in rects) {
                        if (cir.CheckCollision(r)) {
                            success = false;
                            break;
                        }
                    }

                    if (success) {
                        cir.UpdateShape();
                        return true;
                    }
                    return false;

                }

            }
        }
    }

}
