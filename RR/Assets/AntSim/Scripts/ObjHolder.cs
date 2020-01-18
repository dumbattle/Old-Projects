using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AntSim {
    public class ObjHolder : MonoBehaviour {
        public static ObjHolder main;

        public GameObject baseObj;
        public GameObject harvesterObj;
        public GameObject scoutObj;
        public GameObject censusTakerObj;


        private void Awake() {
            main = this;
            harvesterObj.SetActive(false);
            scoutObj.SetActive(false);
            censusTakerObj.SetActive(false);
            baseObj.SetActive(false);
        }


    }
}