using DumbML;


namespace Astroids {
    public static class ModelUtility {


        public static Operation GeneratePolicy() {
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
            op = new FullyConnected(9).Build(op);
            op = new Softmax(op);

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

