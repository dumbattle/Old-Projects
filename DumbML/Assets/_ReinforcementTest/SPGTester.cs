using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class SPGTester : MonoBehaviour {
    public Point2 mapSize;
    public GameObject tile;

    [Space]
    public Point2 agentPos;
    public Point2 goalPos;

    NeuralNetwork agent;
    SpriteRenderer[,] tiles;

    Tensor testInput;
    public string testOutput;
    public string output;
    public string smOutput;

    SPG spg;

    public int step = 0;

    void Start() {
        tiles = new SpriteRenderer[mapSize.x, mapSize.y];

        Camera.main.orthographicSize = mapSize.x / 2 + 1;
        Camera.main.transform.position = new Vector3(mapSize.x / 2, mapSize.y / 2, -10);

        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                GameObject g = Instantiate(tile, new Vector2(x, y), Quaternion.identity);

                tiles[x, y] = g.GetComponent<SpriteRenderer>();
                tiles[x, y].color = Color.black;
            }
        }

        ResetGame();
        SetupNetwork();
        testInput = GetInput();
    }


    // Update is called once per frame
    void Update() {
        Next();
    }

    void Next() {
        step++;
        testOutput = agent.Compute(testInput).ToString();
        Tensor input = GetInput();
        var (action, prob) = spg.Next(input);

        output = agent.Layers[2].output.ToString();
        smOutput = agent.Layers[3].output.ToString();

        Point2 direction = Point2.UDLR[action];
        Point2 newPoint = agentPos + direction;

        float reward = -1;
        bool score = false;

        if (IsInRange(newPoint)) {
            SetAgentPosition(newPoint);
            score = UpdateGame();
            reward = score ? 1 : 0;
        }

        spg.Add(input, action, prob, reward);

        if (step >= 100) {
            spg.Train();
            step = 0;
        }
        
    }

    bool UpdateGame() {
        if (agentPos == goalPos) {
            SetGoalPosition(Point2.Random(mapSize));
            return true;
        }
        else {
            return false;
        }
    }

    bool IsInRange(Point2 p) {
        return IsInRange(p.x, p.y);
    }
    bool IsInRange(int x, int y) {
        return x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y;
    }

    Tensor GetInput() {
        Tensor result = new Tensor(4);

        //result[0] = (float)agentPos.x;
        //result[1] = (float)agentPos.y;
        //result[2] = (float)goalPos.x;
        //result[3] = (float)goalPos.y;

        result[0] = (float)agentPos.x / mapSize.x;
        result[1] = (float)agentPos.y / mapSize.y;
        result[2] = (float)goalPos.x / mapSize.x;
        result[3] = (float)goalPos.y / mapSize.y;

        return result;
    }

    private void SetupNetwork() {
        const float INIT = .01f;
        agent = new NeuralNetwork(4);
        agent.Add(new FullyConnected(32, new WeightInitializer.Uniform(-INIT, INIT), ActivationFunction.LeakyRelu));
        agent.Add(new FullyConnected(32, new WeightInitializer.Uniform(-INIT, INIT), ActivationFunction.LeakyRelu));
        agent.Add(new FullyConnected(4, new WeightInitializer.Uniform(-INIT, INIT), ActivationFunction.LeakyRelu));
        agent.Add(new SoftMax());
        agent.Build();

        spg = new SPG(agent);
    }

    public void ResetGame() {
        SetAgentPosition((0, 0));
        SetGoalPosition(mapSize / 2);
    }

    void SetGoalPosition(Point2 p) {
        if (p == agentPos) {
            p = Point2.Random(mapSize);
        }
        goalPos = p;
        if (p != null) {
            tiles[p.x, p.y].color = Color.black;
        }
        tiles[p.x, p.y].color = Color.yellow;
    }
    void SetAgentPosition(Point2 p) {

        if (agentPos != null) {
            tiles[agentPos.x, agentPos.y].color = Color.black;
            if (agentPos == goalPos) {
                tiles[agentPos.x, agentPos.y].color = Color.yellow;

            }
        }

        agentPos = p;
        tiles[p.x, p.y].color = Color.green;
    }
}

public class SPG {
    public NeuralNetwork agent;
    public int actionSpace;
    List<Tensor> probs = new List<Tensor>();
    Optimizer optimizer;

    List<(Tensor state, Tensor gradient, float reward)> memory = new List<(Tensor, Tensor, float)>();

    public SPG (NeuralNetwork agent) {
        this.agent = agent;
        actionSpace = agent.outputShape[0];
        optimizer = new RMSProp(agent);
    }




    public (int action, Tensor prob) Next (Tensor input) {
        Tensor output = agent.Compute(input);
        probs.Add(output);

        int action = 0;

        float dart = Random.value;

        for (int i = 0; i < output.Shape[0]; i++) {
            dart -= output[i];
            if (dart <= 0) {
                action = i;
                break;
            }
        }

        return (action, output);
    }

    public void Add(Tensor state, int action, Tensor prob, float reward) {
        // state, action, prob, reward
        Tensor target = new Tensor(actionSpace);
        target[action] = 1;

        //Tensor gradient = prob.PointWise((f) => (float)System.Math.Log(f));
        Tensor gradient = target - prob;

        memory.Add((state, gradient, reward));
    }

    public void Train() {
        Tensor[] inputs = (from x in memory select x.state).ToArray();
        float[] rewards = (from x in memory select x.reward).ToArray();
        Tensor[] targets = new Tensor[inputs.Length];

        NormalizeRewards();
        rewards = DiscoutRewards(rewards);

        for (int i = 0; i < targets.Length; i++) {
            targets[i] = probs[i] + memory[i].gradient * rewards[i] * .001f;
        }

        optimizer.TrainBatch(inputs, targets);
        memory.Clear();
        probs.Clear();
    }

    float[] DiscoutRewards(float[] rewards) {
        float[] result = new float[rewards.Length];

        float running = 0;
        for (int i = rewards.Length - 1; i >= 0; i--) {
            float r = rewards[i];

            if (r != 0) {
                running = 0;
            }
            running = running * .9f + r;
            result[i] = running;
        }
        return result;
    }
    void NormalizeRewards() {
        float mean = 0;
        float stdev = 0;
        foreach (var m in memory) {
            mean += m.reward;
        }

        mean /= memory.Count();
        foreach (var m in memory) {
            stdev += (m.reward - mean).Sqr();
        }
        stdev = stdev.Sqrt();

        for (int i = 0; i < memory.Count; i++) {
            memory[i] = (memory[i].state, memory[i].gradient, ((memory[i].reward - mean) / stdev));
        }
    }
}
