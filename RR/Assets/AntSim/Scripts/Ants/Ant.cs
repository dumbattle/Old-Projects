using UnityEngine;

namespace AntSim {
    public abstract class Ant : IPoolable {
        public const float DIST_THRESH = .1f;

        public bool Active { get; set; }
        public GameObject obj;
        public Transform transform => obj.transform;

        public float speed;
        public int sight;

        public float maxStanima;
        public float stanima;
        public float stanimaDecay;

        public int maxHealth;
        public int health { get; private set; }
        public int attack{ get; private set; }
        
        public AntColony colony;
        public HexCoord homeTile;
        public HexCoord hex;
        public HexCoord destination;

        public AntAI ai;
        

        public Ant (GameObject obj) {
            this.obj = obj;
        }

        public virtual void Initialize(AntColony colony) { 
}

        public void Initialize(AntColony colony, AntStats.CommonStats stats) {
            this.colony = colony;
            hex = homeTile = colony.pos;
            transform.position = homeTile.ToCartesian(AntSim.orientation, AntSim.Main.scale);

            speed = stats.speed;
            sight = stats.sight;
            stanima = maxStanima = stats.stanima;
            stanimaDecay = stats.stanimaDrain;
            health = maxHealth = stats.health;
            attack = stats.attack;

            AntSim.antMap.AddAnt(this, hex);
            obj.SetActive(true);
        }

        public virtual void Update() {
            ai?.Update();
        }

        public virtual void Die() {
            obj.SetActive(false);
            Active = false;
            AntSim.antMap.RemoveAnt(this, hex);
        }

        public virtual void Move(Vector3 direction) {
            transform.position += direction.normalized * speed * AntSim.DeltaTime;
            var newHex = HexCoord.FromCartesian(transform.position, AntSim.Main.scale, AntSim.orientation);

            if (hex != newHex) {
                AntSim.antMap.MoveAnt(this, hex, newHex);
            }

            hex = newHex;
            transform.up = direction;
        }

        public bool TakeDamage(int dmg) {
            health -= dmg;

            if (health <= 0 ){
                Die();
                return true;
            };
            return false;
        }
    }


}