using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using DumbML;


namespace Tests {
    public class OpTests_Simple {
        static void print(object msg) {
            Debug.Log(msg.ToString());
        }
        [Test]
        public void Abs() {
            float[] _a = { -3, -2, -1, 0, 1, 2, 3 };
            float[] _e = { 3, 2, 1, 0, 1, 2, 3 };
            Tensor expected = Tensor.FromArray(_e);
            Constant input = new Constant(Tensor.FromArray(_a));
            Operation op = new Abs(input);
            Tensor result = op.Eval();

            // Eval
            Assert.True(result.CompareData(expected), $"Eval Result incorrect:\ne: {expected}\nr: {result}");
            Assert.True(op.value.CompareData(expected), $"Value incorrect:\ne: {expected}\nv: {result}");

            // Backwards
            Gradients g = new Gradients(input);

            float[] _er = { 1, 1, 1, 1, 1, 1, 1 };
            Tensor er = Tensor.FromArray(_er);

            float[] _eg = { -1, -1, -1, 1, 1, 1, 1 };
            Tensor expectedGradient = Tensor.FromArray(_eg);

            op.Backwards(g);
            Tensor gr = g[input];

            Assert.True(gr.CompareData(expectedGradient), $"Incorrect Gradient:\ne: {expectedGradient}\nr: {gr}");

            // copy
            Operation copy = op.Copy();
            var copyOutput = copy.Eval();

            Assert.True(copyOutput.CompareData(result), $"Incorrect Copy Result:\nc: {copyOutput}\nr: {result}");
        }

        [Test]
        public void Add() {
            // 1D
            {
                float[] _a = { -3, -2, -1, 0, 1, 2, 3 };
                float[] _b = { .1f, .2f, .3f, .4f, .5f, .6f, .7f };
                float[] _e = { -2.9f, -1.8f, -.7f, .4f, 1.5f, 2.6f, 3.7f };

                Tensor a = Tensor.FromArray(_a);
                Tensor b = Tensor.FromArray(_b);
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Add(a, b);
                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval 1D Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value 1D incorrect:\ne: {expected}\nv: {result}");
            }

            // 2D
            {
                float[,] _a = { { 1, 2, 3 }, { 1, 2, 3 }, { 1, 2, 3 } };
                float[,] _b = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
                float[,] _e = { { 2, 4, 6 }, { 5, 7, 9 }, { 8, 10, 12 } };

                Tensor a = Tensor.FromArray(_a);
                Tensor b = Tensor.FromArray(_b);
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Add(a, b);
                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval Result 2D incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value 2D incorrect:\ne: {expected}\nv: {result}");
            }

            // backwards
            {
                float[] _a = { -3, -2, -1, 0, 1, 2, 3 };
                float[] _b = { .1f, .2f, .3f, .4f, .5f, .6f, .7f };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));

                Operation op = new Add(a, b);
                Tensor result = op.Eval();


                Gradients g = new Gradients(a, b);

                op.Backwards(g);

                Tensor agrad = g[a];
                Tensor bgrad = g[b];

                float[] _ea = { 1, 1, 1, 1, 1, 1, 1 };
                float[] _eb = { 1, 1, 1, 1, 1, 1, 1 };

                Tensor ea = Tensor.FromArray(_ea);
                Tensor eb = Tensor.FromArray(_eb);
                Assert.True(ea.CompareData(agrad), $"Incorrect Gradient A:\ne: {ea}\ng: {agrad}");
                Assert.True(eb.CompareData(bgrad), $"Incorrect Gradient B:\ne: {eb}\ng: {bgrad}");
            }

            // Copy
            {
                float[] _a = { -3, -2, -1, 0, 1, 2, 3 };
                float[] _b = { .1f, .2f, .3f, .4f, .5f, .6f, .7f };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));

                Operation op = new Add(a, b);
                Tensor result = op.Eval();

                Operation copy = op.Copy();

                Tensor copyResult = copy.Eval();

                Assert.True(copyResult.CompareData(result), $"Copy Value incorrect:\ne: {result}\nc: {copyResult}");
            }
        }

        [Test]
        public void Append() {
            //Eval 1D
            {
                float[] _a = { 1, 2, 3, 4 };
                float[] _b = { 5, 6, 7, 8, 9 };
                float[] _e = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Append(a, b);

                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval 1D Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value 1D incorrect:\ne: {expected}\nv: {result}");
            }

            //Eval 2D
            {
                float[,] _a = { { 1, 2, 3 }, { 4, 5, 6 } };
                float[,] _b = { { 7, 8, 9 } };
                float[,] _e = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Append(a, b);

                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval 2D Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value 2D incorrect:\ne: {expected}\nv: {result}");
            }

            //Eval Multiple
            {
                float[] _a = { 1, 2, 3, 4 };
                float[] _b = { 5, 6, 7, 8, 9 };
                float[] _c = { 8, 7, 6, 5, 4, 3, 2, 1 };
                float[] _e = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));
                Constant c = new Constant(Tensor.FromArray(_c));
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Append(a, b, c);

                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval 1D Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value 1D incorrect:\ne: {expected}\nv: {result}");
            }

            // Backwards
            {
                float[] _a = { 1, 2, 3, 4 };
                float[] _b = { 5, 6, 7, 8, 9 };
                float[] _c = { 8, 7, 6, 5, 4, 3, 2, 1 };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));
                Constant c = new Constant(Tensor.FromArray(_c));

                Operation op = new Append(a, b, c);

                Tensor result = op.Eval();

                Gradients g = new Gradients(a, b, c);

                op.Backwards(g);

                Tensor agrad = g[a];
                Tensor bgrad = g[b];
                Tensor cgrad = g[c];

                float[] _ea = { 1, 1, 1, 1 };
                float[] _eb = { 1, 1, 1, 1, 1 };
                float[] _ec = { 1, 1, 1, 1, 1, 1, 1, 1 };

                Tensor ea = Tensor.FromArray(_ea);
                Tensor eb = Tensor.FromArray(_eb);
                Tensor ec = Tensor.FromArray(_ec);

                Assert.True(ea.CompareData(agrad), $"Incorrect Gradient A:\ne: {ea}\ng: {agrad}");
                Assert.True(eb.CompareData(bgrad), $"Incorrect Gradient B:\ne: {eb}\ng: {bgrad}");
                Assert.True(ec.CompareData(cgrad), $"Incorrect Gradient C:\ne: {ec}\ng: {cgrad}");
            }

            // Copy
            {
                float[,] _a = { { 1, 2, 3 }, { 4, 5, 6 } };
                float[,] _b = { { 7, 8, 9 } };
                float[,] _c = { { 2, 4, 6 } };
                float[,] _e = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 }, { 2, 4, 6 } };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));
                Constant c = new Constant(Tensor.FromArray(_c));
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Append(a, b, c);

                Tensor result = op.Eval();

                Operation copy = op.Copy();

                Tensor copyResult = copy.Eval();

                Assert.True(copyResult.CompareData(result), $"Copy Value incorrect:\ne: {result}\nc: {copyResult}");
            }
        }

        [Test]
        public void BroadcastScalar() {
            // Eval
            {
                float[] _a = { -3 };
                int[] shape = { 2, 3 };
                float[,] _e = { { -3, -3, -3 }, { -3, -3, -3 } };

                Tensor a = Tensor.FromArray(_a);
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new BroadcastScalar(a, shape);

                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval 1D Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value 1D incorrect:\ne: {expected}\nv: {result}");
            }

            // backwards
            {
                float[] _a = { -3 };
                int[] shape = { 2, 3 };

                Constant a = new Constant(Tensor.FromArray(_a));

                Operation op = new BroadcastScalar(a, shape);

                Tensor result = op.Eval();



                Gradients g = new Gradients(a);

                op.Backwards(g);

                Tensor grad = g[a];

                float[] _e = { 6 };

                Tensor ea = Tensor.FromArray(_e);
                Assert.True(ea.CompareData(grad), $"Incorrect Gradient:\ne: {ea}\ng: {grad}");
            }

            // Copy
            {
                float[] _a = { -3, -2, -1, 0, 1, 2, 3 };
                float[] _b = { .1f, .2f, .3f, .4f, .5f, .6f, .7f };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));

                Operation op = new Add(a, b);
                Tensor result = op.Eval();

                Operation copy = op.Copy();

                Tensor copyResult = copy.Eval();

                Assert.True(copyResult.CompareData(result), $"Copy Value incorrect:\ne: {result}\nc: {copyResult}");
            }
        }

        [Test]
        public void Constant() {
            // Eval
            {
                float[] _a = { 15, 153, -12 };
                float[] _e = { 15, 153, -12 };

                Tensor a = Tensor.FromArray(_a);
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Constant(a);

                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval 1D Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value 1D incorrect:\ne: {expected}\nv: {result}");
            }

            // backwards
            {
                float[] _a = { 15, 153, -12 };

                Tensor a = Tensor.FromArray(_a);

                Operation op = new Constant(a);

                Tensor result = op.Eval();

                Gradients g = new Gradients(op);

                op.Backwards(g);

                Tensor grad = g[op];

                float[] _e = { 1, 1, 1 };

                Tensor e = Tensor.FromArray(_e);
                Assert.True(e.CompareData(grad), $"Incorrect Gradient:\ne: {e}\ng: {grad}");
            }

            // Copy
            {
                float[] _a = { 15, 153, -12 };

                Tensor a = Tensor.FromArray(_a);

                Operation op = new Constant(a);

                Tensor result = op.Eval();

                Operation copy = op.Copy();

                Tensor copyResult = copy.Eval();

                Assert.True(copyResult.CompareData(result), $"Copy Value incorrect:\ne: {result}\nc: {copyResult}");
            }
        }

        [Test]
        public void Conv2DDepth() {
            // Eval - stride 1x1 no padding
            {
                // these values were copied from the BlasTest script
                float[,,] _a = {
                    { { 1, -2, 3 }, { 4, -5, 6 }, { 7, 8, -9 }, { 1, -3, 5 }, { 5, -2, -6 }, },
                    { { 3, 6, 4 }, { 1, 3, 2 }, { 7, 2, -6 }, { 4, 5, 2 }, { 8, 3, 7 }, },
                    { { 1, 5, -7 }, { 1, 2, -5 }, { 1, 2, 4 }, { -1, 5,- 6 }, { 9, 0, -4 }, },
                    { { 1, -4, 5 }, { 5, -4, 3 }, { 4, -4, 3 }, { 7, 5, 3 }, { -1, 2, 3 }, },
                    { { -4, 5, 2 }, { 4, 7, -2 }, { 1, 6, -8 }, { 1, 5, 4 }, { 4, -5, 7 }, }
                };

                float[,,] _b = {
                    { { 1,  1,  1}, {1,  1, 1 }, {1,  1,  1} },
                    { { 1,  1,  1}, {1,  0, 1 }, {1,  1,  1} },
                    { { -1, 1, -1}, {1, -1, 1 }, {-1, 1, -1} }
                };
                float[,,] _e = {
                    { { 22,14,-2}, {25,13,15 }, {21,5,-13} },
                    { { 14,14,-13}, {5,22,-12 }, {32,5,-6} },
                    { { 20,5,7}, {13,16,-8 }, {15,1,8} }
                };


                Tensor a = Tensor.FromArray(_a);
                Tensor b = Tensor.FromArray(_b);
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Conv2DDepth(a, b, (1, 1), false);

                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval stride 1x1 no pad Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value stride 1x1 no pad incorrect:\ne: {expected}\nv: {result}");
            }

            // Eval - stride 1x1 with padding
            {
                // these values were copied from the BlasTest script
                float[,,] _a = {
                    { { 1, -2, 3 }, { 4, -5, 6 }, { 7, 8, -9 }, { 1, -3, 5 }, { 5, -2, -6 }, },
                    { { 3, 6, 4 }, { 1, 3, 2 }, { 7, 2, -6 }, { 4, 5, 2 }, { 8, 3, 7 }, },
                    { { 1, 5, -7 }, { 1, 2, -5 }, { 1, 2, 4 }, { -1, 5,- 6 }, { 9, 0, -4 }, },
                    { { 1, -4, 5 }, { 5, -4, 3 }, { 4, -4, 3 }, { 7, 5, 3 }, { -1, 2, 3 }, },
                    { { -4, 5, 2 }, { 4, 7, -2 }, { 1, 6, -8 }, { 1, 5, 4 }, { 4, -5, 7 }, }
                };

                float[,,] _b = {
                    { { 1,  1,  1}, {1,  1, 1 }, {1,  1,  1} },
                    { { 1,  1,  1}, {1,  0, 1 }, {1,  1,  1} },
                    { { -1, 1, -1}, {1, -1, 1 }, {-1, 1, -1} }
                };
                float[,,] _e = {
                    { { 7, -8, 11 }, { 3, 11, 4 }, { 14, -2, -8 }, { 2, 6, -9 }, { 10, -1, 4 } },
                    { { 9, -7, 13 }, { 22, 14, -2 }, { 25, 13, 15 }, { 21, 5, -13 }, { 28, 5, 10 } },
                    { { 2, 11, -4 }, { 14, 14, -13 }, { 5, 22, -12 }, { 32, 5, -6 }, { 12, 16, -1 } },
                    { { 0, 5, 0 }, { 20, 5, 7 }, { 13, 16, -8 }, { 15, 1, 8 }, { 17, 20, -1 } },
                    { { 6, -1, 8 }, { 11, -1, 3 }, { 22, 9, 3 }, { 16, 4, 12 }, { 11, 12, 17 } }
                };

                Tensor a = Tensor.FromArray(_a);
                Tensor b = Tensor.FromArray(_b);
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Conv2DDepth(a, b, (1, 1), true);

                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval stride 1x1 with pad Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value stride 1x1 with pad incorrect:\ne: {expected}\nv: {result}");
            }

            // Eval - stride 2x2 no padding
            {
                // these values were copied from the BlasTest script
                float[,,] _a = {
                    { { 1, -2, 3 }, { 4, -5, 6 }, { 7, 8, -9 }, { 1, -3, 5 }, { 5, -2, -6 }, },
                    { { 3, 6, 4 }, { 1, 3, 2 }, { 7, 2, -6 }, { 4, 5, 2 }, { 8, 3, 7 }, },
                    { { 1, 5, -7 }, { 1, 2, -5 }, { 1, 2, 4 }, { -1, 5,- 6 }, { 9, 0, -4 }, },
                    { { 1, -4, 5 }, { 5, -4, 3 }, { 4, -4, 3 }, { 7, 5, 3 }, { -1, 2, 3 }, },
                    { { -4, 5, 2 }, { 4, 7, -2 }, { 1, 6, -8 }, { 1, 5, 4 }, { 4, -5, 7 }, }
                };

                float[,,] _b = {
                    { { 1,  1,  1}, {1,  1, 1 }, {1,  1,  1} },
                    { { 1,  1,  1}, {1,  0, 1 }, {1,  1,  1} },
                    { { -1, 1, -1}, {1, -1, 1 }, {-1, 1, -1} }
                };
                float[,,] _e = {
                    { { 22,14,-2}, {21,5,-13} },
                    { { 20,5,7}, {15,1,8} }
                };

                Tensor a = Tensor.FromArray(_a);
                Tensor b = Tensor.FromArray(_b);
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Conv2DDepth(a, b, (2, 2), false);

                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval stride 2x2 no pad Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value stride 2x2 no pad incorrect:\ne: {expected}\nv: {result}");
            }


            // Eval - stride 2x2 with padding
            {
                // these values were copied from the BlasTest script
                float[,,] _a = {
                    { { 1, -2, 3 }, { 4, -5, 6 }, { 7, 8, -9 }, { 1, -3, 5 }, { 5, -2, -6 }, },
                    { { 3, 6, 4 }, { 1, 3, 2 }, { 7, 2, -6 }, { 4, 5, 2 }, { 8, 3, 7 }, },
                    { { 1, 5, -7 }, { 1, 2, -5 }, { 1, 2, 4 }, { -1, 5,- 6 }, { 9, 0, -4 }, },
                    { { 1, -4, 5 }, { 5, -4, 3 }, { 4, -4, 3 }, { 7, 5, 3 }, { -1, 2, 3 }, },
                    { { -4, 5, 2 }, { 4, 7, -2 }, { 1, 6, -8 }, { 1, 5, 4 }, { 4, -5, 7 }, }
                };

                float[,,] _b = {
                    { { 1,  1,  1}, {1,  1, 1 }, {1,  1,  1} },
                    { { 1,  1,  1}, {1,  0, 1 }, {1,  1,  1} },
                    { { -1, 1, -1}, {1, -1, 1 }, {-1, 1, -1} }
                };
                float[,,] _e = {
                    { { 7, -8, 11 }, { 14, -2, -8 }, { 10, -1, 4 } },
                    { { 2, 11, -4 }, { 5, 22, -12 }, { 12, 16, -1 } },
                    { { 6, -1, 8 }, { 22, 9, 3 }, { 11, 12, 17 } }
                };

                Tensor a = Tensor.FromArray(_a);
                Tensor b = Tensor.FromArray(_b);
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Conv2DDepth(a, b, (2, 2), true);

                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval stride 2x2 with pad Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value stride 2x2 with pad incorrect:\ne: {expected}\nv: {result}");
            }



            // backwards - stride 1x1 no padding
            {
                float[,,] _a = {
                    { { 1, 3 }, { 4, 6 }, { 7, -9 }, { 1, 5 }, { 5, -6 } },
                    { { 3, 4 }, { 1, 2 }, { 7, -6 }, { 4, 2 }, { 8, 7 } },
                    { { 1, -7 }, { 1, -5 }, { 1, 4 }, { -1, -6 }, { 9, -4 } },
                    { { 1, 5 }, { 5, 3 }, { 4, 3 }, { 7, 3 }, { -1, 3 } },
                    { { -4, 2 }, { 4, -2 }, { 1, -8 }, { 1, 4 }, { 4, 7 } }
                };

                float[,,] _b = {
                    { { 1, 1}, {1, -1 }, {-1, -1} },
                    { { -1, 1}, {1, -1 }, {1, 1} },
                    { { 1,-1}, {-1, 1 }, {-1, -1} }
                };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));

                Operation op = new Conv2DDepth(a, b, (1, 1), false);

                Tensor result = op.Eval();

                Gradients g = new Gradients(a, b);

                op.Backwards(g);

                Tensor gradA = g[a];
                Tensor gradB = g[b];

                float[,,] _ea = {
                    { { 1, 1 }, { 2, 0 }, { 1, -1 }, { 0, -2 }, { -1, -1 } },
                    { { 0, 2 }, { 2, 0 }, { 2, 0 }, { 2, -  2 }, { 0, 0 } },
                    { { 1, 1 }, { 2, 0 }, { 1, -1 }, { 0, -2 }, { -1, -1 } },
                    { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } },
                    { { 1, -1 }, { 0, 0 }, { -1, -1 }, { -2, 0 }, { -1, -1 } }
                };

                float[,,] _eb = {
                    { { 26, -8}, { 25, -7 }, { 41, -13  } },
                    { { 24, 3 }, { 29, 0 },  {38, 6} },
                    { { 14, -5 }, { 23, -4 }, { 25, 6} }
                };


                Tensor ea = Tensor.FromArray(_ea);
                Tensor eb = Tensor.FromArray(_eb);
                Assert.True(ea.CompareData(gradA), $"Incorrect Gradient stride 1x1 no pad Input:\ne: {ea}\ng: {gradA}");
                Assert.True(eb.CompareData(gradB), $"Incorrect Gradient stride 1x1 no pad Filter:\ne: {eb}\ng: {gradB}");
            }

            // backwards - stride 2x2 with padding
            {
                float[,,] _a = {
                    { { 1, 3 }, { 4, 6 }, { 7, -9 }, { 1, 5 }, { 5, -6 } },
                    { { 3, 4 }, { 1, 2 }, { 7, -6 }, { 4, 2 }, { 8, 7 } },
                    { { 1, -7 }, { 1, -5 }, { 1, 4 }, { -1, -6 }, { 9, -4 } },
                    { { 1, 5 }, { 5, 3 }, { 4, 3 }, { 7, 3 }, { -1, 3 } },
                    { { -4, 2 }, { 4, -2 }, { 1, -8 }, { 1, 4 }, { 4, 7 } }
                };

                float[,,] _b = {
                    { { 1, 1}, {1, -1 }, {-1, -1} },
                    { { -1, 1}, {1, -1 }, {1, 1} },
                    { { 1,-1}, {-1, 1 }, {-1, -1} }
                };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));

                Operation op = new Conv2DDepth(a, b, (2, 2), true);

                Tensor result = op.Eval();

                Gradients g = new Gradients(a, b);

                op.Backwards(g);

                Tensor gradA = g[a];
                Tensor gradB = g[b];

                float[,,] _ea = {
                    { { 1, -1 }, { 0, 2 }, { 1, -1 }, { 0, 2 }, { 1, -1  } },
                    { { 0, 0 }, { 0, -2 }, { 0, 0 }, { 0, -2 }, { 0, 0 } },
                    { { 1, -1 }, { 0, 2 }, { 1, -1  }, { 0, 2 }, { 1, -1  } },
                    { { 0, 0 }, { 0, -2 }, { 0, 0 }, { 0, -2 }, { 0, 0 } },
                    { { 1, -1  }, { 0, 2 }, { 1, -1  }, { 0, 2 }, { 1, -1  } }
                };

                float[,,] _eb = {
                    { { 17, 10 }, { 22, 16 }, { 17, 10 } },
                    { { 10, 2 }, { 25, -18 },  {10, 2 } },
                    { { 17, 10  }, { 22, 16 }, { 17, 10 } }
                };


                Tensor ea = Tensor.FromArray(_ea);
                Tensor eb = Tensor.FromArray(_eb);
                Assert.True(ea.CompareData(gradA), $"Incorrect Gradient stride 2x2 with pad Input:\ne: {ea}\ng: {gradA}");
                Assert.True(eb.CompareData(gradB), $"Incorrect Gradient stride 2x2 with pad Filter:\ne: {eb}\ng: {gradB}");
            }

            // Copy
            {
                float[,,] _a = {
                    { { 1, 3 }, { 4, 6 }, { 7, -9 }, { 1, 5 }, { 5, -6 } },
                    { { 3, 4 }, { 1, 2 }, { 7, -6 }, { 4, 2 }, { 8, 7 } },
                    { { 1, -7 }, { 1, -5 }, { 1, 4 }, { -1, -6 }, { 9, -4 } },
                    { { 1, 5 }, { 5, 3 }, { 4, 3 }, { 7, 3 }, { -1, 3 } },
                    { { -4, 2 }, { 4, -2 }, { 1, -8 }, { 1, 4 }, { 4, 7 } }
                };

                float[,,] _b = {
                    { { 1, 1}, {1, -1 }, {-1, -1} },
                    { { -1, 1}, {1, -1 }, {1, 1} },
                    { { 1,-1}, {-1, 1 }, {-1, -1} }
                };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));

                Operation op = new Conv2DDepth(a, b, (2, 2), true);

                Tensor result = op.Eval();


                Operation copy = op.Copy();

                Tensor copyResult = copy.Eval();

                Assert.True(copyResult.CompareData(result), $"Copy Value incorrect:\ne: {result}\nc: {copyResult}");
            }
        }

        [Test]
        public void Conv2DPointwise() {
            {
                float[,,] _a = {
                { { 1, -2, 3 }, { 4, -5, 6 }, { 7, 8, -9 }, { 1, -3, 5 }, { 5, -2, -6 }, },
                { { 3, 6, 4 }, { 1, 3, 2 }, { 7, 2, -6 }, { 4, 5, 2 }, { 8, 3, 7 }, },
                { { 1, 5, -7 }, { 1, 2, -5 }, { 1, 2, 4 }, { -1, 5,- 6 }, { 9, 0, -4 }, },
                { { 1, -4, 5 }, { 5, -4, 3 }, { 4, -4, 3 }, { 7, 5, 3 }, { -1, 2, 3 }, },
                { { -4, 5, 2 }, { 4, 7, -2 }, { 1, 6, -8 }, { 1, 5, 4 }, { 4, -5, 7 }, } };
                float[,] _b = {
                { 1, -1 },
                { 1, 1 },
                { 1, -1 }
            };
                float[,,] _e = {
                { { 2, -6}, { 5, -15 }, { 6, 10 }, { 3, -9 }, { -3, -1 }, },
                { { 13, -1 }, { 6, 0 }, { 3, 1 }, { 11, -1 }, { 18, -12 }, },
                { { -1, 11 }, { -2, 6 }, { 7, -3 }, { -2,12 }, { 5, -5 }, },
                { { 2, -10 }, { 4, -12 }, { 3, -11 }, { 15, -5 }, { 4, 0 }, },
                { { 3, 7 }, { 9, 5 }, { -1, 13 }, { 10, 0 }, { 6, -16 }, }
            };

                Tensor a = Tensor.FromArray(_a);
                Tensor b = Tensor.FromArray(_b);
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new Conv2DPoint(a, b);
                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value incorrect:\ne: {expected}\nv: {result}");
            }

            // backwards
            {
                float[,,] _a = {
                    { { 1, -2,  3 }, { 4, -5,  6 }, { 7,  8, -9 }, {  1, -3,  5 }, {  5, -2, -6 }, },
                    { { 3,  6,  4 }, { 1,  3,  2 }, { 7,  2, -6 }, {  4,  5,  2 }, {  8,  3,  7 }, },
                    { { 1,  5, -7 }, { 1,  2, -5 }, { 1,  2,  4 }, { -1,  5, -6 }, {  9,  0, -4 }, },
                    { { 1, -4,  5 }, { 5, -4,  3 }, { 4, -4,  3 }, {  7,  5,  3 }, { -1,  2,  3 }, },
                    { { -4, 5,  2 }, { 4,  7, -2 }, { 1,  6, -8 }, {  1,  5,  4 }, {  4, -5,  7 }, }
                };
                float[,] _b = {
                    { 1, -1 },
                    { 1, 1 },
                    { 1, -1 }
                };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));

                Operation op = new Conv2DPoint(a, b);
                Tensor result = op.Eval();


                Gradients g = new Gradients(a, b);

                op.Backwards(g);

                Tensor agrad = g[a];
                Tensor bgrad = g[b];

                float[,,] _ea = {
                    { { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, },
                    { { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, },
                    { { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, },
                    { { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, },
                    { { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, { 0, 2, 0 }, }
                };
                float[,] _eb ={
                    { 74, 74 },
                    { 42, 42 },
                    { 10, 10 }
                };

                Tensor ea = Tensor.FromArray(_ea);
                Tensor eb = Tensor.FromArray(_eb);
                Assert.True(ea.CompareData(agrad), $"Incorrect Gradient A:\ne: {ea}\ng: {agrad}");
                Assert.True(eb.CompareData(bgrad), $"Incorrect Gradient B:\ne: {eb}\ng: {bgrad}");
            }

            // Copy
            {
                float[,,] _a = {
                    { { 1, -2,  3 }, { 4, -5,  6 }, { 7,  8, -9 }, {  1, -3,  5 }, {  5, -2, -6 }, },
                    { { 3,  6,  4 }, { 1,  3,  2 }, { 7,  2, -6 }, {  4,  5,  2 }, {  8,  3,  7 }, },
                    { { 1,  5, -7 }, { 1,  2, -5 }, { 1,  2,  4 }, { -1,  5, -6 }, {  9,  0, -4 }, },
                    { { 1, -4,  5 }, { 5, -4,  3 }, { 4, -4,  3 }, {  7,  5,  3 }, { -1,  2,  3 }, },
                    { { -4, 5,  2 }, { 4,  7, -2 }, { 1,  6, -8 }, {  1,  5,  4 }, {  4, -5,  7 }, }
                };
                float[,] _b = {
                    { 1, -1 },
                    { 1, 1 },
                    { 1, -1 }
                };

                Constant a = new Constant(Tensor.FromArray(_a));
                Constant b = new Constant(Tensor.FromArray(_b));

                Operation op = new Conv2DPoint(a, b);
                Tensor result = op.Eval();


                Operation copy = op.Copy();

                Tensor copyResult = copy.Eval();

                Assert.True(copyResult.CompareData(result), $"Copy Value incorrect:\ne: {result}\nc: {copyResult}");
            }
        }


        [Test]
        public void GlobalAveragePooling() {
            {

                float[,,] _a = new float[3, 3, 2];
                // first channel
                _a[0, 0, 0] = 1;
                _a[0, 1, 0] = 1;
                _a[0, 2, 0] = 1;
                _a[1, 0, 0] = 1;
                _a[1, 1, 0] = 1;
                _a[1, 2, 0] = 1;
                _a[2, 0, 0] = 1;
                _a[2, 1, 0] = 1;
                _a[2, 2, 0] = 1;
                // second channel
                _a[0, 0, 1] = 1;
                _a[0, 1, 1] = -1;
                _a[0, 2, 1] = 4;
                _a[1, 0, 1] = 2;
                _a[1, 1, 1] = -5;
                _a[1, 2, 1] = -1;
                _a[2, 0, 1] = 2;
                _a[2, 1, 1] = -3;
                _a[2, 2, 1] = 1;

                float[] _e = new float[] { 1, 0 };


                Tensor a = Tensor.FromArray(_a);
                Tensor expected = Tensor.FromArray(_e);

                Operation op = new GlobalAveragePooling(a);
                Tensor result = op.Eval();

                Assert.True(result.CompareData(expected), $"Eval Result incorrect:\ne: {expected}\nr: {result}");
                Assert.True(op.value.CompareData(expected), $"Value incorrect:\ne: {expected}\nv: {result}");
            }
        }
    }
}
