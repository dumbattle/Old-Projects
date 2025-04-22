public class MoistureLevel {
    public float max { get; private set; }
    public float amount { get; private set; }

    public MoistureLevel(int max) {
        this.max = max;
        amount = max;
    }

    public void Update() {
        amount += EvoSimMain.DELTA_TIME;
        if (amount > max) {
            amount = max;
        }
    }
    public float Drain (float amnt) {
        if (amount < amnt) {
            float temp = amount;
            amount = 0;
            return temp;
        }

        amount -= amnt;
        return amnt;
    }
}