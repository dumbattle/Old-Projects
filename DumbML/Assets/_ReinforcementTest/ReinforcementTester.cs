using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReinforcementTester : MonoBehaviour {
    public static ReinforcementTester current;
    public const float timeStep = .05f;
    public GameObject map;
    public float mapWidth;

    public GameObject player;
    public GameObject obstacle;
    public GameObject bonus;

    public float spawnInterval = 2;
    public float bonusSpawnInterval = 5;

    float bonusTimer = 0;
    float spawnTimer = 0;
    

    void Start() {
        current = this;
        map.transform.localScale = new Vector2(mapWidth, 100);
        obstacle.SetActive(false);
        bonus.SetActive(false);

        
        player.transform.position = new Vector2(0, -3.5f);
    }

    void Update() {
        spawnTimer += timeStep;
        bonusTimer += timeStep;

        if (spawnTimer >= spawnInterval) {
            spawnTimer = 0;
            Vector2 spawnLocation = new Vector2(Random.Range(-mapWidth, mapWidth) / 2, 10);

            Instantiate(obstacle, spawnLocation, Quaternion.identity).SetActive(true);
        }

        if (bonusTimer >= bonusSpawnInterval) {
            bonusTimer = 0;
            Vector2 spawnLocation = new Vector2(Random.Range(-mapWidth, mapWidth) / 2, 6);

            Instantiate(bonus, spawnLocation, Quaternion.identity).SetActive(true);
        }
    }
}