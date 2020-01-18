using System.Collections.Generic;
using UnityEngine;


public class Gene {
    public float Value { get; private set; }
    public float Min { get; private set; }
    public float Max { get; private set; }
    public float MutationStrength { get; private set; }

    public Gene (float min, float max, float mutationStrength) {
        Value = Random.Range(min, max);
        Min = min;
        Max = max;
        MutationStrength = mutationStrength;
    }
    public Gene(float value, float min, float max, float mutationStrength) {
        Value = value;
        Min = min;
        Max = max;
        MutationStrength = mutationStrength;
    }

    public Gene(Gene src) {
        Value = src.Value;
        Min = src.Min;
        Max = src.Max;
        MutationStrength = src.MutationStrength;
    }

    public void Mutate() {
        Value = Mathf.Clamp(Value + Random.Range(-MutationStrength, MutationStrength), Min, Max);
    }

    public static implicit operator float(Gene g) {
        return g.Value;
    }
}