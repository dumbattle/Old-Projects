using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using DumbML;


namespace Tests
{
    public class BlasTests {
        [Test]
        public void MatrixMult1dx2d() {
            Tensor a = Tensor.FromArray(
                new[] { 1f, 2f, 3f }
            );
            Tensor b = Tensor.FromArray(
                new[,] {
                    { 1f, 2f, 3f},
                    { 1f, 2f, 3f},
                    { 1f, 2f, 3f}
                }
            );

            Tensor result = new Tensor(new[] { 3 });

            Blas.MatrixMult(a, b, result);

            Tensor expected = Tensor.FromArray(
                new[] { 6f, 12f, 18f }
            );

            Assert.True(result.CompareData(expected));
        }


        [Test]
        public void Conv2DPointWise_5x5x3__3x2() {
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

            Tensor result = new Tensor(new[] { 5, 5, 2 });

            Blas.Parallel.Convolution2DPointwise(a, b, result);
            Assert.True(result.CompareData(expected), $"{expected}\n{result}");
        }

        [Test]
        public void Conv2DDepthwise_Stride1x1_PadFalse_5x5x3_3x3x3() {
            float[,,] _a = {
                { { 1, -2, 3 }, { 4, -5, 6 }, { 7, 8, -9 }, { 1, -3, 5 }, { 5, -2, -6 }, },
                { { 3, 6, 4 }, { 1, 3, 2 }, { 7, 2, -6 }, { 4, 5, 2 }, { 8, 3, 7 }, },
                { { 1, 5, -7 }, { 1, 2, -5 }, { 1, 2, 4 }, { -1, 5,- 6 }, { 9, 0, -4 }, },
                { { 1, -4, 5 }, { 5, -4, 3 }, { 4, -4, 3 }, { 7, 5, 3 }, { -1, 2, 3 }, },
                { { -4, 5, 2 }, { 4, 7, -2 }, { 1, 6, -8 }, { 1, 5, 4 }, { 4, -5, 7 }, } };

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

            Tensor result = new Tensor(new[] { 3, 3, 3 });

            Blas.Parallel.Convolution2DDepthwise(a, b, result, stride: (1, 1), pad: false);

            Assert.True(result.CompareData(expected), $"e: {expected}\nr: {result}");
        }
    }
}
