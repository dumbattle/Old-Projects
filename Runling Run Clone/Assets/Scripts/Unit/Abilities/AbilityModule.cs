using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class AbilityModule : UnitModule {
    public string key;
    public float cooldown = 1;
    protected bool active = true;
    
    public void Update() {
        if (active && Input.GetKey(key)) {
            Execute();
        }
    }
    protected void StartCooldown() {
        Invoke("EndCooldown", cooldown);
    }
    void EndCooldown() {
        CancelInvoke("EndCooldown");
        active = true;
    }
    public abstract void Execute();
}