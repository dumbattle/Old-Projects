using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedupAbility : AbilityModule {
    public float speedBonus = 2;
    public float duration = 2;


    public override void Execute() {
        active = false;
        controller.Movement.ApplySpeedModifier(speedBonus);
        Invoke("End", duration);
    }
    void End() {
        controller.Movement.ApplySpeedModifier(1f / speedBonus);
        StartCooldown();
    }
}