using UnityEngine;
using System;
using System.Collections.Generic;

namespace DumbML {
    [CreateAssetMenu(fileName = "New Weights", menuName = "DumbMl/New Weights")]
    public class ModelWeightsAsset : ScriptableObject {
        [SerializeField]
        TensorData[] _data;
        public bool HasData => _data.Length != 0;

        public void Save(Variable[] data) {
            if (_data.Length == data.Length) {
                for (int i = 0; i < data.Length; i++) {
                    _data[i].val.Clear();
                    _data[i].val.AddRange(data[i].value.value);
                    _data[i].shape.Clear();
                    _data[i].shape.AddRange(data[i].value.Shape);
                    _data[i].Name = data[i].Name;
                }
            }
            else {

                _data = new TensorData[data.Length];
                for (int i = 0; i < data.Length; i++) {
                    _data[i] = new TensorData() {
                        val = new List<float>(data[i].value.value),
                        shape = new List<int>(data[i].value.Shape),
                        Name = data[i].Name
                    };
                }
            }
        }

        public Tensor[] Load() {
            if (_data.Length == 0) {
                return null;
            }

            Tensor[] result = new Tensor[_data.Length];


            for (int i = 0; i < result.Length; i++) {
                result[i] = _data[i].ToTensor();

            }
            return result;
        }

        [Serializable]
        struct TensorData {
            [HideInInspector]
            public string Name;
            public List<float> val;
            public List<int> shape;

            public Tensor ToTensor() {
                IEnumerator<float> ie = ((IEnumerable<float>)val).GetEnumerator();
                return new Tensor(() => { ie.MoveNext(); return ie.Current; }, shape);
            }
        }
    }

}