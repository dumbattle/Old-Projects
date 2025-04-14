using DumbML;

namespace TTT {
    public class Game {
        int[] state = new int[9];
        Tensor stateTensor = new Tensor(9);
        public void Reset() {
            state[0] = 0;
            state[1] = 0;
            state[2] = 0;
            state[3] = 0;
            state[4] = 0;
            state[5] = 0;
            state[6] = 0;
            state[7] = 0;
            state[8] = 0;
        }

        public void MakeMove(int x, int y) {
            MakeMove(ToLinearIndex(x, y));
        }

        public void MakeMove(int i) {
            int player = CurrentPlayer();

            if (state[i] != 0) {
                throw new System.ArgumentException($"Invalid index: ({i}) - Has Value: {state[i]}");
            }

            state[i] = player;
        }

        public int CurrentPlayer() {
            int sum = 0;
            foreach (var i in state) {
                sum += i;
            }

            // i == 0,1 ==> [1, -1]
            return sum * -2 + 1;
        }

        public int Winner() {
            int result = 0;

            result = Check(0, 1, 2);
            if (result != 0) return result;
            result = Check(3, 4, 5);
            if (result != 0) return result;
            result = Check(6, 7, 8);
            if (result != 0) return result;

            result = Check(0, 3, 6);
            if (result != 0) return result;
            result = Check(1, 4, 7);
            if (result != 0) return result;
            result = Check(2, 5, 8);
            if (result != 0) return result;

            result = Check(0, 4, 8);
            if (result != 0) return result;
            result = Check(2, 4, 6);
            if (result != 0) return result;
            return 0;

            int Check(int a, int b, int c) {
                int sum = state[a] + state[b] + state[c];
                if (sum == 3) {
                    return 1;
                }
                if (sum == -3) {
                    return -1;
                }
                return 0;
            }
        }
        public bool Full() {
            foreach (var i in state) {
                if (i == 0) {
                    return false;
                }
            }
            return true;
        }

        public int Tile(int x, int y) {
            return state[ToLinearIndex(x, y)];
        }
        public int Tile(int i) {
            return state[i];
        }

        public Tensor GetStateTensor() {
            stateTensor.value[0] = state[0];
            stateTensor.value[1] = state[1];
            stateTensor.value[2] = state[2];
            stateTensor.value[3] = state[3];
            stateTensor.value[4] = state[4];
            stateTensor.value[5] = state[5];
            stateTensor.value[6] = state[6];
            stateTensor.value[7] = state[7];
            stateTensor.value[8] = state[8];
            return stateTensor;
        }
        
        
        int ToLinearIndex(int x, int y) {
            return x * 3 + y;
        }
    }

}
