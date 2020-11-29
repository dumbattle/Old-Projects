using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class SnakePG : MonoBehaviour {
    public ModelWeightsAsset modelWeights;
    public int mapSize = 10;
    public string output;

    SnakeAC agent;
    bool isPlaying = false;

    Vector2Int pos;
    Vector2Int food;

    void Start() {
            agent = new SnakeAC(new Vector2Int(mapSize, mapSize), modelWeights);
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
        int step = 0;
        while (true) {
            step++;
            var mt = MapTensor();

            var exp = agent.SampleAction(mt);
            output = exp.output.ToString();
            //agent.criticModel.forward.Eval();
            print(agent.testOP.value);
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

            if (newPos.x < 0 || newPos.x >= mapSize || newPos.y < 0 || newPos.y >= mapSize || step > 100) {
                reward = -1;
            }
            else if (newPos == food) {
                reward = 1;
                food = RandomPos(newPos);
                step = 0;
            }

            exp.reward = reward;

            agent.AddExperience(exp);
            pos = newPos;
            if (reward < 0) {
                agent.EndTrajectory();
                pos = new Vector2Int(mapSize / 2, mapSize / 2);
                food = RandomPos(pos);
                step = 0;
            }


            yield return null;
        }

        Tensor MapTensor() {
            Tensor result = new Tensor(mapSize, mapSize, 2);
            result[pos.x, pos.y, 0] = 1;
            result[food.x, food.y, 1] = 1;
            return result;
        }
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
    Vector2Int _mapShape;
    ModelWeightsAsset _asset;
    public Operation testOP;
    public SnakeAC(Vector2Int mapShape, ModelWeightsAsset savedData) : base() {
        //normalizeRewards = false;
        _mapShape = mapShape;
        Build();
        _asset = savedData;
        if (savedData == null || !savedData.HasData) {
            return;
        }
        combinedAC.SetWeights(savedData.Load());
    }

    protected override Operation Input() {
        Operation x = new InputLayer(_mapShape.x, _mapShape.y, 2).Build();
        x = new Convolution2D(30, af: ActivationFunction.Tanh, pad: false).Build(x);
        x = new Convolution2D(30, af: ActivationFunction.Tanh, pad: false).Build(x);
        x = new Convolution2D(30, af: ActivationFunction.Tanh, pad: false).Build(x);
         x = new Convolution2D(30, af: ActivationFunction.Tanh, pad: false).Build(x);
        testOP = x = new GlobalAveragePooling(x);

        x = new FullyConnected(50, ActivationFunction.Tanh).Build(x);

        return x;
    }
    public override void EndTrajectory() {
        base.EndTrajectory();
        _asset?.Save(GetVariables());
    }

    protected override Operation Actor(Operation input) {
        Operation x = new FullyConnected(4).Build(input);
        x = x.Softmax();
        return x;
    }

    protected override Operation Critic(Operation input) {
        Operation x = new FullyConnected(1).Build(input);
        return x;
    }

    protected override Optimizer Optimizer() {
        return new RMSProp();
    }
}