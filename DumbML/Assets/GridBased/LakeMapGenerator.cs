using UnityEngine;

[CreateAssetMenu(fileName = "New Lake Map", menuName = "New Map/ Lake")]
public class LakeMapGenerator : MapGenerator {
    [Range(1, 1000)]
    public int width = 50, height = 50;
    [Range(0, 1)]
    public float expansionScale = .5f;
    public int expansionCount = 10;

    public int numLakes = 1;
    public int buffer = 5;

    public override Map GenerateMap() {
        ProtoMap map = new ProtoMap(width, height, true) {
            edgesAreWalls = false
        };

        Point2[] points = new Point2[numLakes];
        for (int i = 0; i < numLakes; i++) {
            Point2 p = Point2.Random(new Point2(buffer, buffer),  new Point2(width - buffer, height - buffer));
            map[p] = false;
            points[i] = p;
        }


        for (int i = 0; i < expansionCount; i++) {
            map.ExpandWalls(expansionScale);
            map.ExpandFloors(.05f);

        }
        map.Smooth(10);


        return map.ToMap();
    }
}
