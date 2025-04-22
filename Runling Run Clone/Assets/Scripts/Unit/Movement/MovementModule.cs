using UnityEngine;

[DisallowMultipleComponent()]
[RequireComponent(typeof(UnitController), typeof(Rigidbody2D), typeof(CircleCollider2D))]
public abstract class MovementModule : UnitModule {
    public float speed { get => baseSpeed * speedModifier; }
    public float baseSpeed;
    public float speedModifier = 1;
    protected new CircleCollider2D collider;

    public Rigidbody2D rb { get; private set; }


    public void Awake() {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        speedModifier = 1;
        Initialize();
    }

    public virtual void ApplySpeedModifier(float mod) {
        speedModifier *= mod;

        //fix rounding errors that might accumalate
        if (Mathf.Approximately(speedModifier,1f)) {
            speedModifier = 1;
        }
    }
    protected virtual void Initialize() { }
}
