using System;
using UnityEngine;

public interface INoise2D {
    float GetValue(float x, float y);
}
[Serializable]
public class Perlin2D : INoise2D {
    public int seed;
    public bool useSeed;

    
    public float scale = 1;
    Vector2 _offsets;
    bool set = false;
    public Vector2 Offsets {
        get {
            if (set) {
                return _offsets;
            }
            System.Random rng;

            if (!useSeed) {
                seed = UnityEngine.Random.Range(-10000000, 10000000);
            }
            rng = new System.Random(seed);

            _offsets = new Vector2(UnityEngine.Random.Range(-100f, 100f), UnityEngine.Random.Range(-100f, 100f));
            set = true;

            return _offsets;
        }
        set {
            _offsets = value;
        }
    }

    public float GetValue(float x, float y) {
        return (Mathf.PerlinNoise(x / scale + Offsets.x, y / scale + Offsets.y));
    }
}

[Serializable]
public class CompoundPerlin : INoise2D {
    public Layer[] layers;
    //float[] values;
    public float GetValue(float x, float y) {
        var values = new float[layers.Length];

        //get values
        for (int i = 0; i < layers.Length; i++) {
            float v = layers[i].perlin.GetValue(x, y);
            float min = layers[i].floor.threshold;

            if (!layers[i].floor.enabled ) {
                values[i] = v;
            }
            else if ( v >= min || min <= 0){
                values[i] = (v + layers[i].floor.maxFloorHeight - min) / (1 + layers[i].floor.maxFloorHeight - min);
            }
            else {
                values[i] = Lerp.InFlatOut(0, layers[i].floor.maxFloorHeight, v / min) / (1 + layers[i].floor.maxFloorHeight - min);
            }

        }
        float result = 0;

        //apply masks 
        for (int i = 0; i < layers.Length; i++) {
            if (layers[i].mask.enabled) {
                int ind = layers[i].mask.index;
                values[i] = values[i] * Lerp.Linear(0,1, values[ind]);
            }
        }
        // add
        for (int i = 0; i < layers.Length; i++) {
            if (layers[i].enable) {
                result += values[i] * layers[i].height;
            }
        }
        return result;
    }
    [Serializable]
    public class Layer {
        public bool enable = true;
        public float height = 1;

        public Perlin2D perlin;

        public Floor floor;
        public Mask mask;


        [Serializable]
        public class Floor {
            public bool enabled = false;
            [Range(0,1)]
            public float threshold = .5f;
            [Range(0,1)]
            public float maxFloorHeight = .1f;
        }
        [Serializable]
        public class Mask {
            public bool enabled = false;
            public int index = 0;
        }
    }
    //void SetValuesArray () {
    //    if (values != null) {
    //        return;
    //    }
    //    values = new float[layers.Length];
    //}
}
public static class Lerp {
    public static float Linear (float min, float max, float t) {
        return min * (1 - t) + max * t;
    }
    public static float In(float min, float max, float t) {
        t = Mathf.Clamp01(t);

        t = t * t;
        return min * (1 - t) + max * t;

    }
    public static float InFlatOut(float min, float max, float t) {
        t = Mathf.Clamp01(t);
        t = t + .5f;
        t = t * t / 2 - .125f;
        return min * (1 - t) + max * t;

    }
}