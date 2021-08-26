//using System;
//using System.Linq;
//using System.Collections.Generic;
//using UnityEngine;
//using DumbML;


//public class GraphTester : MonoBehaviour {
//    [TextArea]
//    public string output;

//    TestClass tc;

//    Tensor input;
//    //void Start() {
//    //    tc = new TestClass();new
//    //    tc.normalizeRewards = false;
//    //    tc.Build();
//    //    input = Tensor.Random(10);
//    //    print(new Constant(Tensor.FromArray(new float[] { -1, 0, 1, 2 })).Softmax().Eval());
//    //}


//    //private void Update() {
//    //    for (int i = 0; i < 100; i++) {
//    //        var exp = tc.SampleAction( input);
//    //        exp.reward = exp.action;
//    //        tc.AddExperience(exp);
//    //        output = tc.actorModel.Result.ToString();
//    //    }
//    //    tc.EndTrajectory();
//    //}
//    void Start() {
//        tc = new TestClass();
//        tc.normalizeRewards = false;
//        input = Tensor.Random(10);
//        print(new Constant(Tensor.FromArray(new float[] { -1, 0, 1, 2 })).Softmax().Eval());
//    }


//    private void Update() {
//        for (int i = 0; i < 100; i++) {
//            var exp = tc.Sample(("Input", input));
//            tc.AddExperience(exp, exp.action);
//        }
//            output = tc["Actor"].ToString();
//        tc.EndTrajectory();
//    }
//}

//public class TestClass : AC {
//    protected override Operation Input() {
//        return new FullyConnected(10, ActivationFunction.Sigmoid).Build( new Placeholder("Input", 10));
//    }
//    protected override Operation Actor(Operation input) {
//        return new FullyConnected(4, ActivationFunction.Sigmoid).Build(input).Softmax();
//    }

//    protected override Operation Critic(Operation input) {
//        return new FullyConnected(1).Build(input);
//    }
//    protected override Optimizer Optimizer() {
//        return new Adam();
//    }
//}
//public abstract class AC : CustomModel {
//    public bool normalizeRewards = true;
//    public float discount = .9f;

//    Dictionary<string, Placeholder> inputStates;
//    string[] inputStatesOrder;
//    int numStateTensors;
//    RingBuffer<Experience> trajectory = new RingBuffer<Experience>(1000);
//    int actionSpace;
//    ObjectPool<Experience> experiencePool;

//    ObjectPool<Tensor> rewardTensorPool;
//    ObjectPool<Tensor> actionMaskPool;

//    protected override sealed (string name, Operation op)[] Forward() {
//        Operation x = Input();

//        inputStates = x.GetOperations<Placeholder>().Distinct().ToDictionary((p) => p.Name);
//        inputStatesOrder = inputStates.Keys.ToArray();
//        numStateTensors = inputStatesOrder.Length;

//        var a = Actor(x);
//        var c = Critic(x);
//        actionSpace = a.shape[0];

//        experiencePool = new ObjectPool<Experience>(30,
//            () => new Experience(),
//            (e) => {
//                e.action = 0;
//                e.output = null;
//                e.state = null;
//                e.reward = 0;
//            }
//        );
//        actionMaskPool = new ObjectPool<Tensor>(30, () => new Tensor(actionSpace));
//        rewardTensorPool = new ObjectPool<Tensor>(30, () => new Tensor(1));
//        return new[] {
//            ("Actor", a),
//            ("Critic", c)
//        };
//    }

//    protected abstract Operation Input();
//    protected abstract Operation Actor(Operation input);
//    protected abstract Operation Critic(Operation input);

//    protected override sealed Operation Loss(IReadOnlyDictionary<string, Operation> forwardOps) {
//        var a = forwardOps["Actor"];
//        var c = forwardOps["Critic"];

//        var rewardPH = new Placeholder("Reward", 1);

//        var adv = rewardPH - c;
//        var actionMask = new Placeholder("Action Mask", a.shape);


//        var aloss = new Log(a * actionMask) * new BroadcastScalar(-1 * adv.Detach(), a.shape);
//        var cLoss = DumbML.Loss.MSE.Compute(c, rewardPH);

//        return new Sum(aloss) + cLoss;
//    }



//    public Experience Sample(params Tensor[] state) {
//        Build();
//        return Sample(state.Zip(inputStatesOrder, (s, o) => (o, s)).ToArray());
//    }

//    public Experience Sample(params (string ph, Tensor val)[] state) {
//        Build();

//        if (state.Length != inputStatesOrder.Length) {
//            throw new ArgumentException($"Wrong number of states. Expected: {inputStatesOrder.Length}. Got: {state.Length}");
//        }
//        Experience exp = experiencePool.Get().obj;
//        exp.state = state;

//        Compute(state);
//        exp.output = this["Actor"];
//        exp.action = exp.output.Sample();
//        return exp;
//    }

//    public void AddExperience(Experience exp, float reward) {
//        exp.reward = reward;
//        trajectory.Add(exp);
//    }

//    public void EndTrajectory() {
//        NormalizeRewards();
//        Train();
//        foreach (var e in trajectory) {
//            experiencePool.Return(e);
//        }
//        trajectory.Clear();
//    }


//    void NormalizeRewards() {
//        if (!normalizeRewards) {
//            return;
//        }

//        float score = 0;

//        for (int i = trajectory.Count - 1; i >= 0; i--) {
//            var exp = trajectory[i];

//            score *= discount;
//            score += exp.reward;
//            exp.reward = score;
//        }
//    }
//    void Train() {
        
//        int size = trajectory.Count;
//        (string, Tensor[])[] inputs = new (string, Tensor[])[numStateTensors + 2];

//        Tensor[] rewards = new Tensor[size];
//        Tensor[] actionsMasks = new Tensor[size];

//        for (int i = 0; i < size; i++) {
//            var e = trajectory[i];
//            rewards[i] = rewardTensorPool.Get().obj;
//            rewards[i][0] = e.reward;

//            actionsMasks[i] = actionMaskPool.Get().obj;
//            actionsMasks[i][e.action] = 1;
//        }

//        Tensor[][] states = new Tensor[numStateTensors][];

//        for (int i = 0; i < numStateTensors; i++) {
//            //states[i] = (from x in trajectory select x.state[i].Item2).ToArray();
//            var t = new Tensor[size];
//            for (int j = 0; j < size; j++) {
//                t[j] = trajectory[i].state[i].Item2;
//            }
//            states[i] = t;
//        }

//        inputs[0] = ("Reward", rewards);
//        inputs[1] = ("Action Mask", actionsMasks);

//        for (int i = 0; i < numStateTensors; i++) {
//            inputs[i + 2] = (inputStatesOrder[i], states[i]);
//        }

//        Train(inputs);
//        for (int i = 0; i < size; i++) {
//            var e = trajectory[i];
//            rewardTensorPool.Return(rewards[i]);

//            actionMaskPool.Return(actionsMasks[i]);
//        }

//    }

//    protected override Optimizer Optimizer() {
//        return new Adam();
//    }

//    public class Experience {
//        public (string, Tensor)[] state;
//        public Tensor output;

//        public float reward;
//        public int action;

//    }
//}

//public abstract class CustomModel : Model_base {
//    protected override void Build() {
//        if (graph != null) {
//            return;
//        }
//        base.Build();
//        BuildOptimizer();
//    }

//    void BuildOptimizer() {
//        var l = Loss(forwardOps);
//        SetTrainer(l, Optimizer());
//    }


//    protected abstract override (string name, Operation op)[] Forward();
//    protected abstract Operation Loss(IReadOnlyDictionary<string, Operation> forwardOps);
//    protected virtual Optimizer Optimizer() { return new SGD(); }

//   public new Tensor Train(params (string ph, Tensor[] vals)[] inputs) {
//        return base.Train(inputs);
//    }
//}



//public class OperationGraph : Graph<Operation> {
//    Dictionary<Operation, bool> visited = new Dictionary<Operation, bool>();
//    Dictionary<string, Placeholder> placeHolders;

//    public OperationGraph(params Operation[] ops) {
//        Action<Operation> BuildGraph = null;

//        BuildGraph = (o) => {
//            if (Contains(o)) {
//                return;
//            }

//            AddVertex(o);
//            visited.Add(o, false);

//            foreach (var inner in o.inner) {
//                BuildGraph(inner);
//                AddEdge(inner, o, true);
//            }
//        };

//        foreach (var op in ops) {
//            BuildGraph(op);
//        }

//        placeHolders = new Dictionary<string, Placeholder>();
//        foreach (var ph in GetPlaceholders()) {
//            string name = ph.Name;
//            if (placeHolders.ContainsKey(name)) {
//                throw new ArgumentException($"Placeholder with name'{name}' already exists in this graph");
//            }else {
//                placeHolders.Add(name, ph);
//            }
//        }
//    }


//    public void Compute(params (string name, Tensor val)[] inputs) {
//        SetInputs(inputs);
//        Eval();
//    }

//    public void Eval() {
//        foreach (var k in visited.Keys.ToList()) {
//            visited[k] = false;
//        }

//        foreach (var r in GetRoots()) {
//            EvalOp(r);
//        }

//        Tensor EvalOp(GraphVertex<Operation> v) {
//            if (visited[v.Value]) {
//                return v.Value.value;
//            }
//            Operation op = v.Value;

//            //Tensor[] operands = new Tensor[op.inner.Length];

//            for (int i = 0; i < op.operands.Length; i++) {
//                op.operands[i] = EvalOp(GetVertex(op.inner[i]));
//            }

//            visited[v.Value] = true;
//            return op.Compute(op.operands);
//        }
//    }

//    protected void SetInputs(params (string name, Tensor val)[] inputs) {
//        foreach (var i in inputs) {
//            if (placeHolders.ContainsKey(i.name)) {
//                placeHolders[i.name].SetVal(i.val);
//            }
//        }
//    }
//    public IEnumerable<Placeholder> GetPlaceholders() {
//        return GetOperations<Placeholder>();
//    }
//    public IEnumerable<Operation> GetOperations(Func<Operation, bool> condition = null) {
//        return (from x in Vertices() where (condition == null || condition(x.Value)) select x.Value);
//    }
//    public IEnumerable<T> GetOperations<T>(Func<Operation, bool> condition = null) where T : Operation {
//        return (from x in Vertices() where (x.Value is T t && (condition == null || condition(t))) select x.Value as T);
//    }

//}

//public abstract class Model_base {
//    protected OperationGraph graph;
//    protected Dictionary<string, Operation> forwardOps;

//    public Tensor this[string name] {
//        get => forwardOps[name].value;
//    }


//    protected abstract (string name, Operation op)[] Forward();

//    protected virtual void Build() {
//        if (graph != null) {
//            return;
//        }
//        forwardOps = new Dictionary<string, Operation>();

//        foreach (var (name, op) in Forward()) {
//            forwardOps.Add(name, op);
//        }

//        graph = new OperationGraph((from x in forwardOps select x.Value).ToArray());
//    }

//    public void Compute(params (string name, Tensor val)[] inputs) {
//        if (graph == null) {
//            Build();
//        }
//        graph.Compute(inputs);
//    }



//    protected MTrainer trainer;

//    protected void SetTrainer(Operation lossFn, Optimizer o) {
//        Build();
//        trainer = new MTrainer(lossFn, o);
//    }


//    protected Tensor Train(params (string name, Tensor[] batch)[] inputs) {
//        Build();

//        int count = -1;

//        foreach (var (name, val) in inputs) {
//            if (count == -1) {
//                count = val.Length;
//            }

//            if (count != val.Length) {
//                throw new ArgumentException("Each input must have the same number of samples");
//            }
//        }

//        Tensor loss = null;
//        (string name, Tensor)[] input = new (string, Tensor)[inputs.Length];
//        for (int i = 0; i < count; i++) {

//            for (int j = 0; j < inputs.Length; j++) {
//                input[j] = (inputs[j].name, inputs[j].batch[i]);
//            }

//            Tensor l = trainer.FullPass(input);
//            loss = loss == null ? l : loss.Add(l, true);
//        }

//        trainer.Update();

//        return loss;
//    }

//}

//public class NeuralModel : Model_base {
//    const string INPUT_PH_NAME = "Input";
//    const string TARGET_PH_NAME = "Target";
//    int[] inputShape;
//    Operation outputOp;
//    List<ILayer> layers = new List<ILayer>();


//    public NeuralModel(params int[] inputShape) {
//        this.inputShape = inputShape;
//    }


//    public void Add(ILayer layer) {
//        layers.Add(layer);
//    }

//    public Tensor Compute(Tensor input) {
//        Compute((INPUT_PH_NAME, input));
//        return outputOp.value;
//    }

//    protected override sealed (string name, Operation op)[] Forward() {
//        Operation x = new Placeholder(INPUT_PH_NAME, inputShape);

//        foreach (var l in layers) {
//            x = l.Build(x);
//        }

//        outputOp = x;

//        return new[] { ("Output", x) };
//    }



//    public void SetOptimizer(Loss l , Optimizer o) {
//        Operation loss = l.Compute(outputOp, new Placeholder(TARGET_PH_NAME, outputOp.shape));
//        SetTrainer(loss, o);
//    }

//    public Tensor Train(Tensor[] inputs, Tensor[] targets, int batchSize = 32) {
//        return Train((INPUT_PH_NAME, inputs), (TARGET_PH_NAME, targets));
//    }
//}

//public class MTrainer {
//    OperationGraph lossGraph;

//    Dictionary<Operation, Tensor> gradients;
//    Dictionary<Operation, bool> visited;

//    Gradients trackedGradients;
//    Optimizer o;

//    Tensor lossTensor;
//    Operation lossOp;
//    public MTrainer(Operation loss, Optimizer o) {
//        lossOp = loss;
//        lossGraph = new OperationGraph(loss);
//        trackedGradients = new Gradients(lossGraph.GetOperations<Variable>());
//        this.o = o;
//        o.InitializeGradients(trackedGradients);

//        gradients = new Dictionary<Operation, Tensor>();
//        visited = new Dictionary<Operation, bool>();
//        foreach (var op in lossGraph.GetOperations()) {
//            gradients.Add(op, new Tensor(op.shape));
//            visited.Add(op, false);
//        }
//        lossTensor = new Tensor(() => 1, loss.shape);
//    }

//    public Tensor FullPass(params (string name, Tensor val)[] inputs) {
//        lossGraph.Compute(inputs);
//        Backwards();
//        return lossOp.value;
//    }

//    public void Backwards() {
//        foreach (var k in gradients.Keys.ToList()) {
//            gradients[k].SetValuesToZero();
//            visited[k] = false;
//        }
//        gradients[lossOp] = lossTensor;

//        foreach (var l in trackedGradients.keys) {
//            CalcGrad(l);
//            trackedGradients[l].Add(gradients[l], true);
//        }


//        void CalcGrad(Operation op) {
//            if (visited[op]) {
//                return;
//            }
//            else {
//                foreach (var parent in lossGraph.GetVertex(op).NextVertices()) {
//                    CalcGrad(parent.Value);
//                }

//                var childGrads = op.BackwardsPass(gradients[op]);

//                if (childGrads != null) {
//                    for (int i = 0; i < childGrads.Length; i++) {
//                        var g = gradients[op.inner[i]];

//                            g.Add(childGrads[i], true);
//                    }
//                }
//                visited[op] = true;
//            }
//        }
//    }

//    public void Update() {
//        o.Update();
//        o.ZeroGrad();
//    }
//}



//public class ObjectPool<T> {
//    Dictionary<T, PooledItem<T>> dict = new Dictionary<T, PooledItem<T>>();
//    List<PooledItem<T>> pool;
//    Func<T> itemCreator;
//    Action<T> itemInitializer;

//    public ObjectPool(int initialCapacity, Func<T> itemCreator) {
//        pool = new List<PooledItem<T>>(initialCapacity);
//        this.itemCreator = itemCreator;
//        for (int i = 0; i < initialCapacity; i++) {
//            CreateNewItem();
//        }
//    }

//    public ObjectPool(int initialCapacity, Func<T> itemCreator, Action<T> itemInitializer) {
//        pool = new List<PooledItem<T>>(initialCapacity);

//        this.itemCreator = itemCreator;
//        this.itemInitializer = itemInitializer;
//        for (int i = 0; i < initialCapacity; i++) {
//            CreateNewItem();
//        }

//    }

//    PooledItem<T> CreateNewItem() {
//        PooledItem<T> newItem = new PooledItem<T>(itemCreator, itemInitializer);
//        pool.Add(newItem);
//        dict.Add(newItem.obj, newItem);
//        return newItem;
//    }

//    public PooledItem<T> Get() {
//        foreach (var i in pool) {
//            if (!i.active) {
//                i.Activate();
//                return i;
//            }
//        }

//        PooledItem<T> newItem = CreateNewItem();
//        newItem.Activate();
//        return newItem;
//    }

//    public void Return(PooledItem<T> item) {
//        item.Return();
//    }
//    public void Return(T item) {
//        dict[item].Return();
//    }
//}

//public class PooledItem<T> {
//    public T obj { get; private set; }
//    public bool active { get; private set; }

//    Func<T> itemCreator;
//    Action<T> itemInitializer;

//    public PooledItem (Func<T> itemCreator, Action<T> itemInitializer) {
//        obj = itemCreator();
//        this.itemInitializer = itemInitializer;
//    }
//    public void Activate() {
//        active = true;
//        itemInitializer?.Invoke(obj);
//    }
//    public void Return() {
//        active = false;
//    }
//}
