using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphTester : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        Graph.YMin = -2;
        Graph.YMax = 2;

        Graph.channel[0].SetActive();
        Graph.channel[0].color = Color.green;
        Graph.channel[1].SetActive();
        Graph.channel[1].color = Color.red;
        Graph.channel[2].SetActive();
        Graph.channel[2].color = Color.blue;
        Graph.channel[3].SetActive();
        Graph.channel[3].color = Color.black;
        Graph.channel[4].SetActive();
        Graph.channel[4].color = Color.magenta;
        Graph.channel[5].SetActive();
        Graph.channel[5].color = Color.yellow;
        Graph.channel[6].SetActive();
        Graph.channel[6].color = Color.gray;
        Graph.channel[7].SetActive();
        Graph.channel[7].color = Color.cyan;


    }

    // Update is called once per frame
    void Update() {
        Graph.channel[0].Feed(Mathf.Sin(Time.time));
        Graph.channel[1].Feed(Mathf.Sin(Time.time * 1.1f));
        Graph.channel[2].Feed(Mathf.Sin(Time.time * 1.2f));
        Graph.channel[3].Feed(Mathf.Sin(Time.time * 1.3f));
        Graph.channel[4].Feed(Mathf.Sin(Time.time * 1.4f));
        Graph.channel[5].Feed(Mathf.Sin(Time.time * 1.5f));
        Graph.channel[6].Feed(Mathf.Sin(Time.time * 1.6f));
        Graph.channel[7].Feed(Mathf.Sin(Time.time * 1.7f));

    }
}