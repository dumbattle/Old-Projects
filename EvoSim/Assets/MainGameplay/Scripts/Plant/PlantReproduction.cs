using UnityEngine;

public class PlantReproduction :PlantComponent {
    public float SpawnInterval { get; private set; }
    public float SpawnTimer { get; private set; }
    public int SpawnDist { get; private set; }

    float waterUsage = 1;

    public PlantReproduction(Plant p) :base(p){}

    public override void InitializeGenes(PlantGenetics genes) {
        SpawnInterval = genes.SpawnDelay.Value;
        SpawnDist = genes.SpawnDist.IntVal;
    }
    

    public bool Update() {
        SpawnTimer += EvoSimMain.DELTA_TIME;

        if (SpawnTimer >= SpawnInterval && plant.WaterStorage.RequestFull(waterUsage)) {
            HexCoord spawnLocation = new HexGrid(SpawnDist, plant.pos).RandomCoord();

            Plant child = new Plant(plant.Obj);
            child.SetGenes(plant.Genes.Mutate());

            plant.Grid.SpawnPlant(spawnLocation, child);
            Color c = plant.Obj.GetComponent<SpriteRenderer>().color;
            c.r *= Random.Range(.95f, 1.05f);
            c.g *= Random.Range(.95f, 1.05f);
            c.b *= Random.Range(.95f, 1.05f);
            child.Obj.GetComponent<SpriteRenderer>().color = c;
            SpawnTimer = 0;
        }

        return true;
    }
}