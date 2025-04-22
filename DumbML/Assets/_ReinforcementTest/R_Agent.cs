using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;


public class R_Agent : MonoBehaviour {
    static NeuralNetwork brain;
    static ReinforcementTrainer2 rt;
    
    public float avgScore;
    float score = 0;
    public float speed = 5;
    public float loss;
    public float exploration;
    public int resolution = 10;
    int previousAction;
    Tensor previousState;
    public int step = 0;


    void Start() {
        if (brain == null) {
            brain = new NeuralNetwork(resolution + 1);
            brain.Add(new FullyConnected(resolution * 2, new WeightInitializer.Uniform(-1,1), ActivationFunction.LeakyRelu));
            brain.Add(new FullyConnected(resolution * 1, new WeightInitializer.Uniform(-1, 1), ActivationFunction.LeakyRelu));
            brain.Add(new FullyConnected(3, new WeightInitializer.Uniform(-1, 1)));
            brain.Build();
            rt = new ReinforcementTrainer2(brain, 3);
        }
    }

    void Update() {
        step++;
        if (step >= 1000) {
            loss = rt.Train(1080,32,10);
            step = 0;
        }
        if (step % 100 == 0) {
            score = 1;
        }
        var input = GetInput();
        var output = brain.Compute(input);

        if (previousState != null) {
            rt.Add(previousState, previousAction, score, input, score > .5f ? 10 : 1);
            avgScore = .9f * avgScore + score;
            score = 0;
        }

        //loss = rt.Train(10);
        //loss = o.Train(input, new Tensor(3));

        exploration = rt.exploreProb;
        int action = rt.Next(input);

        previousState = input;
        previousAction = action;

        if (action == 0) {
            transform.Translate(new Vector3(-speed * ReinforcementTester.timeStep, 0, 0));
        }

        if (action == 2) {
            transform.Translate(new Vector3(speed * ReinforcementTester.timeStep, 0, 0));
        }

        float x = transform.position.x.Clamp(ReinforcementTester.current.mapWidth / -2f, ReinforcementTester.current.mapWidth / 2f);
        transform.position = new Vector2(x, transform.position.y);


    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Obstacle")) {
            score += 1;
            //Destroy(other.gameObject);
        }
    }

    Tensor GetInput () {
        Tensor result = new Tensor(resolution + 1);

        float scale = 1f / (resolution - 1f);

        for (int i = 0; i < resolution; i++) {
            float x =( scale * i - .5f) * ReinforcementTester.current.mapWidth;

            var hit = Physics2D.Raycast(new Vector3(x, 10, 0), Vector2.down, 10, 1 << 8);
            result[i] = hit.distance / 10;
        }

        result[resolution] = transform.position.x / ReinforcementTester.current.mapWidth * 2;
        //print(result);
        return result;
    }


}