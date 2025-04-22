using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public static MapManager current;
    public const float timeStep = .1f;

    public MapGenerator mapGenerator;
    public Map map;
    public GameObject waterDeepTile, waterTile, groundTile, grassTile;
    public GameObject chicken;
    public GameObject tree;
    public GameObject foodItem;

    public int numChickens = 5;
    public int numTrees = 25;
    Block[,] BlockMap;

    public Block this[int x, int y] {
        get {
            if (!BlockMap.IsInRange(x, y)) {
                return null;
            }
            else return BlockMap[x, y];
        }
        set {
            BlockMap[x, y] = value;
        }
    }
    public Block this[Point2 p] {
        get {
            return this[p.x, p.y];
        }
        set {
            BlockMap[p.x, p.y] = value;
        }
    }

    private void Awake() {
        current = this;
    }
    void Start() {
        map = mapGenerator.GenerateMap();
        CreateTiles();
        for (int i = 0; i < numTrees; i++) {

            SpawnTrees();
        }
        for (int i = 0; i < numChickens; i++) {
            SpawnChicken();
        }
        for (int i = 0; i < 30; i++) {
            SpawnFood();
        }
    }

    public void CreateTiles() {
        BlockMap = new Block[map.Width, map.Height];

        for (int x = 0; x < map.Width; x++) {
            for (int y = 0; y < map.Height; y++) {

                GameObject tile =
                    map[x, y] < .4f ? 
                    (map[x, y] < .35f ? waterDeepTile : waterTile) : 
                    (map[x, y] < .5f ? groundTile : grassTile);


                GameObject go = Instantiate(tile, new Vector3(x, -.5f, y), Quaternion.identity,transform);
                BlockMap[x, y] = go.GetComponent<Block>();
                BlockMap[x, y].index = new Point2(x, y);
            }
        }
    }

    void SpawnTrees () {
        //get ground tile
        bool keepGoing = true;
        int safety = 0;
        while (keepGoing) {
            safety++;
            if (safety > 1000) {
                throw new System.Exception();
            }
            Block b = BlockMap.Random();
            if (b.ground && !b.water && !b.obstructed) {
                keepGoing = false;

                GameObject c = Instantiate(tree, new Vector3(b.position.x, 0, b.position.z), Quaternion.Euler(0,Random.Range(0,360),0), transform);
                b.obstructed = true;
            }
        }

    }
    void SpawnChicken() {
        //get spawnable tile
        bool keepGoing = true;
        int safety = 0;
        while (keepGoing) {
            safety++;
            if (safety > 1000) {
                throw new System.Exception();
            }
            Block b = BlockMap.Random();
            if (b.ground && !b.obstructed) {
                keepGoing = false;

                GameObject c = Instantiate(chicken, new Vector3(b.position.x, .5f, b.position.z), Quaternion.identity);
                UnitController uc = c.GetComponent<UnitController>();
                uc.currentBlock = b;

                CameraController.selected = uc;
            }
        }
    }

    public void SpawnFood() {
        //get spawnable tile
        bool keepGoing = true;
        int safety = 0;
        while (keepGoing) {
            safety++;
            if (safety > 1000) {
                throw new System.Exception();
            }


            Block b = BlockMap.Random();
            if (b.ground && !b.obstructed && b.fertility > .9f) {
                GameObject food = Instantiate(foodItem, new Vector3(b.position.x, 0, b.position.z), Quaternion.identity);
                Item i = food.GetComponent<Item>();

                b.SetItem(i);
                keepGoing = false;
            }
        }
    }
}
