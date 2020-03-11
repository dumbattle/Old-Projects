public class Gene {
    public float Value { get; private set; }
    public float Min { get; private set; }
    public float Max { get; private set; }
    public float MutationStrength { get; private set; }

    public Gene (float initial, float min, float max, float mutStrength) {
        Value = initial;
        Min = min;
        Max = max;
        MutationStrength = mutStrength;
    }
    public void SetGene(float v) {
        if (v <= Max && v >= Min) {
            Value = v;
        }
    }

    public float Mutate() {
        float newVal = Utility.Gaussian(Value, MutationStrength);

        if (newVal > Max) {
            return Max;
        }
        if (newVal < Min) {
            return Min;
        }

        return newVal;
    }
}
