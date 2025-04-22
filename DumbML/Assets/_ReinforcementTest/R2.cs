using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;
using DumbML.Unity;


public class R2 : MonoBehaviour {
    public Point2 mapSize;
    public GameObject tile;

    [Space]
    public Point2 agentPos;
    public Point2 goalPos;

    NeuralNetwork agent;
    SpriteRenderer[,] tiles;
    public string output;
    public ReinforcementTrainer2 rt;

    public NeuralNetworkAsset nna;
    // Start is called before the first frame update
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
    }

    private void SetupNetwork() {
        if (!nna.HasNetwork) {
            agent = new NeuralNetwork(4);
            agent.Add(new FullyConnected(32, new WeightInitializer.Uniform(-.1f, .1f), ActivationFunction.LeakyRelu));
            agent.Add(new FullyConnected(32, new WeightInitializer.Uniform(-.1f, .1f), ActivationFunction.LeakyRelu));
            agent.Add(new FullyConnected(4, new WeightInitializer.Uniform(-.1f, .1f)));
            agent.Build();

            nna.Save(agent);
        }
        else {
            agent = nna.Load();
        }

        rt = new ReinforcementTrainer2(agent, 4);
        rt.exploreDecay = .000_01f;
        //rt.curiosityStop = .1f;
    }

    void Update() {
        Next();
    }

    void Next () {
        Tensor input = GetInput();
        int action = rt.Next(input);

        Point2 direction = Point2.UDLR[action];

        Point2 newPoint = agentPos + direction;

        float reward = -1;
        bool score = false;

        if (IsInRange(newPoint)) {
            SetAgentPosition(newPoint);
            score = UpdateGame();
            reward = score ? 1 : 0;
        }

        if (score) {
            rt.Add(input, action, reward, null);
        }
        else {
            rt.Add(input, action, reward /*- (agentPos - goalPos).Magnitude()*/, GetInput());
        }

        rt.Train(8,8,4);
        output = agent.Compute( GetInput()).ToString();
        //nna.Save(agent);

        if (rt.step % 1000 == 0) {
            nna.Save(agent);
        }
    }

    bool UpdateGame () {
        if (agentPos == goalPos) {
            SetGoalPosition( Point2.Random(mapSize));
            return true;
        } else {
            return false;
        }
    }

    int GetOutput(Tensor input) {
        Tensor output = agent.Compute(input);
        int max = 0;

        for (int i = 1; i < 4; i++) {
            if (output[i] > output[max]) {
                max = i;
            }
        }

        int action = rt.Next(input);

        return max;
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


    bool IsInRange(Point2 p) {
        return IsInRange(p.x, p.y);
    }
    bool IsInRange(int x, int y) {
        return x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y;
    }


    public void ResetGame () {
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
