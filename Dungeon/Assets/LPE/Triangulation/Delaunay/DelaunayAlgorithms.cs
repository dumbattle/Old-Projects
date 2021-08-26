using UnityEngine;
using System.Collections.Generic;
using LPE.Math;

namespace LPE.Triangulation {
    public static class DelaunayAlgorithms {
        static List<DelaunayEdge> portals = new List<DelaunayEdge>();
        public static List<Vector2> Funnel(List<DelaunayTriangle> channel, Vector2 start, Vector2 end, List<Vector2> result = null, float radius = 0) {
            result = result ?? new List<Vector2>();
            var et = channel[channel.Count - 1];
            bool containsEnd = Geometry.InTriangle(end, et.v1.pos, et.v2.pos, et.v3.pos);

            if (!containsEnd) {
                // NOT TESTED
                var e1 = Geometry.NearestPointOnSegment(end, et.e1.v1.pos, et.e1.v2.pos);
                var e2 = Geometry.NearestPointOnSegment(end, et.e2.v1.pos, et.e2.v2.pos);
                var e3 = Geometry.NearestPointOnSegment(end, et.e3.v1.pos, et.e3.v2.pos);

                var ee = e1;
                if ((e2 - end).sqrMagnitude < (ee - end).sqrMagnitude) {
                    ee = e2;
                }
                if ((e3 - end).sqrMagnitude < (ee - end).sqrMagnitude) {
                    ee = e3;
                }
                end = ee;
            }

            portals.Clear();
            for (int i = 0; i < channel.Count - 1; i++) {
                DelaunayTriangle t1 = channel[i];
                DelaunayTriangle t2 = channel[i + 1];

                DelaunayEdge edge = t1.e1 == t2.e1 || t1.e1 == t2.e2 || t1.e1 == t2.e3 ? t1.e1 :
                                    t1.e2 == t2.e1 || t1.e2 == t2.e2 || t1.e2 == t2.e3 ? t1.e2 :
                                    t1.e3 == t2.e1 || t1.e3 == t2.e2 || t1.e3 == t2.e3 ? t1.e3 : null;
                portals.Add(edge);

            }

            result.Add(start);
            if (portals.Count == 0) {
                result.Add(end);
                return result;
            }

            var ind = 0;

            var p = portals[ind];
            var s = start;
            var a = p.v1;
            var b = p.v2;
            var pa = a;
            var pb = b;

            var ra = Geometry.IsClockwise(s, a.pos, b.pos);
            var rb = !ra;


            MainLoop();

            result.Add(end);

            AddRadius();


            return result;

            void MainLoop() {
                var safety = new LoopSafety(1000);
                while (ind < portals.Count && ind >= 0 && safety.Inc()) {
                    ind++;
                    if (ind >= portals.Count) {
                        break;
                    }

                    p = portals[ind];

                    bool aSide = pa == p.v1 || pa == p.v2;

                    DelaunayVertex vnext =
                        aSide
                        ? p.v1 == pa
                            ? p.v2
                            : p.v1
                        : p.v1 == pb
                            ? p.v2
                            : p.v1;

                    if (aSide) {
                        pb = vnext;
                        //advance b
                        // wrong way
                        if (Geometry.IsClockwise(s, b.pos, vnext.pos) != rb) {
                            continue;
                        }

                        // crossover
                        if (Geometry.IsClockwise(s, vnext.pos, a.pos) != rb) {
                            AddVertex(a, s);

                            ind = -1;
                            for (int i = portals.Count - 1; i >= 0; i--) {
                                var val = portals[i];
                                if (val.v1 == a || val.v2 == a) {
                                    break;
                                }
                                ind = i;
                            }

                            if (ind == -1) {
                                break;
                            }

                            p = portals[ind];
                            a = p.v1;
                            b = p.v2;
                            pa = a;
                            pb = b;
                            ra = Geometry.IsClockwise(s, a.pos, b.pos);
                            rb = !ra;
                            continue;
                        }
                        b = vnext;
                    }
                    else {
                        pa = vnext;
                        //advance a

                        // wrong way
                        if (Geometry.IsClockwise(s, a.pos, vnext.pos) != ra) {
                            continue;
                        }

                        // crossover
                        if (Geometry.IsClockwise(s, vnext.pos, b.pos) != ra) {
                            AddVertex(b, s);

                            ind = -1;
                            for (int i = portals.Count - 1; i >= 0; i--) {
                                var val = portals[i];
                                if (val.v1 == b || val.v2 == b) {
                                    break;
                                }
                                ind = i;
                            }

                            if (ind == -1) {
                                break;
                            }

                            p = portals[ind];
                            a = p.v1;
                            b = p.v2;
                            pa = a;
                            pb = b;
                            ra = Geometry.IsClockwise(s, a.pos, b.pos);
                            rb = !ra;
                            continue;
                        }

                        a = vnext;
                    }

                }

                EndIter();

            }

            void EndIter() {
                // one more iteration with end
                //advance b
                // wrong way
                if (Geometry.IsClockwise(s, b.pos, end) != rb) {
                    AddVertex(b, s);


                    ind = -1;
                    for (int i = portals.Count - 1; i >= 0; i--) {
                        var val = portals[i];
                        if (val.v1 == b || val.v2 == b) {
                            break;
                        }
                        ind = i;
                    }

                    if (ind == -1) {
                        return;
                    }

                    p = portals[ind];
                    a = p.v1;
                    b = p.v2;
                    pa = a;
                    pb = b;
                    ra = Geometry.IsClockwise(s, a.pos, b.pos);
                    rb = !ra;
                    MainLoop();
                }

                // crossover
                if (Geometry.IsClockwise(s, end, a.pos) != rb) {
                    AddVertex(a, s);

                    ind = -1;
                    for (int i = portals.Count - 1; i >= 0; i--) {
                        var val = portals[i];
                        if (val.v1 == a || val.v2 == a) {
                            break;
                        }
                        ind = i;
                    }

                    if (ind == -1) {
                        return;
                    }

                    p = portals[ind];
                    a = p.v1;
                    b = p.v2;
                    pa = a;
                    pb = b;
                    ra = Geometry.IsClockwise(s, a.pos, b.pos);
                    rb = !ra;
                    MainLoop();
                }
            }

            void AddVertex(DelaunayVertex v, Vector2 src) {
                bool first = true;
                Vector2 pos = v.pos;
                bool cc = true; ;
                foreach (var e in portals) {
                    Vector2 a;

                    if (e.v1 == v) {
                        a = e.v1.pos;
                    }
                    else if (e.v2 == v) {
                        a = e.v2.pos;

                    }
                    else {
                        continue;
                    }

                    if (first) {
                        first = false;
                        cc = Geometry.IsClockwise(src, a, pos);
                        pos = a;
                    }
                    else {
                        var cc2 = Geometry.IsClockwise(src, a, pos);
                        if (cc == cc2) {
                            pos = a;
                        }
                        else {
                            result.Add(pos);
                            src = pos;
                            pos = a;
                        }
                    }
                }

                s = pos;
                result.Add(s);

            }
        
            void AddRadius() {
                // result => list of vertices
                // portals => list of edges

                var pi = -1;
                int ri = 0;
                    var safety = new LoopSafety(10000);

                while (safety.Inc()) {
                    ri++;
                    if (ri >= result.Count - 1) {
                        break;
                    }
                    Vector2 startV = result[ri - 1];
                    Vector2 endV = result[ri];
                    Vector2 nextV = result[ri + 1];


                    while(safety.Inc()) {
                        pi++;
                        if (pi >= portals.Count) {
                            break;
                        }
                        var p = portals[pi];

                        // hit vertex
                        DelaunayVertex vert = endV == p.v1.pos ? p.v1 : endV == p.v2.pos ? p.v2 : null;

                        bool cc = Geometry.IsClockwise(startV, endV, nextV);

                        if (vert != null) {
                            // temp
                            result.Insert(ri, vert.pos);
                            while (safety.Inc()) {
                                var (NewV, _) = p.v1 == vert ? Geometry.ShortenSegment(p.v1.pos, p.v2.pos, radius) : Geometry.ShortenSegment(p.v2.pos, p.v1.pos, radius);
                                // next portal shares vertex?
                                bool b = pi + 1 < portals.Count && (portals[pi + 1].v1 == vert || portals[pi + 1].v2 == vert);

                                // override previous new vertext
                                bool cc2 = Geometry.IsClockwise(startV, NewV, result[ri]);

                                if (cc2 == cc) {
                                    result[ri] = NewV;
                                    if (b) {
                                        pi++;
                                        p = portals[pi];
                                    }
                                    else {
                                        result.RemoveAt(ri + 1);
                                        break;
                                    }
                                }
                                else {
                                    if (b) {
                                        result.Insert(ri + 1, NewV);
                                        ri++;
                                        pi++;
                                        p = portals[pi];
                                    }
                                    else {
                                        result[ri + 1] = NewV;
                                        ri++;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                        
                        else { 


                            //bool intersecting = Geometry.IsIntersecting(startV, endV, p.v1.pos, p.v2.pos);

                            //if (intersecting) {
                            //    var d2 = Geometry.PointSegmentDistSqr(p.v1.pos, startV, endV);
                            //    if (d2 < radius * radius) {
                            //        var (NewV, _) = Geometry.ShortenSegment(p.v1.pos, p.v2.pos, radius);
                            //        result.Insert(ri, NewV);
                            //        break;
                            //    }

                            //    d2 = Geometry.PointSegmentDistSqr(p.v2.pos, startV, endV);
                            //    if (d2 < radius * radius) {
                            //        var (NewV, _) = Geometry.ShortenSegment(p.v2.pos, p.v1.pos, radius);
                            //        result.Insert(ri, NewV);
                            //        break;
                            //    }
                            //}
                            //else {
                            //    // next edge
                            //    break;
                            //}
                        }

           


                    }
                }
            }

        }

    }
}
