using UnityEngine;
using DumbML;
using System;
using System.Collections.Generic;


namespace DumbML {
    
    [CreateAssetMenu(fileName = "New Model", menuName = "DumbMl/New Model")]
    public class ModelAsset : ScriptableObject {
        [SerializeField]
        SerializedModel sModel;

        public void Save(Model m) {
            sModel = new SerializedModel();
            sModel.Save(m);
        }

        public Model LoadModel() {
            return sModel.Load();
        }
    }


    [Serializable]
    class SerializedModel {
        [SerializeField]
        Parameters[] parameters;

        [SerializeField]
        _ChildOp[] _childOp;

        [SerializeField]
        string[] _opTypes;

        [SerializeField]
        string[] _names;

        public bool HasModel { get => _opTypes.Length != 0; }

        public void Save(Model m) {
            var ops = m.forward.GetOperations();

            parameters = new Parameters[ops.Count];
            _childOp = new _ChildOp[ops.Count];
            _opTypes = new string[ops.Count];
            _names = new string[ops.Count];

            for (int i = 0; i < ops.Count; i++) {
                if (ops[i] is Variable v) {
                    parameters[i] = new Parameters() {
                        val = (float[])v.Value.value.Clone(),
                        shape = v.Value.Shape,
                    };
                }
                else if (ops[i] is Constant c) {
                    parameters[i] = new Parameters() {
                        val = (float[])c.Value.value.Clone(),
                        shape = c.Value.Shape,
                    };
                }
                else if (ops[i] is BroadcastScalar bc) {
                    parameters[i] = new Parameters() {
                        shape = bc.shape,
                    };
                }
                else if (ops[i] is Conv2DDepth c2dd) {
                    parameters[i] = new Parameters() {
                        stride = c2dd.stride,
                        pad = c2dd.pad,
                    };
                }
                else if (ops[i] is Placeholder ph) {
                    parameters[i] = new Parameters() {
                        shape = ph.shape,
                    };
                }
                else if (ops[i] is DuplicateOperation dupe) {
                    parameters[i] = new Parameters() {
                        ind = ops.FindIndex((x) => x == dupe.i)
                    };
                }

                _opTypes[i] = ops[i].GetType().Name;
                _childOp[i] = new _ChildOp(ops[i].inner.Length);

                for (int j = 0; j < ops[i].inner.Length; j++) {
                    int ind = ops.FindIndex((x) => ops[i].inner[j] == x);
                    _childOp[i].childInds[j] = ind;
                }
                _names[i] = ops[i].Name;
            }
        }

        public Model Load() {
            if (!HasModel) {
                return null;
            }
            Operation[] ops = new Operation[_opTypes.Length];
            Model result = null;

            Operation root = BuildOp(0);

            result = new Model(root);

            return result;


            Operation BuildOp(int ind) {
                Operation res;
                if (ops[ind] != null) {
                    return ops[ind];
                }

                Operation[] children = new Operation[_childOp[ind].childInds.Length];

                for (int i = 0; i < children.Length; i++) {
                    children[i] = BuildOp(_childOp[ind].childInds[i]);
                }

                switch (_opTypes[ind]) {
                    case "Abs":
                        res = new Abs(children[0]);
                        break;
                    case "Add":
                        res = new Add(children[0], children[1]);
                        break;
                    case "Append":
                        res = new Append(children);
                        break;
                    case "BroadcastScalar":
                        res = new BroadcastScalar(children[0], parameters[ind].shape);
                        break;
                    case "Constant":
                        res = new Constant(parameters[ind].Tensor());
                        break;
                    case "Conv2DDepth":
                        res = new Conv2DDepth(children[0], children[1], parameters[ind].stride, parameters[ind].pad);
                        break;
                    case "Conv2DPoint":
                        res = new Conv2DPoint(children[0], children[1]);
                        break;
                    case "Divide":
                        res = new Divide(children[0], children[1]);
                        break;
                    case "DuplicateOperation":
                        res = new DuplicateOperation(BuildOp(parameters[ind].ind));
                        break;
                    case "Exp":
                        res = new Exp(children[0]);
                        break;
                    case "FlattenOp":
                        res = new FlattenOp(children[0]);
                        break;
                    case "LeakyRelu":
                        res = new LeakyRelu(children[0]);
                        break;
                    case "Log":
                        res = new Log(children[0]);
                        break;
                    case "MatrixMult":
                        res = new MatrixMult(children[0], children[1]);
                        break;
                    case "Multiply":
                        res = new Multiply(children[0], children[1]);
                        break;
                    case "Placeholder":
                        res = new Placeholder(_names[ind], parameters[ind].shape);
                        break;
                    case "Relu":
                        res = new Relu(children[0]);
                        break;
                    case "Sigmoid":
                        res = new Sigmoid(children[0]);
                        break;
                    case "Square":
                        res = new Square(children[0]);
                        break;
                    case "Subtract":
                        res = new Subtract(children[0], children[1]);
                        break;
                    case "Sum":
                        res = new Sum(children[0]);
                        break;
                    case "Tanh":
                        res = new Tanh(children[0]);
                        break;
                    case "Variable":
                        res = new Variable(parameters[ind].Tensor());
                        break;
                    default:
                        throw new ArgumentException($"Could not deserialize operation of type: {_opTypes[ind]}");
                }

                if (res != null) {
                    res.SetName(_names[ind]);
                }
                ops[ind] = res;
                return res;
            }
        }


        [Serializable]
        struct Parameters {
            public float[] val;
            public int[] shape;
            public (int, int) stride;
            public bool pad;
            public int ind;

            public Tensor Tensor() {
                IEnumerator<float> ie = ((IEnumerable<float>)val).GetEnumerator();
                return new Tensor(() => { ie.MoveNext(); return ie.Current; }, shape);
            }
        }
        [Serializable]
        struct _ChildOp {
            public int[] childInds;
            public _ChildOp(int numChild) {
                childInds = new int[numChild];
            }
        }
    }
}