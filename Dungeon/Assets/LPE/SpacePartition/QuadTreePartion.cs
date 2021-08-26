﻿using System.Collections.Generic;
using UnityEngine;
using System;

namespace LPE.SpacePartition {


    public class QuadTreePartion<T> where T : class, IShape2D {
        static ObjectPool<QuadTreePartion<T>> partitionPool;
        static QuadTreePartion() {
            partitionPool = new ObjectPool<QuadTreePartion<T>>(() => new QuadTreePartion<T>());
        }

        //children
        public QuadTreePartion<T> q1 { get; private set; }
        public QuadTreePartion<T> q2 { get; private set; }
        public QuadTreePartion<T> q3 { get; private set; }
        public QuadTreePartion<T> q4 { get; private set; }

        public bool leaf => q1 == null;

        //bounds 
        public Vector2 min { get; private set; }
        public Vector2 max { get; private set; }

        int maxDepth;
        int maxOccupants = 1;

        HashSet<T> _shapes = new HashSet<T>();

        public int ShapeCount() {
            return _shapes.Count;
        }

        public QuadTreePartion() { }

        public void Initialize(Vector2 regionMin, Vector2 regionMax, int maxDepth, int maxOccupants) {
            min = regionMin;
            max = regionMax;

            this.maxDepth = maxDepth;
            this.maxOccupants = maxOccupants;

            //bottom = null;
            //top = null;
            //right = null;
            //left = null;

            _shapes.Clear();
        }

        public void UpdateShape(T s) {
            (Vector2 min, Vector2 max) aabb = s.shape.AABB();
            if (_shapes.Contains(s)) {
                if (Overlap(aabb.min, aabb.max, min, max)) {
                    if (!leaf) {
                        q1.UpdateShape(s);
                        q2.UpdateShape(s);
                        q3.UpdateShape(s);
                        q4.UpdateShape(s);
                    }
                }
                else {
                    RemoveShape(s);
                }
            }
            else {
                if (Overlap(aabb.min, aabb.max, min, max)) {
                    AddShape(s);
                }
            }
        }

        public void AddShape(T s) {
            _shapes.Add(s);

            if (leaf) {
                if (_shapes.Count > maxOccupants && maxDepth > 0) {
                    SubPartition();
                }
            }
            else {
                (Vector2 min, Vector2 max) aabb = s.shape.AABB();
                if (Overlap(aabb.min, aabb.max, q1.min, q1.max)) {
                    q1.AddShape(s);
                }
                if (Overlap(aabb.min, aabb.max, q2.min, q2.max)) {
                    q2.AddShape(s);
                }
                if (Overlap(aabb.min, aabb.max, q3.min, q3.max)) {
                    q3.AddShape(s);
                }
                if (Overlap(aabb.min, aabb.max, q4.min, q4.max)) {
                    q4.AddShape(s);
                }
            }
        }


        public void RemoveShape(T s) {
            if (!_shapes.Contains(s)) {
                return;
            }
            _shapes.Remove(s);

            if (!leaf) {
                q1.RemoveShape(s);
                q2.RemoveShape(s);
                q3.RemoveShape(s);
                q4.RemoveShape(s);
            }
        }

        public void CleanUp() {
            if (leaf) {
                return;
            }

            if (_shapes.Count <= maxOccupants) {
                ReturnChildren();
            }
            else {
                q1.CleanUp();
                q2.CleanUp();
                q3.CleanUp();
                q4.CleanUp();
            }
        }

        void Return() {
            ReturnChildren();
            partitionPool.Return(this);
        }

        private void ReturnChildren() {
            q1?.Return();
            q2?.Return();
            q3?.Return();
            q4?.Return();

            q1 = null;
            q2 = null;
            q3 = null;
            q4 = null;
        }

        public bool IsColliding(T s) {
            var aabb = s.shape.AABB();
            if (_shapes.Count == 0 || !Overlap(aabb.min, aabb.max, min, max)) {
                return false;
            }


            if (leaf) {
                foreach (var s2 in _shapes) {
                    if (s2 == s) {
                        continue;
                    }

                    if (s.shape.CheckCollision(s2.shape)) {
                        return true;
                    }
                }
            }
            else {
                return q1.IsColliding(s) || q2.IsColliding(s) || q3.IsColliding(s) || q4.IsColliding(s);
            }

            return false;
        }
        public void GetOverlap(IShape2D s, List<T> results) {

            if (_shapes.Count == 0) {
                return;
            }


            (Vector2 min, Vector2 max) aabb = s.shape.AABB();
            if (leaf || _shapes.Count <= 5) {
                foreach (var s2 in _shapes) {
                    if (s2 == s) {
                        continue;
                    }
                    var a2 = s2.shape.AABB();
                    if (Overlap(a2.min, a2.max, aabb.min, aabb.max) && s.shape.CheckCollision(s2.shape)) {
                        if (!results.Contains(s2)) {
                            results.Add(s2);
                        }
                    }
                }
            }
            else {
                if (Overlap(aabb.min, aabb.max, q1.min, q1.max)) {
                    q1.GetOverlap(s, results);
                }
                if (Overlap(aabb.min, aabb.max, q2.min, q2.max)) {
                    q2.GetOverlap(s, results);
                }
                if (Overlap(aabb.min, aabb.max, q3.min, q3.max)) {
                    q3.GetOverlap(s, results);

                }
                if (Overlap(aabb.min, aabb.max, q4.min, q4.max)) {
                    q4.GetOverlap(s, results);
                }
            }
        }

        void SubPartition() {
            q1 = partitionPool.Get();
            q2 = partitionPool.Get();
            q3 = partitionPool.Get();
            q4 = partitionPool.Get();

            float midY = (min.y + max.y) / 2;
            float midX = (min.x + max.x) / 2;

            Vector2 center = (min + max) / 2;
            q1.Initialize(center, max, maxDepth - 1, maxOccupants);
            q2.Initialize(
                    new Vector2(min.x, midY),
                    new Vector2(midX, max.y), maxDepth - 1, maxOccupants);
            q3.Initialize(min, center, maxDepth - 1, maxOccupants);
            q4.Initialize(
                    new Vector2(midX, min.y),
                    new Vector2(max.x, midY), maxDepth - 1, maxOccupants);

            foreach (var s in _shapes) {
                var a = s.shape.AABB();
                if (Overlap(a.min, a.max, q1.min, q1.max)) {
                    q1.AddShape(s);
                }
                if (Overlap(a.min, a.max, q2.min, q2.max)) {
                    q2.AddShape(s);
                }
                if (Overlap(a.min, a.max, q3.min, q3.max)) {
                    q3.AddShape(s);
                }
                if (Overlap(a.min, a.max, q4.min, q4.max)) {
                    q4.AddShape(s);
                }
            }
        }

        public void OnDrawGizmos() {
            if (leaf) {
                //Gizmos.color = Color.black;
                //Vector2 center = (min + max) / 2;
                //foreach (var t in AdjacentTop()) {
                //    Gizmos.DrawLine(
                //        center,
                //        (t.min + t.max) / 2);
                //}
                //foreach (var t in AdjacentRight()) {
                //    Gizmos.DrawLine(
                //        center,
                //        (t.min + t.max) / 2);
                //}
                return;
            }

            if (_shapes.Count == 0) {
                return;
            }
            q1?.OnDrawGizmos();
            q2?.OnDrawGizmos();
            q3?.OnDrawGizmos();
            q4?.OnDrawGizmos();

            Gizmos.color = Color.blue;

            Gizmos.DrawLine(
                new Vector2((min.x + max.x) / 2, min.y),
                new Vector2((min.x + max.x) / 2, max.y));

            Gizmos.DrawLine(
                new Vector2(min.x, (min.y + max.y) / 2),
                new Vector2(max.x, (min.y + max.y) / 2));
        }

        static bool Overlap(Vector2 mina, Vector2 maxa, Vector2 minb, Vector2 maxb) {
            bool result =
                mina.x <= maxb.x &&
                mina.y <= maxb.y &&
                maxa.x >= minb.x &&
                maxa.y >= minb.y;
            return result;
        }
    }


}
