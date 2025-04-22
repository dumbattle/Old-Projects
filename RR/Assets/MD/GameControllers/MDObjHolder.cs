using UnityEngine;


namespace MysteryDungeon {
    public class MDObjHolder : MonoBehaviour {
        public static MDObjHolder main;
        public GameObject unitObj;
        public GameObject highlightObj;
        public GameObject sortingObj;

        public GameObject emptySprite;
        public HexTileSet tileSet;

        private void Awake() {
            main = this;
            emptySprite.SetActive(false);
            unitObj.SetActive(false);
            highlightObj.SetActive(false);
            sortingObj.SetActive(false);
        }

        public static GameObject GetEmptySprite() {
            GameObject g = Instantiate(main.emptySprite);
            g.SetActive(true);
            return g;
        }
        public static GameObject GetUnit() {
            return Instantiate(main.unitObj);
        }
        public static GameObject GetHighlight() {
            return Instantiate(main.highlightObj);
        }


        public static GameObject GetSortingObj() {
            GameObject g = Instantiate(main.sortingObj);
            g.SetActive(true);
            return g;
        }
    }    
}