using UnityEngine;

public class EvoSimMain : MonoBehaviour {
    public const float DELTA_TIME = 1f / 60f;
    static EvoSimMain _main;
    public GameObject plant;
    public GameObject background;

    public int mapSize = 10;
    Map _map;

    private void Awake() {
        _main = this;
    }
    void Start() {
        _map = new Map(mapSize);
        _map.PlantGrid.SpawnPlant((0,0), new Plant(plant));
    }

    void Update() {
        _map.Update();
    }

    void OnDrawGizmos() {
        if (_map == null) {
            return;
        }
        Gizmos.color = Color.blue;

        foreach(HexCoord h in new HexGrid(mapSize)) {
            var ml = _map.MoistureGrid[h];
            Gizmos.DrawSphere(_map.GetWorldPos(h), ml.amount / ml.max / 2);
        }
    }
    public static GameObject PlantObj() {
        return _main.plant;
    }
}
