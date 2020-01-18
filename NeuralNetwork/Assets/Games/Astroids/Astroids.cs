using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroids : MonoBehaviour {
    public Vector2Int mapSize;
    public float spawnInterval = .3f;

    AstroidGame game;
    float spawnTimer = 0;
    Map2D map;
    bool isPlaying = false;
    LinkedList<Astroid> astroids = new LinkedList<Astroid>();

    void Start() {
        isPlaying = true;
        map = new Map2D(mapSize);
        game = new AstroidGame(astroids, map);
    }

    void Update() {
        UpdateAstroids();
        UpdateGames();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnInterval) {
            spawnTimer = 0;
            SpawnAstroid();
        }


    }

    void OnDrawGizmos() {
        if (map == null) {
            return;
        }
        //DrawGrid();
        //DrawOutline();
        DrawAstroids();
        DrawPlayers();

        void DrawGrid() {
            Gizmos.color = Color.yellow;
            for (int x = 0; x < mapSize.x; x++) {
                for (int y = 0; y < mapSize.y; y++) {
                    Gizmos.DrawWireCube(new Vector2(x, y), Vector3.one);
                }
            }
        }
        void DrawOutline() {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube((Vector2)mapSize / 2, (Vector2)mapSize);
        }
        void DrawAstroids() {
            foreach (var a in astroids) {
                Gizmos.color = Color.red;
                if (a.partition != null) {
                    Gizmos.DrawCube((Vector2)a.partition.index, new Vector3(1, 1, .1f));
                }

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(a.position, .3f);
            }
        }
        void DrawPlayers() {
            Gizmos.color = Color.green;
            if (game.player.partition != null) {
                Gizmos.DrawCube((Vector2)game.player.partition.index, new Vector3(1, 1, .1f));
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(game.player.position, .3f);
        }
    }

    Vector2 GetPointOnBorder () {
        int side = Random.Range(0, 4);

        switch (side) {
            case 0:
                return new Vector2(Random.Range(-0, mapSize.x), mapSize.y);
            case 1:
                return new Vector2(mapSize.x,Random.Range(-0, mapSize.y));
            case 2:
                return new Vector2(Random.Range(-0, mapSize.x), -0);
            default:
                return new Vector2(-0, Random.Range(-0, mapSize.y ));
        }
    }

    void SpawnAstroid() {
        astroids.AddLast(new Astroid(GetPointOnBorder(), GetPointOnBorder(), 5));
    }
    void UpdateAstroids() {
        var n = astroids.First;

        while (n != null) {
            var a = n.Value;
            a.Update();
            if (a.finished) {
                astroids.Remove(a);
            }
            map.SetPosition(a, a.position);
            n = n.Next;
        }
    }
    void UpdateGames () {
        game.Next();
    }
}

public class AstroidGame {
    public AstroidPlayer player;

    Map2D map;
    LinkedList<Astroid> astroids;

    public AstroidGame(LinkedList<Astroid> astroidList, Map2D map) {
        this.map = map;
        player = new AstroidPlayer((Vector2)map.mapSize / 2f);
        map.SetPosition(player, player.position);
        astroids = astroidList;
    }

    public void Next() {
        //get nearest astroid
        var items = map.GetNearby(player.partition);
        Astroid closest = null;
        float dist = float.PositiveInfinity;

        foreach (var i in items) {
            if(i is Astroid a) {
                float d = (a.position - player.position).sqrMagnitude;
                if (d < dist) {
                    dist = d;
                    closest = a;
                }
            }
        }
        //move away from it
        if (closest == null) {
            return;
        }

        var dir = (player.position - closest.position).normalized;

        player.position += dir * Time.deltaTime * 4;
        map.SetPosition(player, player.position);
    }
}

public class AstroidPlayer : IMapObject {
    public Vector2 position;
    public MapPartition partition { get; set; }

    public AstroidPlayer(Vector2 pos) {
        position = pos;
    }
}

public class Astroid : IMapObject {
    public Vector2 position { get; set; }
    public MapPartition partition { get; set; }

    public Vector2 start, end;
    public float moveSpeed;
    float timer, dist;
    public bool finished;

    public Astroid(Vector2 start, Vector2 end, float moveSpeed) {
        this.start = start;
        this.end = end;
        this.moveSpeed = moveSpeed / 30f;
        dist = (start - end).magnitude;
        timer = 0;
        finished = false;
    }

    public void Update () {
        timer += moveSpeed / dist;
        if (timer > 1) {
            finished = true;
            return;
        }

        position = Vector2.Lerp(start, end, timer);
    }
    
}
public class Map2D {
    MapPartition[,] tiles;
    public Vector2Int mapSize => new Vector2Int(tiles.GetLength(0), tiles.GetLength(1));

    public Map2D(Vector2Int mapSize) {
        tiles = new MapPartition[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                tiles[x, y] = new MapPartition( new Vector2Int(x,y));
            }
        }
    }

    public void SetPosition(IMapObject obj, Vector2 pos) {
        Vector2Int v2i = new Vector2Int((int)pos.x, (int)pos.y);
        float dx = pos.x - v2i.x;
        float dy = pos.y - v2i.y;

        int xMode = dx > .5f ? 1 : 0;
        int yMode = dy > .5f ? 1 : 0;
        int x = Mathf.Clamp(v2i.x + xMode, 0, tiles.GetLength(0) - 1);
        int y = Mathf.Clamp(v2i.y + yMode, 0, tiles.GetLength(1) - 1);

        MapPartition closestTiles = tiles[x, y];

        obj.partition?.Remove(obj);
        obj.partition = (closestTiles);
        obj.partition.Add(obj);
    }

    public List<IMapObject> GetNearby (MapPartition partition) {
        List<IMapObject> result = new List<IMapObject>();

        List<MapPartition> par = new List<MapPartition>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                var v = partition.index + new Vector2Int(x, y);

                if (v.x >= 0 && v.x < mapSize.x && v.y >=0 && v.y < mapSize.y) {
                    par.Add(tiles[v.x, v.y]);
                }
            }
        }

        foreach (var p in par) {
            foreach (var item in p.items) {
                result.Add(item);
            }
        }

        return result;
    }
}
public class MapPartition {
    public LinkedList<IMapObject> items = new LinkedList<IMapObject>();
    public Vector2Int index;
    public MapPartition(Vector2Int index) {
        this.index = index;
    }

    public void Add (IMapObject item) {
        if (items.Contains(item)) {
            return;
        }
        items.AddFirst(item);
    }
    public void Remove(IMapObject item) {
        items.Remove(item);
    }
    public bool Contains(IMapObject item) {
        return items.Contains(item);
    }
}
public interface IMapObject {
    MapPartition partition { get; set; }

}

