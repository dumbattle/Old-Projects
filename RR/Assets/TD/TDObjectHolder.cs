using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDObjectHolder : MonoBehaviour {
    public static TDObjectHolder current;

    public GameObject trackTile;
    public GameObject towerTile;
    public GameObject creepObj;
    public GameObject MGTowerObj;
    public GameObject LaserObj;

    private void Awake() {
        current = this;
        trackTile.SetActive(false);
        towerTile.SetActive(false);
        creepObj.SetActive(false);
        MGTowerObj.SetActive(false);
        LaserObj.SetActive(false);
    }
}
