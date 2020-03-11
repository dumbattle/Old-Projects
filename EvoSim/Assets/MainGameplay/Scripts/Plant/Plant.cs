using UnityEngine;

public class Plant {
    public HexCoord pos { get; private set; }

    public PlantHealth Health { get; private set; }
    public PlantReproduction RePro { get; private set; }
    public PlantWaterStorage WaterStorage { get; private set; }

    public PlantGrid Grid { get; private set; }
    public GameObject Obj { get; private set; }

    public PlantGenetics Genes { get; private set; }

    int rootRadius;

    public Plant (GameObject src) {
        Obj = Object.Instantiate(EvoSimMain.PlantObj());
        Obj.SetActive(false);
        Genes = new PlantGenetics();

        Health = new PlantHealth(this);
        RePro = new PlantReproduction(this);
        WaterStorage = new PlantWaterStorage(this);
    }

    public void SetGenes(PlantGenetics g) {
        Genes = g;
    }

    public void Spawn(HexCoord h, PlantGrid grid) {
        Obj.SetActive(true);
        Grid = grid;

        Obj.transform.position = Grid.map.GetWorldPos(h);

        pos = h;

        Health.InitializeGenes(Genes);
        RePro.InitializeGenes(Genes);
        WaterStorage.InitializeGenes(Genes);
        rootRadius = Genes.RootDist.IntVal;
    }

    public void Update() {
        GetWater();

        bool alive =
            Health.Update() &&
            RePro.Update();

        if (!alive) {
            Die();
            return;
        }
    }

    void GetWater() {
        float drain = Genes.WaterUsage.Value * Genes.WaterDrainMult.Value;
        for (int i = 0; i < rootRadius; i++) {
            float ratio = drain * (rootRadius - i) / rootRadius;

            foreach (var r in new HexGrid(i, pos)) {
                if (Grid.IsInRange(r)) {
                    float h2o = Grid.map.MoistureGrid[r].Drain(EvoSimMain.DELTA_TIME * ratio);
                    WaterStorage.Store(h2o);
                }
            }
        }
    }

    public void Die() {
        Obj.SetActive(false);
        Grid?.RemovePlant(this);
        Object.Destroy(Obj);
    }
}
