using System.Collections.Generic;
using System.Linq;

namespace AntSim {
    public class AntStats {
        //all genes start at 1
        public Gene ga = new Gene( 0, 1, .1f);
        public Gene gb = new Gene( 0, 1, .1f);
        public Gene gc = new Gene( 0, 1, .1f);
        public Gene gd = new Gene( 0, 1, .1f);

        public Gene ge = new Gene( 0, 1, .1f);
        public Gene gf = new Gene( 0, 1, .1f);
        public Gene gg = new Gene( 0, 1, .1f);
        public Gene gh = new Gene( 0, 1, .1f);

        public CommonStats harvester, scout;


        public int HarvesterCarryCapacity;
        public float scoutPoiDecay;
        public float scoutExplore;
        public int scoutFoodThresh;


        void ComputeStats() {
            {
                harvester.speed = CalcStat(1, 10,
                    (ga, 2),
                    (gd, 1),
                    (gb, -1),
                    (gc, -2));
                harvester.sight = (int)CalcStat(3, 10,
                    (gc, 2),
                    (gg, 1),
                    (gf, -1),
                    (ga, -2));
                harvester.stanima = CalcStat(3, 10,
                    (gb, 2),
                    (ga, 1),
                    (gg, -1),
                    (gc, -2));
                harvester.stanimaDrain = CalcStat(.1f, .3f,
                    (gf, 2),
                    (gc, 1),
                    (gh, -1),
                    (ga, -2));
                harvester.health = (int)CalcStat(10, 50,
                    (gd, 2),
                    (ge, 1),
                    (gc, -1),
                    (gb, -2));
                harvester.attack = (int)CalcStat(1, 10,
                    (ge, 2),
                    (gf, 1),
                    (gd, -1),
                    (gb, -2));
                HarvesterCarryCapacity = (int)CalcStat(1, 10,
                    (gc, 2),
                    (gd, 1),
                    (ge, -1),
                    (gf, -2));
            }
            {
                scout.speed = CalcStat(5, 15,
                    (gd, 2),
                    (gc, 1),
                    (gb, -1),
                    (ge, -2));
                scout.sight = (int)CalcStat(5, 12,
                    (gh, 2),
                    (gb, 1),
                    (ge, -1),
                    (gf, -2));
                scout.stanima = CalcStat(4, 10,
                    (ge, 2),
                    (ga, 1),
                    (gc, -1),
                    (gg, -2));
                scout.stanimaDrain = CalcStat(.1f, .3f,
                    (ga, 2),
                    (ge, 1),
                    (gd, -1),
                    (gh, -2));
                scout.health = (int)CalcStat(10, 35,
                    (gg, 2),
                    (gb, 1),
                    (ga, -1),
                    (gh, -2));
                scout.attack = (int)CalcStat(1, 5,
                    (gh, 2),
                    (gg, 1),
                    (gf, -1),
                    (ge, -2));
                scoutPoiDecay = CalcStat(.01f, 1,
                    (gg, 2),
                    (gf, 1),
                    (gh, -1),
                    (ge, -2));
                scoutExplore = CalcStat(1, 5,
                    (gf, 2),
                    (gh, 1),
                    (gg, -1),
                    (ge, -2));
                scoutFoodThresh = (int)CalcStat(1, 50,
                    (gb, 2),
                    (gh, 1),
                    (ga, -1),
                    (gg, -2));
            }
        }

        float CalcStat(float min, float max, params (float val, int weight)[] genes) {
            float range = max - min;

            float total = 0;
            int weight = 0;
            foreach (var g in genes) {
                if (g.weight > 0) {
                    total += g.val * g.weight;
                    weight += g.weight;
                }
                else{
                    total -= (1 - g.val) * g.weight;
                    weight -= g.weight;
                }
            }
            return total / weight * range + min;
        }


        public AntStats() {
            harvester = new CommonStats();
            scout = new CommonStats();
            ComputeStats();
        }


        public AntStats (AntStats src) {
            ga = new Gene(src.ga);
            gb = new Gene(src.gb);
            gc = new Gene(src.gc);
            gd = new Gene(src.gd);
            ge = new Gene(src.ge);
            gf = new Gene(src.gf);
            gg = new Gene(src.gg);
            gh = new Gene(src.gh);

            harvester = new CommonStats();
            scout = new CommonStats();
            ComputeStats();
        }

        public AntStats Copy() {
            return new AntStats(this);
        }

        public void Mutate() {
            ga.Mutate();
            gb.Mutate();
            gc.Mutate();
            gd.Mutate();
            ge.Mutate();
            gf.Mutate();
            gg.Mutate();
            gh.Mutate();

            ComputeStats();
        }
        public class CommonStats {
            public float speed;
            public int sight;
            public float stanima;
            public float stanimaDrain;
            public int health;
            public int attack;
        }
        public override string ToString() {
            return
                $"Harvester speed:              \t{harvester.speed}\n" +
                $"Harvester sight:              \t{harvester.sight}\n" +
                $"Harvester stanima:            \t{harvester.stanima}\n" +
                $"Harvester stanima drain:      \t{harvester.stanimaDrain}\n" +
                $"Harvester health:             \t{harvester.health}\n" +
                $"Harvester attack:             \t{harvester.attack}\n" +
                $"Harvester carrying capacity:  \t{HarvesterCarryCapacity}\n" +
                $"Scout speed:               \t{scout.speed}\n" +
                $"Scout sight:               \t{scout.sight}\n" +
                $"Scout stanima:             \t{scout.stanima}\n" +
                $"Scout stanima drain:       \t{scout.stanimaDrain}\n" +
                $"Scout health:              \t{scout.health}\n" +
                $"Scout attack:              \t{scout.attack}\n" +
                $"Scout poi decay:           \t{scoutPoiDecay}\n" +
                $"Scout explore multiplier:  \t{scoutExplore}\n" +
                $"Scout food threshold:      \t{scoutFoodThresh}\n";
        }
    }
}
