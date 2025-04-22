using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Test {
    public class CircleCast_Square_Test : MonoBehaviour {
        public GameObject sqr;
        public GameObject start;
        public GameObject end;

        public Vector2 sqrSize;
        public float radius;

        private void OnDrawGizmos() {


            Gizmos.DrawWireCube(sqr.transform.position, new Vector3(sqrSize.x, sqrSize.y, 0));
            Gizmos.DrawWireSphere(start.transform.position, radius);



            Vector2 dir = end.transform.position - start.transform.position;
            dir.Normalize();
            var t = LPE.Math.Geometry.CircleCast_SquareAxisAligned(start.transform.position, radius, dir, sqr.transform.position, sqrSize.x, sqrSize.y);


            Gizmos.DrawWireSphere(start.transform.position + (Vector3)(dir * t), radius);

        }
    }


}