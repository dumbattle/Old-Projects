using UnityEngine;

namespace AntSim {

    public static class Priority {
        public const float None = 0;
        public const float Low= 25;
        public const float Medium = 50;
        public const float High= 75;
        public const float Max = 100;
    }


    public abstract class AntAI {
        public AntAction current;
        public float aggression = 20;
        protected HungerAction hungerAction;
        protected AttackAction attackAction;
        
        Ant ant;


        public AntAI(Ant ant) {
            this.ant = ant;
            hungerAction = new HungerAction(ant);
            attackAction = new AttackAction(ant);
        }

        public void Update() {
            current = CheckForEnemies() ?? current;
            current = CheckStanima() ?? current;
            current = current.Update() ?? GetDefaultAction();
        }

        AntAction CheckStanima() {
            float s = ant.stanima -= ant.stanimaDecay * AntSim.DeltaTime;

            if (s <= 0) {
                ant.Die();
                return null;
            }

            if (s / ant.maxStanima < .2f && hungerAction.priority > current.priority) {
                return hungerAction;
            }

            return null;
        }


        AntAction CheckForEnemies () {
            if (aggression <= current.priority) {
                return null;
            }
            var e = LookForClosestEnemy();
            if (e == null) {
                return null;
            }

            return attackAction.Reset(e, aggression);

            
            Ant LookForClosestEnemy() {
                //search for food
                foreach (var h in new HexGrid(ant.sight, ant.hex)) { // look at each grid
                    if (!new HexGrid(AntSim.Main.mapRadius).IsInRange(h)) { // in map range?
                        continue;
                    }

                    var ants = AntSim.antMap.GetAnts(h);

                    if (ants == null || ants.Count == 0) {
                        continue;
                    }
                    foreach (var a in ants) {
                        if (a.colony != ant.colony) {
                            return a;
                        }
                    }
                   
                }
                return null;
            }
        }
        public abstract AntAction GetDefaultAction();


    }



    public class AttackAction : AntAction {
        Ant ant;
        Ant target;

        public AttackAction (Ant ant) : base(Priority.None) {
            this.ant = ant;
        }

        public AttackAction Reset(Ant target, float p) {
            priority = p;
            this.target = target;
            return this;
        }

        float timer = 3;
        float attackTimer = 0;
        public override AntAction Update() {
            timer -= AntSim.DeltaTime;
            attackTimer -= AntSim.DeltaTime;
            if (timer <= 0) {
                timer = 3;
                return null;
            }
            //check target is in range         
            Vector3 dir = target.transform.position - ant.transform.position;

            //attack if close
            if (dir.sqrMagnitude < Ant.DIST_THRESH) {
                return Attack();
            }
            else {
                //else move towards
                ant.Move(dir);
                return this;
            }
        }


        AntAction Attack() {
            if (attackTimer > 0) {
                return this;
            }
            attackTimer = 1;
            return target.TakeDamage(ant.attack) ? null : this ;
        }
    }


    public class HungerAction : AntAction {
        Ant ant;
        public HungerAction(Ant ant) : base(Priority.High) {
            this.ant = ant;
        }
        public override AntAction Update() {
            //if at food or home
            if (((Vector3)ant.hex.ToCartesian(AntSim.orientation, AntSim.Main.scale) - ant.transform.position).sqrMagnitude < Ant.DIST_THRESH) {

                if (FoodField.Main[ant.hex] != null) {
                    Food food = FoodField.Main[ant.hex];

                    if (food != null || food.amount > 0) {
                        int amount = food.Harvest((int)(ant.maxStanima - ant.stanima));
                        food.amount -= amount;
                        ant.stanima += amount;
                        return null;
                    }
                }
                if (ant.homeTile == ant.hex && ant.colony.food > 0) {
                    AntColony c = ant.colony;

                    int amount = Mathf.Min(c.food, (int)(ant.maxStanima - ant.stanima));
                    c.food -= amount;
                    ant.stanima += amount;
                    return null;
                }
            }
            //move to nearest food
            var f = LookForClosestFood(ant);
            if (f != null) {
                Vector3 dir = (Vector3)f.Value.ToCartesian(AntSim.orientation, AntSim.Main.scale) - ant.transform.position;
                ant.Move(dir);
            }
            else {
                Vector3 dir = (Vector3)ant.homeTile.ToCartesian(AntSim.orientation, AntSim.Main.scale) - ant.transform.position;
                ant.Move(dir);
            }
            return this;
        }
    }

    public abstract class AntAction {
        public float priority;

        public AntAction(float priority) {
            this.priority = priority;
        }
        public abstract AntAction Update();

        public static HexCoord? LookForClosestFood(Ant ant) {
            //search for food
            foreach (var h in new HexGrid(ant.sight, ant.hex)) { // look at each grid
                if (!new HexGrid(AntSim.Main.mapRadius).IsInRange(h)) { // in map range?
                    continue;
                }

                var food = FoodField.Main[h];

                if (food == null) { // has food?
                    continue;
                }

                return h;
            }
            return null;
        }

    }
}