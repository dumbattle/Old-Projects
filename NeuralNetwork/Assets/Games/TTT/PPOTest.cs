using DumbML;
using LPE;


namespace TTT {
    public class PPOTest {
        const int BUFFER_SIZE = 90;
        RingBuffer<Tensor> stateBuffer = new RingBuffer<Tensor>(BUFFER_SIZE);
        RingBuffer<Tensor> probsBuffer = new RingBuffer<Tensor>(BUFFER_SIZE);
        RingBuffer<int> actionBuffer = new RingBuffer<int>(BUFFER_SIZE);
        RingBuffer<float> rewardBuffer = new RingBuffer<float>(BUFFER_SIZE);
        RingBuffer<float> valueBuffer = new RingBuffer<float>(BUFFER_SIZE);

        RingBuffer<float> advBuffer = new RingBuffer<float>(BUFFER_SIZE);
        RingBuffer<float> returnBuffer = new RingBuffer<float>(BUFFER_SIZE);


        ObjectPool<Tensor> statePool = new ObjectPool<Tensor>(() => new Tensor(9));

        int count = 0;
        int trajStart = 0;

         

        public PPOTest(Operation actor, Operation critic) {

        }

        public void AddExperience(Tensor state, Tensor probs, int action, float reward, float value) {
            var s = statePool.Get();
            s.CopyFrom(state);

            var p = statePool.Get();
            p.CopyFrom(state);

            stateBuffer.Add(s);
            probsBuffer.Add(p);
            actionBuffer.Add(action);
            rewardBuffer.Add(reward);
            valueBuffer.Add(value);
            count++;
        }

        public void EndTrajectory(float finalReward) {
            UnityEngine.Debug.Log(trajStart);
            for (int i = trajStart; i < count; i++) {
                float v = i == count - 1 ? finalReward : valueBuffer[i + 1];
                advBuffer.Add(rewardBuffer[i] + .9f * v - valueBuffer[i]);
                returnBuffer.Add(rewardBuffer[0]);
            }
            float adv = finalReward;
            float ret = finalReward;

            for (int i = count - 1; i >= trajStart; i--) {
                adv = advBuffer[i] + adv * .9f * .95f;
                ret = returnBuffer[i] + ret * .9f;
            }

            trajStart = count;
        }

        public void Train() {
            trajStart = 0;
            count = 0;
        }
    }
}
