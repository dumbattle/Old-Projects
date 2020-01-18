using UnityEngine;

[DisallowMultipleComponent()]
[RequireComponent(typeof(UnitController))]
public abstract class AnimationModule : UnitModule {
    public Vector2 direction;
    public abstract void SetDirection(Vector2 direction);
}