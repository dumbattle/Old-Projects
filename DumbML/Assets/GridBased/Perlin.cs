using UnityEngine;

[CreateAssetMenu(fileName =("New Perlin Noise Map"), menuName =("New Map/Perlin Noise Map"))]
public class Perlin : MapGenerator {
    [Range(1, 1000)]
    public int width = 50;
    [Range(1, 1000)]
    public int height = 50;

    [Range(1, 1000)]
    public float scale = 1;

    [Range(0, 10)]
    public int octaves = 1;
    [Range(0, 1)]
    public float persistance = 1;
    [Range(1, 10)]
    public float lacunarity = 1;

    [Space]
    public int seed;
    public bool useSeed;


    public override Map GenerateMap() {
        return new Map(Noise());
    }

    public float[,] Noise() {
        float[,] map = new float[width, height];
        
        if (scale <= 0) {
            scale = .0001f;
        }

        System.Random prng = useSeed ? new System.Random(seed) : new System.Random(seed = Random.Range(0,int.MaxValue));

        
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }


        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;


                for (int i = 0; i < octaves; i++) {
                    float sampleX = x  / scale * frequency + octaveOffsets[i].x;
                    float sampleY = y / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                map[x, y] = noiseHeight;
            }
        }


        for (int y = 0; y < width; y++) {
            for (int x = 0; x < height; x++) {
                map[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, map[x, y]);
            }
        }
        return map;
    }
}