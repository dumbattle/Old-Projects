public class IntGene : Gene {
    public int IntVal => (int)Value;

    public IntGene(float initial, float min, float max, float mutStrength) : base(initial, min, max, mutStrength) { }
}
