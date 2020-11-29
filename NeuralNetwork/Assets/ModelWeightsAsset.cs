﻿using UnityEngine;
using System;
using System.Collections.Generic;

namespace DumbML {
    [CreateAssetMenu(fileName = "New Weights", menuName = "DumbMl/New Weights")]
    public class ModelWeightsAsset : ScriptableObject {
        [SerializeField]
        TensorData[] _data;
        public bool HasData => _data.Length != 0;

        public void Save(Variable[] data) {
            _data = new TensorData[data.Length];
            for (int i = 0; i < data.Length; i++) {
                _data[i] = new TensorData() {
                    val = data[i].value.value,
                    shape = data[i].value.Shape,
                    Name = data[i].Name
                };
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
            public float[] val;
            public int[] shape;

            public Tensor ToTensor() {
                IEnumerator<float> ie = ((IEnumerable<float>)val).GetEnumerator();
                return new Tensor(() => { ie.MoveNext(); return ie.Current; }, shape);
            }
        }
    }

}