using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;


namespace DumbML {
    public class Gradients {
        static ProfilerMarker profile = new ProfilerMarker("Gradients.Reset");
        Dictionary<Operation, Tensor> grad = new Dictionary<Operation, Tensor>();
        public readonly Operation[] keys;

        public Tensor this[Operation key] {
            get {
                return grad[key];
            } 
        }


        public Gradients(params Operation[] wrt) {
            foreach (var op in wrt) {
                if (!grad.ContainsKey(op)) {
                    grad.Add(op, new Tensor(op.shape));
                }
            }
            keys = grad.Keys.ToArray();
        }
        public Gradients(IEnumerable<Operation> wrt) {
            foreach (var op in wrt) {
                if (!grad.ContainsKey(op)) {
                    grad.Add(op, new Tensor(op.shape));
                }
            }
            keys = grad.Keys.ToArray();
        }
        public void Reset() {
            profile.Begin();
            //var keys = grad.Keys;

            foreach (var k in keys) {
                grad[k].SetValuesToZero();
            }
            profile.End();
        }
        public bool Contains(Operation op) {
            return grad.ContainsKey(op);
        }

    }
}