public class PlantGenetics {
    public Gene HealthGene { get; private set; }
    public Gene SpawnDelay { get; private set; }

    public Gene WaterUsage { get; private set; }
    public Gene WaterDrainMult { get; private set; }

    public IntGene SpawnDist { get; private set; }
    public IntGene RootDist { get; private set; }

    public PlantGenetics() {
        HealthGene = new Gene(7, 5, 25, .2f);
        SpawnDelay = new Gene(3, 1, 5, .1f);

        WaterUsage = new Gene(1, .1f, 3f, .1f);
        WaterDrainMult = new Gene(1.5f, 1, 2, .1f);

        SpawnDist = new IntGene(7, 1, 10, .1f);
        RootDist = new IntGene(7, 1, 10, .1f);
    }
    
    public PlantGenetics Mutate() {
        PlantGenetics result = new PlantGenetics();

        result.HealthGene.SetGene(HealthGene.Mutate());
        result.SpawnDelay.SetGene(SpawnDelay.Mutate());
        result.SpawnDist.SetGene(SpawnDist.Mutate());
        result.RootDist.SetGene(RootDist.Mutate());

        return result;
    }
}