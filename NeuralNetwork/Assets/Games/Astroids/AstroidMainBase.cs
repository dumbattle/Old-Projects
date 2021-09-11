using System.Collections.Generic;
using UnityEngine;
using LPE;


namespace Astroids {
    public abstract class AstroidMainBase : MonoBehaviour {
        [Header("Base")]
        public GameObject background;
        public GameObject astroidSrc;
        public GameObject playerObj;

        protected List<GameObject> activeAstroids = new List<GameObject>();
        protected ObjectPool<GameObject> astroidPool;


        protected void InitEnvironment() {
            background.transform.localScale = new Vector3(Parameters.mapSize.x, Parameters.mapSize.y, 1);
            astroidPool = new ObjectPool<GameObject>(() => Instantiate(astroidSrc));

            astroidSrc.SetActive(false);
        }
    }
}
