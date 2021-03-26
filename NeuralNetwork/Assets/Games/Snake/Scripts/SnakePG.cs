using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class SnakePG : MonoBehaviour {
    public static SnakePG main;
    public ModelWeightsAsset modelWeights;
    public int mapSize = 10;
    public string output;

    SnakeAC agent;
    bool isPlaying = false;

    Vector2Int pos;
    Vector2Int food;
    Channel scoreChannel;
    int noFoodCount = 0;

    public int gameSpeed = 1;
    void Start() {
        main = this;
        scoreChannel = Channel.New("Score");
        if (modelWeights.HasData) {
            agent = new SnakeAC(new Vector2Int(mapSize, mapSize), modelWeights.Load());
        }else {
            agent = new SnakeAC(new Vector2Int(mapSize, mapSize));
        }
        StartCoroutine(Next());
        isPlaying = true;
    }


    void OnDrawGizmos() {
        if (!isPlaying) {
            return;
        }

        for (int x = 0; x < mapSize; x++) {
            for (int y = 0; y < mapSize; y++) {
                Gizmos.color = Color.black;
                if (food.x == x && food.y == y) {
                    Gizmos.color = Color.red;
                }
                if (pos.x == x && pos.y == y) {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one);
            }
        }

    }


    IEnumerator Next() {
        pos = new Vector2Int(mapSize / 2, mapSize / 2);
        food = RandomPos(pos);
        float totalReward = 0;
        while (true) {
            for (int i = 0; i < gameSpeed; i++) {
                totalReward = GameLoop(totalReward);
            }

            yield return null;
        }

    }

    private float GameLoop(float totalReward) {
        noFoodCount++;
        var mt = MapTensor();

        var exp = agent.SampleAction(mt);
        output = exp.output.ToString();
        //print(agent.testOP.value);
        Vector2Int dir = new Vector2Int(0, 0);
        switch (exp.action) {
            case 0:
                dir = new Vector2Int(0, 1);
                break;
            case 1:
                dir = new Vector2Int(1, 0);
                break;
            case 2:
                dir = new Vector2Int(0, -1);
                break;
            case 3:
                dir = new Vector2Int(-1, 0);
                break;
        }

        Vector2Int newPos = pos + dir;
        float reward = 0;

        if (newPos.x < 0 || newPos.x >= mapSize || newPos.y < 0 || newPos.y >= mapSize || noFoodCount >= 100) {
            reward = -1f;
        }
        else if (newPos == food) {
            reward = 1;
            food = RandomPos(newPos);
            noFoodCount = 0;
        }
        totalReward += reward;
        exp.reward = reward;

        agent.AddExperience(exp);
        pos = newPos;
        if (reward < 0) {
            agent.EndTrajectory();

            pos = new Vector2Int(mapSize / 2, mapSize / 2);
            food = RandomPos(pos);
            scoreChannel.Feed(totalReward);
            totalReward = 0;
            SnakePG.main.modelWeights.Save(agent.GetWeights());
            noFoodCount = 0;
        }

        return totalReward;
    }

    Tensor MapTensor() {
        Tensor result = new Tensor(mapSize, mapSize, 2);
        result[pos.x, pos.y, 0] = 1;
        result[food.x, food.y, 1] = 1;
        //for (int x = 0; x < mapSize; x++) {
        //    for (int y = 0; y < mapSize; y++) {
        //        result[x, y, 2] = 1;
        //    }
        //}
        return result;
    }
    Vector2Int RandomPos(Vector2Int invalidPos) {
        var result =  new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));
        if (result == invalidPos) {
            return RandomPos(invalidPos);
        }

        return result;
    }

   
}

public class SnakeAC : ActorCritic {
    public Operation testOP;
    Vector2Int _mapShape;

    public SnakeAC(Vector2Int mapShape) : base() {
        _mapShape = mapShape;
        Build();
    }

    public SnakeAC(Vector2Int mapShape, Tensor[] weights) : base() {
        _mapShape = mapShape;
        Build();
        combinedAC.SetWeights(weights);
    }

    protected override Operation Input() {
        Operation inp = new InputLayer(_mapShape.x, _mapShape.y, 2).Build();
        Operation x = new Convolution2D(5, af: ActivationFunction.Tanh, pad: false).Build(inp);
        x = new Convolution2D(5, af: ActivationFunction.Tanh, pad: false).Build(x);
        x = new Convolution2D(5, af: ActivationFunction.Tanh, pad: false).Build(x);
        x = new Convolution2D(1, af: ActivationFunction.Tanh, pad: false).Build(x);
        x = new FlattenOp(x);

        Operation y = new Convolution2D(5, af: ActivationFunction.Tanh, pad: true).Build(inp);
        y = new Convolution2D(5, af: ActivationFunction.Tanh, pad: true).Build(y);
        y = new Convolution2D(5, af: ActivationFunction.Tanh, pad: true).Build(y);
        y = new Convolution2D(1, af: ActivationFunction.Tanh, pad: true).Build(y);
        y = new FlattenOp(y);

        Operation o = new Append(x, y);
        o = new FullyConnected(50, ActivationFunction.Sigmoid, false).Build(o);

        return o;
    }


    protected override Operation Actor(Operation input) {
        Operation x = new FullyConnected(4, bias: false).Build(input);
        x = x.Softmax();
        return x;
    }

    protected override Operation Critic(Operation input) {
        Operation x = new FullyConnected(1, bias: true).Build(input);
        return x;
    }

    protected override Optimizer Optimizer() {
        return new RMSProp();
    }
}