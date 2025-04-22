using UnityEngine;

public abstract class TileSetAsset : ScriptableObject {
    public abstract GameObject GenerateMap(MapLayout ml);
}
