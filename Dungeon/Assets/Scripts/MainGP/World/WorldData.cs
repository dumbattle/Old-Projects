namespace MainGP {
    public class WorldData {
        public MapLayout mapLayout;
        public EnvironmentData environment;
        public UnitData unitData;


        public WorldData(MapLayout ml) {
            mapLayout = ml;
            environment = new EnvironmentData(ml);
            unitData = new UnitData(ml);
        }


        public void DrawGizmos() {
            environment.DrawGizmos();
            unitData.DrawGizmos();
        }
    }
}
