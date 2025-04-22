using UnityEngine;

public class Map {
    public Orientation orientation => Orientation.horizontal;

    public int Radius { get; private set; }
    public int Scale { get; private set; }

    public PlantGrid PlantGrid { get; private set; }
    public MoistureGrid MoistureGrid { get; private set; }


    public Map(int radius) {
        Radius = radius;
        Scale = 1;
        PlantGrid = new PlantGrid(this);
        MoistureGrid = new MoistureGrid(this);
    }    

    public void Update() {
        PlantGrid.Update();
        MoistureGrid.Update();
    }
    public Vector2 GetWorldPos(HexCoord pos) {
        return pos.ToCartesian(orientation, Scale);
    }
}

public class MoistureGrid : HexMap<MoistureLevel> {
    public Map map { get; private set; }
    public uint FrameCount { get; private set; }

    public MoistureGrid(Map m) : base(m.Radius) {
        map = m;

        foreach (HexCoord h in grid) {
            this[h] = new MoistureLevel(10);
        }
    }

    public void Update() {
        FrameCount++;
        foreach (HexCoord h in grid) {
            this[h].Update();
        }
    }
}
