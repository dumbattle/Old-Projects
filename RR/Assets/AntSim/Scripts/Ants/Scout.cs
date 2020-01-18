using UnityEngine;
using System;

namespace AntSim {
    public class Scout : Ant {
        //pooling
        static ObjectPool<Scout> pool;

        static Scout() {
            Func<Scout> f = () => {
                var obj = UnityEngine.Object.Instantiate(ObjHolder.main.scoutObj, Vector3.zero, Quaternion.identity);
                obj.SetActive(true);
                return new Scout(obj);
            };


            pool = new ObjectPool<Scout>(1, f);
        }
        public static Scout Get() {
            var h = pool.Get();
            return h;
        }

        Scout(GameObject obj) : base(obj) { }

        public override void Initialize(AntColony colony) {
            Initialize(colony, colony.stats.scout);

            ai = new ScoutAI(this);
        }

        public override void Die() {
            base.Die();
            pool.Return(this);
        }

    }
}