using LPE.SpacePartition;
using System.Collections.Generic;
using System;
using UnityEngine;


namespace MainGP {
    public class UnitData {
        Partition2D<Unit> _partition;
        LinkedList<Unit> _units = new LinkedList<Unit>();

        public UnitData(MapLayout ml) {
            _partition = new Grid2D<Unit>(new Vector2(0, 0), new Vector2(ml.width, ml.height), new Vector2Int(ml.width, ml.height));
        }

        public void AddUnit(Unit u) {
            _units.AddLast(u);
            _partition.Add(u, u.instanceData.shape.AABB());
        }

        public void UpdateUnits() {
            var n = _units.First;

            while(n != null) {
                n.Value.Update();
                n = n.Next;
            }
        }

        public void UnitMoved(Unit u) {
            _partition.UpdateItem(u, u.instanceData.shape.AABB());
        }
        public void Iterate(Action<Unit> a) {
            var n = _units.First;
            while (n != null) {
                a(n.Value);
                n = n.Next;
            }
        }

        public void DrawGizmos() {
            Gizmos.color = Color.blue; 
            foreach (var u in _units) {
                Gizmos.DrawWireSphere(new Vector3(u.instanceData.shape.position.x, .001f, u.instanceData.shape.position.y), u.instanceData.shape.radius);

                if (u is TestEnemy e) {
                    e.Gizmos();
                }
            }
        }
    }
}
