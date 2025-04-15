using System.Collections.Generic;
using UnityEngine;

namespace Swimming {
    public class BackgroundBehaviour : MonoBehaviour {
        public SalmonMain main;
        public List<FishEntry> fishes;


        float height => main.parameters.playableHeight;
        float width => main.parameters.playableWidth;


        List<ActiveFishEntry> activeFishes = new List<ActiveFishEntry>();

        Vector3 current;
        Vector3 targetCurrent;
        int currentTimer=0;

        void Start() {
            for (int i = 0; i < fishes.Count; i++) {
                FishEntry e = fishes[i];
                e.src.SetActive(false);

                for (int j = 0; j < e.count; j++) {
                    var obj = Instantiate(e.src);
                    activeFishes.Add(new ActiveFishEntry(obj, e));
                    obj.transform.position = new Vector2(Random.Range(0, width), Random.Range(0, height));
                    obj.SetActive(true);
                }
            }
            
        }
        void Update() {
            currentTimer--;
            if (currentTimer <= 0) {
                currentTimer = 100;
                targetCurrent = Random.insideUnitCircle;
            }
            current = targetCurrent * 0.025f + current * 0.975f;

            foreach (var fish in activeFishes) {
                UpdateTimer(fish);
                Vector3 targetDir = UpdateVelocityToTarget(fish);
                Vector3 flocking = UpdateVelocityBoids(fish);
                UpdateVelocity(fish, targetDir, flocking);
                AvoidPredator(fish);
                Vector3 p = UpdatePosition(fish);
                p = ScreenWrap(p);

                fish.obj.transform.position = p;
            }

            void UpdateTimer(ActiveFishEntry fish) {
                fish.timer--;
                if (fish.timer < 0) {
                    fish.timer = Random.Range(100, 150);
                    fish.target = new Vector2(Random.Range(0, width), Random.Range(0, height));
                }
            }

            Vector3 UpdateVelocityToTarget(ActiveFishEntry fish) {
                if (fish.data.flocking == 1) {
                    return new Vector3();
                }
                var targetDir = GetDir(fish.obj.transform.position, fish.target).normalized;

               

                return targetDir.normalized;
            }

            Vector3 UpdateVelocityBoids(ActiveFishEntry fish) {
                if(fish.data.flocking == 0) {
                    return new Vector3();
                }

                var targetSeperation = 0.5f;
                int count = 0;
                Vector3 cohesion = new Vector2();
                Vector3 seperation = new Vector2();
                Vector3 alignment = new Vector2();

                foreach (var f in activeFishes) {
                    if (f == fish) {  continue; }
                    if (f.data != fish.data) { continue; }
                    count++;
                    var dir = GetDir(fish.obj.transform.position, f.obj.transform.position);
                    //dir = f.obj.transform.position- fish.obj.transform.position;
                    var dist = dir.magnitude;

                 
                    //alignment += f.velocity;
                    if (dist > targetSeperation * 1.5f) {
                        continue;
                    }

                    if (dist < targetSeperation) {
                        seperation -= dir * (targetSeperation - dist) * 2;
                    }
                    else {
                        cohesion += dir * (dist - targetSeperation);
                    }
                    alignment += f.velocity;
                }

                if (count == 0) {
                    return new Vector3();
                }
                return (seperation.normalized * 2 + cohesion.normalized + alignment.normalized ).normalized;
                
            }

            void UpdateVelocity(ActiveFishEntry fish, Vector3 targetDir, Vector3 flocking) {
                targetDir = targetDir * (1 - fish.data.flocking) + flocking * fish.data.flocking;
                targetDir = new Vector3(
                   Mathf.Clamp(targetDir.x, -fish.data.maxSpeed.x, fish.data.maxSpeed.x),
                   Mathf.Clamp(targetDir.y, -fish.data.maxSpeed.y, fish.data.maxSpeed.y)
                   );
                fish.velocity = targetDir *fish.data.updateSpeed + fish.velocity * (1- fish.data.updateSpeed);
            }

            void AvoidPredator(ActiveFishEntry fish) {
                if (!fish.data.avoidPredator) {
                    return;
                }

                Vector3 dir = new Vector3();
                float closest = -1;
                foreach (var other in activeFishes) {
                    if (!other.data.predator) {
                        continue;
                    }

                    var d = GetDir(fish.obj.transform.position, other.obj.transform.position);
                    var dist = d.magnitude;

                    if (dist > 2) {
                        continue;
                    }
                    if (closest < 0 || dist < closest) {
                        closest = dist;
                        dir = d;
                    }
                }
                dir = dir.normalized / closest;

                dir = new Vector3(
                    Mathf.Clamp(dir.x, -fish.data.maxSpeed.x, fish.data.maxSpeed.x),
                    Mathf.Clamp(dir.y, -fish.data.maxSpeed.y, fish.data.maxSpeed.y)
                );
                if ( closest > 0) {
                    closest = Mathf.Clamp01(closest / 2);
                    fish.velocity = fish.velocity * closest - dir * (1 - closest);
                }
            }

            Vector3 UpdatePosition(ActiveFishEntry fish) {
                var p = fish.obj.transform.position;
                p += current * 0.02f * fish.mass + fish.velocity;
                fish.sr.flipX = fish.velocity.x < 0;
                return p;
            }

            Vector3 ScreenWrap(Vector3 p) {
                if (p.x < -1) {
                    p = new Vector3(width + 1, p.y, p.z);
                }
                if (p.y < -1) {
                    p = new Vector3(p.x, height + 1, p.z);
                }
                if (p.x > width + 1) {
                    p = new Vector3(-1, p.y, p.z);
                }
                if (p.y > height + 1) {
                    p = new Vector3(p.x, -1, p.z);
                }

                return p;
            }
        }

        Vector3 GetDir(Vector2 start, Vector2 target) {
            var direct = target - start;
            var result = direct;

            // wrap right
            if (direct.x < -width / 2 - 1 ) {
                result.x += width + 2; 
            }
            
            // wrap left
            if (direct.x > width / 2 + 1 ) {
                result.x -= width + 2; 
            }

            // wrap down
            if (direct.y > height / 2 + 1) {
                result.y -= height + 2;
            }

            // wrap up
            if (direct.y < -height / 2 - 1) {
                result.y += height + 2;
            }

            return result;
        }

        [System.Serializable]
        public class FishEntry {
            public int count;
            public GameObject src;
            public float updateSpeed = 0.05f;
            public Vector2 maxSpeed;
            [Range(0, 1)]
            public float flocking;
            public bool predator = false;
            public bool avoidPredator = false;
        }
   
        public class ActiveFishEntry {
            public GameObject obj;
            public int count;
            public FishEntry data;
            public Vector3 velocity;
            public Vector3 target;
            public int timer;
            public SpriteRenderer sr;
            public float mass;

            public ActiveFishEntry(GameObject obj, FishEntry data) {
                this.obj = obj;
                this.data = data;
                velocity = new Vector3(0.02f, 0, 0);
                sr = obj.GetComponent<SpriteRenderer>();
                mass = Random.Range(0.5f, 1);
            }
        }
    }
}