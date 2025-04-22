using UnityEngine;

public class PlantHealth : PlantComponent {
    public float Max { get; private set; }
    public float Current { get; private set; }

    public float waterUsage { get; private set; }


    public PlantHealth(Plant p) : base(p) { }

    public override void InitializeGenes(PlantGenetics genes) {
        Max = genes.HealthGene.Value;
        waterUsage = genes.WaterUsage.Value * EvoSimMain.DELTA_TIME;
        Current = Max / 10f;
        plant.Obj.transform.localScale = new Vector3(Current / Max, Current / Max, Current / Max);
    }

    public bool Update() {
        //use water
        float lackingWater = plant.WaterStorage.Request(waterUsage);

        ChangeHealth(waterUsage / 2f - lackingWater);
        plant.Obj.transform.localScale = new Vector3(Current / Max, Current / Max, Current / Max);
        return Current > 0;
    }
    public bool ChangeHealth(float amnt) {
        if(amnt < 0) {
            return TakeDamage(-amnt);
        }

        RestoreHealth(amnt);
        return true;
    }

    public bool TakeDamage(float amnt) {
        if (amnt < 0) {
            return true;
        }
        Current -= amnt;
        return Current > 0;
    }
    public void RestoreHealth(float amnt) {
        if (amnt < 0) {
            return;
        }

        Current += amnt;

        if (Current > Max) {
            Current = Max;
        }
    }
}
