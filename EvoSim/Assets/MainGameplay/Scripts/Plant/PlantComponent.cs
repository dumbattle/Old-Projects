public abstract class PlantComponent {
    public Plant plant { get; private set; }

    public PlantComponent(Plant p) {
        plant = p;
    }

    public abstract void InitializeGenes(PlantGenetics genes);
}
