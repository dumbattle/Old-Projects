using UnityEngine;

[CreateAssetMenu(fileName = "New Cellular Automata Map", menuName = "New Map/ Cellular Automata")]
public class CellularAutomataMapGenerator : MapGenerator {
    [Range(1, 1000)]
    public int width = 50, height = 50;
    [Range(0, 10)]
    public int buffer = 1;

    [Range(0, 1)]
    public float fillScale = .5f;

    [Range(0, 20)]
    public int smoothCount = 3;

    [Range(0, 1)]
    public float minFillThreshold = .3f;

    [Range(0, 1)]
    public float maxFillThreshold = .7f;

    [Space]
    public int maxRooms = 1;
    public int minRoomSize = 50;




    public override Map GenerateMap() {
        return GenerateMap(minFillThreshold);
    }

    protected virtual Map GenerateMap(float minFillThreshold) {
        ProtoMap map = ProtoMap.CellularAutomata(width, height, fillScale, smoothCount, buffer);


        if (maxRooms >0) {
            map.ClearSmallerRegions(maxRooms);
        }

        if (minRoomSize > 1) {
            map.ClearSmallRegions(minRoomSize);
        }

        float fillAmount = map.FillAmount();
        if (fillAmount < minFillThreshold) {
            return GenerateMap(minFillThreshold - .01f);
        }else {
            Debug.Log((fillAmount * 100).ToString("N0") + "% filled");
        }

        return map.ToMap();
    }

}
