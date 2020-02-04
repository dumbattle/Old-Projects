using Unity.Profiling;

namespace DumbML {
    public abstract class Optimizer {
        public Gradients grad;
        public virtual ProfilerMarker p { get; }
        public bool IsBuilt { get; protected set; }

        public Optimizer(Gradients grad) {
            InitializeGradients(grad);
        }
        public Optimizer() { }

        public void ZeroGrad() {
            grad.Reset();
        }

        public virtual void InitializeGradients(Gradients g) {
            grad = g;
            IsBuilt = true;
        }

        public virtual void Update() {
            p.Begin();
            int ind = 0;

            for (int i = 0; i < grad.keys.Length; i++) {
                var v = grad.keys[i] as Variable;

                if (v == null) {
                    continue;
                }
                if (!v.Trainable) {
                    ind += grad[v].Size;
                    continue;
                }

                int size = v.Value.Size;
                var gradTensor = grad[v]._value;
                var targetTensor = v.Value._value;


                for (int j = 0; j < size; j++) {
                    targetTensor[j] -= UnpdateSingle(gradTensor[j], ind);
                    ind++;
                }
            }
            p.End();
        }

        public virtual float UnpdateSingle(float g, int i) {
            return g;
        }
        protected float[] NewContainer(float init = 0) {
            int count = 0;
            foreach (var k in grad.keys) {
                count += grad[k].Size;
            }

            if (init == 0) {
                return new float[count];
            }else {
                var result = new float[count];
                for (int i = 0; i < count; i++) {
                    result[i] = init;
                }
                return result;
            }
        }
    }

    public class SGD : Optimizer {
        static ProfilerMarker profile = new ProfilerMarker("SGD.Update");
        public override ProfilerMarker p => profile;
        float[] m;
        float lr;
        float momentum;

        public SGD(Gradients g, float lr = .01f, float momentum = .9f) : base(g) {
            this.lr = lr;
            this.momentum = momentum;
        }
        public SGD(float lr = .01f, float momentum = .9f) : base() {
            this.lr = lr;
            this.momentum = momentum;
        }
        public override void InitializeGradients(Gradients g) {
            if (IsBuilt) return;

            base.InitializeGradients(g);
            m = NewContainer();
        }
        public override float UnpdateSingle(float g, int i) {            
            return m[i] = m[i] * momentum + g * (1 - momentum) * lr; 
        }
    }

    public class RMSProp : Optimizer {
        static ProfilerMarker profile = new ProfilerMarker("RMSProp.Update");
        public override ProfilerMarker p => profile;
        float[] v;
        float lr;
        float gamma;

        public RMSProp(Gradients g, float lr = .001f, float gamma = .999f) : base(g) {
            this.lr = lr;
            this.gamma = gamma;
        }
        public RMSProp(float lr = .001f, float gamma = .999f) : base() {
            this.lr = lr;
            this.gamma = gamma;
        }

        public override void InitializeGradients(Gradients g) {
            if (IsBuilt) return;

            base.InitializeGradients(g);
            v = NewContainer(1);
        }

        public override float UnpdateSingle(float g, int i) {
            v[i] = v[i] * gamma + g * g * (1 - gamma) ;
            return g / (float)System.Math.Sqrt(v[i] + 1e-5f) * lr;
        }
    }

    public class Adam : Optimizer {
        static ProfilerMarker profile = new ProfilerMarker("Adam.Update");
        public override ProfilerMarker p => profile;
        float[] v;
        float[] m;
        float lr;
        float gamma;
        float beta;

        public Adam(Gradients g, float lr = .001f, float beta = .99f, float gamma = .999f) : base(g) {
            this.lr = lr;
            this.gamma = gamma;
            this.beta = beta;
        }
        public Adam(float lr = .001f, float beta = .99f, float gamma = .999f) : base() {
            this.lr = lr;
            this.gamma = gamma;
            this.beta = beta;
        }


        public override void InitializeGradients(Gradients g) {
            if (IsBuilt) return;
            base.InitializeGradients(g);
            v = NewContainer(1);
            m = NewContainer();

        }

        public override float UnpdateSingle(float g, int i) {
            v[i] = v[i] * gamma + g * g * (1 - gamma) ;
            m[i] = m[i] * beta + g * (1 - beta);
            return m[i] / (float)System.Math.Sqrt(v[i] + 1e-5f) * lr;
        }
    }
}