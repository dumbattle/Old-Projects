using UnityEngine;
using System;

namespace AntSim {
    public class Harvester : Ant {
        //pooling
        static ObjectPool<Harvester> pool;
        static Harvester() {
            Func<Harvester> f = () => {
                var obj = UnityEngine.Object.Instantiate(ObjHolder.main.harvesterObj, Vector3.zero, Quaternion.identity);
                obj.SetActive(true);
                return new Harvester(obj);
            };


            pool = new ObjectPool<Harvester>(1, f);
        }
        public static Harvester Get() {
            var h = pool.Get();
            return h;
        }



        public int carryCapacity;
        public int currentCarrying { get; set; }

        new HarvesterAI ai;

        Harvester(GameObject obj) : base(obj) {}

        public override void Initialize(AntColony colony) {
            Initialize(colony, colony.stats.harvester);

            carryCapacity = colony.stats.HarvesterCarryCapacity;
            currentCarrying = 0;
            base.ai = ai = new HarvesterAI(this);
        }

        public override void Die() {
            base.Die();
            pool.Return(this);
        }

        public bool IssueHarvestOrder(HexCoord h, float p) {
            return ai.IssueHarvestOrder(h, p);
        }
    }

    public class CensusTaker : Ant {
        //pooling
        static ObjectPool<CensusTaker> pool;
        static CensusTaker() {
            Func<CensusTaker> f = () => {
                var obj = UnityEngine.Object.Instantiate(ObjHolder.main.censusTakerObj, Vector3.zero, Quaternion.identity);
                obj.SetActive(true);
                return new CensusTaker(obj);
            };

            pool = new ObjectPool<CensusTaker>(1, f);
        }
        public static CensusTaker Get() {
            var h = pool.Get();
            return h;
        }


        public CensusTaker(GameObject obj) : base(obj) { }
        public override void Initialize(AntColony colony) {
            Initialize(colony, colony.stats.harvester);

            ai = ai = new CensusTakerAI(this);
        }
    }
}