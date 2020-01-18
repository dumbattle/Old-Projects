using UnityEngine;
using DumbML;
using System.Collections.Generic;

namespace Chess {
    public static class ChessTrainer {
        public static RingBuffer<Tensor> draws = new RingBuffer<Tensor>(1000);
        public static RingBuffer<Tensor> wins = new RingBuffer<Tensor>(1000);
        public static RingBuffer<Tensor> losses = new RingBuffer<Tensor>(1000);

        static Tensor d = new Tensor(() => 0, 1);
        static Tensor w = new Tensor(() => 1, 1);
        static Tensor l = new Tensor(() => -1, 1);
        
        public static void ClearMemory() {
            draws.Reset();
            wins.Reset();
            losses.Reset();
        }

        public static float Train(ChessModel model, int count) {

            if (draws.Count == 0 &&
                wins.Count == 0 &&
                losses.Count == 0) {
                return -1;
            }
            Tensor[] inputs = new Tensor[count * 3];
            Tensor[] labels = new Tensor[count * 3];
            
            if (draws.Count != 0) {
                for (int i = 0; i < count; i++) {
                    inputs[i] = draws[Random.Range(0, draws.Count)];
                    labels[i] = d;
                }
            }
            if (wins.Count != 0) {
                for (int i = 0; i < count; i++) {
                    inputs[i + count] = wins[Random.Range(0, wins.Count)];
                    labels[i + count] = w;
                }
            }
            if (losses.Count != 0) {
                for (int i = 0; i < count; i++) {
                    inputs[i + count + count] = losses[Random.Range(0, losses.Count)];
                    labels[i + count + count] = l;
                }
            }

            return model.main.Train(inputs, labels, count)[0];
        }
        public static Tensor[] GetUniqueStates() {
            List<Tensor> states = new List<Tensor>();
            foreach (var s in draws) {
                bool dupe = false;
                foreach (var m in states) {
                    if (m.CompareData(s)) {
                        dupe = true;
                        break;
                    }
                }
                if (!dupe) {
                    states.Add(s);
                }
            }

            foreach (var s in wins) {
                bool dupe = false;
                foreach (var m in states) {
                    if (m.CompareData(s)) {
                        dupe = true;
                        break;
                    }
                }
                if (!dupe) {
                    states.Add(s);
                }
            }

            foreach (var s in losses) {
                bool dupe = false;
                foreach (var m in states) {
                    if (m.CompareData(s)) {
                        dupe = true;
                        break;
                    }
                }
                if (!dupe) {
                    states.Add(s);
                }
            }
            return states.ToArray();

        }
    }
}
