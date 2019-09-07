using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour {
    public UnitStatBar HealthBar;
    public UnitStatBar StaminaBar;
    public UnitStatBar ThirstBar;
    public UnitStatBar HungerBar;

    public float health = 100;
    public float stamina = 100;
    public float thirst = 100;
    public float hunger = 100;

    void LateUpdate() {
        transform.forward = -(Camera.main.transform.forward);
        //transform.LookAt(Camera.main.transform);
    }

    public void Refresh() {
        health = 100;
        HealthBar.Draw(1);

        stamina = 100;
        StaminaBar.Draw(1);

        thirst = 100;
        ThirstBar.Draw(1);

        hunger = 100;
        HungerBar.Draw(1);
    }

    public void UpdateHealth(float amnt) {
        health += amnt;
        health = health.Clamp(0, 100);
        HealthBar.Draw(health / 100);
    }
    public void UpdateStanima(float amnt) {
        stamina += amnt;
        stamina = stamina.Clamp(0, 100);
        StaminaBar.Draw(stamina / 100);
    }
    public void UpdateThirst(float amnt) {
        thirst += amnt;
        thirst = thirst.Clamp(0, 100);
        ThirstBar.Draw(thirst / 100);
    }
    public void UpdateHunger(float amnt) {
        hunger += amnt;
        hunger = hunger.Clamp(0, 100);
        HungerBar.Draw(hunger / 100);
    }
}
