using UnityEngine;
using LPE;


namespace Astroids {
    public class Astroid {
        static ObjectPool<Astroid> _pool = new ObjectPool<Astroid>(()=>new Astroid());
        public static Astroid Get() => _pool.Get();
        public static void Return(Astroid a) => _pool.Return(a);

        Astroid() { }

        public Vector2 pos;
        public Vector2 dir;
        public float size;
        public int dist;
    }
}
