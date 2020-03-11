public class PlantWaterStorage : PlantComponent {
    public float Current { get; private set; }
    public float Max { get; private set; }


    public PlantWaterStorage (Plant p) : base(p) { }

    public override void InitializeGenes(PlantGenetics genes) {
        Max = 10;
        Current = 1;
    }

    /// <summary>
    /// Returns amount lacking
    /// </summary>
    public float Request(float amnt) {
        Current -= amnt;

        if (Current >= 0) {
            return 0;
        }
        float lack = -Current;
        Current = 0;
        return lack;
    }
    /// <summary>
    /// All or None. Returns true if request was fufilled
    /// </summary>
    public bool RequestFull(float amnt) {
        if (Current < amnt) {
            return false;
        }

        Current -= amnt;
        return true;
    }

    /// <summary>
    /// Returns excess amount
    /// </summary>
    public float Store (float amount) {
        Current += amount;

        float excess = Current - Max;

        if (excess < 0) {
            return 0;
        }

        Current = Max;
        return excess;
    }
}