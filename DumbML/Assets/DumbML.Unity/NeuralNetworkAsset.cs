using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

namespace DumbML.Unity {

    [CreateAssetMenu(fileName = "New Neural Network", menuName = "DumbML/New Neural Network")]
    public class NeuralNetworkAsset : ScriptableObject, ISerializationCallbackReceiver {
        public NeuralNetwork nn;

        public string SerializedData = "null";
        public bool ReadOnly = false;

        public int[] InputShape { get { return nn.inputShape; } }
        public int[] OutputShape { get { return nn.outputShape; } }

        public bool HasNetwork { get => SerializedData != "null"; }

        [System.NonSerialized]
        bool loaded = false;
    
        public NeuralNetwork Load() {
            return (NeuralNetwork)Layer.FromJson(SerializedData);
        }

        public void Clear() {
            if (ReadOnly) {
                return;
            }

            nn = null;
            SerializedData = "null";
        }


        public void Save(NeuralNetwork nn) {
            if (ReadOnly) {
                //throw instead??
                return;
            }


            if (nn == null || !nn.IsBuilt) {
                return;
            }

            SerializedData = nn.ToJSON();
            this.nn = Load();
        }

        public void OnAfterDeserialize() {
            if (SerializedData != "null" && !loaded ) {
                nn = Load();
                loaded = true;
            }
        }
        public void OnBeforeSerialize() {
            //Save(nn);
        }
    }
}