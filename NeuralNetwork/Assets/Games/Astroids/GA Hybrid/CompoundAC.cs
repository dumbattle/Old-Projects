using DumbML;


namespace Astroids {
    public class CompoundAC : ActorCritic  {
        Operation[] policies;
        public int pCount => policies.Length;


        public CompoundAC(params Operation[] policies) {
            this.policies = policies;
            for (int i = 0; i < policies.Length; i++) {
                policies[i] = new Detached(policies[i]);
            }
        }

        protected override Operation Actor(Operation input) {
            Operation op = new FullyConnected(pCount).Build(input);
            op = new Softmax(op);


            Operation[] a = new[] { op, op, op, op, op, op, op, op, op };
            op = new Stack1D(a);
            op = new Transpose(op, new[] { 1, 0 });

            var p = new Stack1D(policies);

            op = op * p;
            op = new Transpose(op, new[] { 1, 0 });
            op = new ReduceSum(op);
            op = new DivideByConst(op, pCount);
            return op;
        }

        protected override Operation Critic(Operation input) {
            Operation op = new FullyConnected(16, ActivationFunction.Sigmoid).Build(input);
            op = new FullyConnected(1).Build(op);
            return op;
        }

        protected override Operation Input() {

            Operation playerPos = new InputLayer(2).Build().SetName("Player Position Input");          // [2]
            Operation astroidInput = new Placeholder("Astroid Input", -1, Parameters.AstroidDataSize); // [x, a]

            int ksize = 16;
            int vsize = 17;
            int qSize = 3;
            Operation aKey = new FullyConnected(ksize, bias: false).Build(astroidInput); // [x, ksize]
            Operation aVal = new FullyConnected(vsize, bias: false).Build(astroidInput); // [x, vsize]

            Operation[] qInput = new Operation[qSize];
            for (int i = 0; i < qSize; i++) {
                qInput[i] = new FullyConnected(ksize).Build(playerPos);
            }
            Operation q = new Stack1D(qInput);                              // [qsize, ksize]
            var (op, attn) = Attention.ScaledDotProduct(aKey, aVal, q);

            op = new FlattenOp(op);
            op = new Append(playerPos, op);
            op = new FullyConnected(32, ActivationFunction.Sigmoid).Build(op);

            return op;
        }
    }
}
/* Steps
 * Generate population of policies
 * 
 * Evaluate policies using an AC coumpound model
 *   - First train
 *   - Then eval => avg score across multiple games
 *   
 * Next generation of policies
 */

