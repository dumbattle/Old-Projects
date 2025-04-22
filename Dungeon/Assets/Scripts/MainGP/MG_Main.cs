using System.Collections;
using UnityEngine;


namespace MainGP {
    public class MG_Main : MonoBehaviour {
        public TileSetAsset tileSet;
        public GameObject playerObj;


        GameObject evironmentObject;


        WorldData worldData;
        void Start() {

            var map = GenerateTestMap();
            evironmentObject = tileSet.GenerateMap(map);
            worldData = new WorldData(map);


            playerObj.SetActive(false);
            worldData.unitData.AddUnit(new TestUnit(worldData, playerObj, new Vector2(map.width / 2f, .5f)));
            worldData.unitData.AddUnit(new TestEnemy(worldData, playerObj, new Vector2(map.width / 2f, map.height - 1.5f)));
        }

        void Update() {
            worldData.unitData.UpdateUnits();
        }

        private void OnDrawGizmos() {
            worldData?.DrawGizmos();
        }

        static MapLayout GenerateTestMap() {
            MapLayout result = new MapLayout(10, 10);

            result.data[3, 3] = TileType.hole;
            result.data[4, 3] = TileType.hole;
            result.data[5, 3] = TileType.hole;
            result.data[6, 3] = TileType.hole;

            result.data[2, 7] = TileType.wall;
            result.data[3, 7] = TileType.wall;
            result.data[6, 7] = TileType.wall;
            result.data[7, 7] = TileType.wall;
            return result;

        }
    }



}
