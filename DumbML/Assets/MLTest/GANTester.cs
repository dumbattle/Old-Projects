using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using DumbML;
using DumbML.Unity;

public class GANTester : MonoBehaviour {
    const int WIDTH = 40, HEIGHT = 32;
    public int noiseSize = 100;
    public int testInd = 0;

    public Optimizer disOpt, genOpt;

    public GameObject tile;
    public int codeSize = 10;

    public float loss;

    public NeuralNetworkAsset nna;
    public RGBADataSet Images;

    public float delay = 0;
    public int epoch = 0;
    public int batchSize = 8;
    public int saveInterval = 1;
    public int remaining;


    Stopwatch sw = new Stopwatch();
    Layer l;
    Point2 size = (WIDTH, HEIGHT);
    NeuralNetwork discriminator, generator, gan;

    Tensor[] input;
    SpriteRenderer[,] srcTiles, resultTiles;
    Tensor noise;
    IEnumerator ie;



    void Start() {
        noise = new Tensor(() => Random.value, 1, 1, noiseSize);

        Physics.autoSimulation = false;
        Physics2D.autoSimulation = false;
        SetupTiles();

        SetupNetwork();
        input = Images.LoadImages();
        for (int i = 0; i < input.Length; i++) {
            input[i] = input[i].Extend(1, 1);
        }

        //draw test img
        var img = input[testInd];

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                srcTiles[x, y].color = new Color(img[x, y, 0], img[x, y, 1], img[x, y, 2], img[x, y, 3]);
            }
        }

        //draw test output
        img = generator.Compute(noise);

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                resultTiles[x, y].color = new Color(img[x, y, 0], img[x, y, 1], img[x, y, 2], img[x, y, 3]);
            }
        }
        ie = Step();
        sw.Start();

        void SetupNetwork() {
            ActivationFunction af = Relu.Default;
            WeightInitializer wi = new WeightInitializer.Normal(-.5f, .5f);


            const bool Pad = true;
            const bool Bias = true;

            generator = new NeuralNetwork();
            generator.Add(new Deconv2DBuilder().FilterSize(5, 4).Channels(64).WeightInitializer(wi).Build());

            generator.Add(new Deconv2DBuilder().FilterSize(3, 3).Channels(64).WeightInitializer(wi).AF(af).Bias(Bias).Stride(2, 2).Pad(true).Build());
            generator.Add(new Deconv2DBuilder().FilterSize(3, 3).Channels(32).WeightInitializer(wi).AF(af).Bias(Bias).Stride(2, 2).Build());
            generator.Add(new Deconv2DBuilder().FilterSize(4, 4).Channels(16).WeightInitializer(wi).AF(af).Bias(Bias).Stride(2, 2).Build());
            generator.Add(new Deconv2DBuilder().FilterSize(3, 3).Channels(4).WeightInitializer(wi).AF(Sigmoid.Default).Pad(true).Bias(Bias).Build());


            discriminator = new NeuralNetwork();

            discriminator.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(16).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Stride(2, 2).Build());
            discriminator.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(32).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Stride(2, 2).Build());
            discriminator.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(64).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Stride(2, 2).Build());
            discriminator.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(64).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Build());
            discriminator.Add(new GlobalAveragePooling());
            discriminator.Add(new FullyConnected(1, wi));




            gan = new NeuralNetwork(1, 1, noiseSize);
            gan.Add(generator);
            gan.Add(discriminator);
            gan.Build();

            nna.Save(gan);

            disOpt = new Adam(discriminator, lr: .001f, lossFunction: new MSE());
            genOpt = new Adam(gan, lr: .001f, lossFunction: new MSE());

            //disOpt = new SGD(discriminator, lr: .1f, lossFunction: new CrossEntropy());
            //genOpt = new SGD(gan, lr: .1f, lossFunction: new CrossEntropy());
        }

    }
    private void Update() {
        ie.MoveNext();
    }
    void SetupTiles() {
        srcTiles = new SpriteRenderer[size.x, size.y];
        if (Camera.main != null) {
            Camera.main.orthographicSize = size.x * 3f / 4 + 1;
            Camera.main.transform.position = new Vector3(size.x / 2, size.y / 2, -10);
        }

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                GameObject g = Instantiate(tile, new Vector2(x - size.x / 2f - 1, y), Quaternion.identity);

                srcTiles[x, y] = g.GetComponent<SpriteRenderer>();
                srcTiles[x, y].color = Color.black;
            }
        }
        resultTiles = new SpriteRenderer[size.x, size.y];
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                GameObject g = Instantiate(tile, new Vector2(x + size.x / 2f + 1, y), Quaternion.identity);

                resultTiles[x, y] = g.GetComponent<SpriteRenderer>();
                resultTiles[x, y].color = Color.black;
            }
        }
        tile.SetActive(false);
    }



    IEnumerator Step() {
        //shuffle
        input = input.OrderBy(x => Random.value).ToArray();

        Tensor[] discriminatorInput = new Tensor[2 * batchSize];
        Tensor[] noise = new Tensor[batchSize];
        Tensor[] discriminatorTarget = new Tensor[2 * batchSize];
        Tensor[] genTarget = new Tensor[batchSize];

        for (int i = 0; i < batchSize; i++) {
            discriminatorTarget[i] = new Tensor(() => 1, 1);
            discriminatorTarget[i + batchSize] = new Tensor(() => -1, 1);

            genTarget[i] = new Tensor(() => 1, 1);
        }

        int index = 0;
        int numBatches = 0;


        for (int i = 0; i < input.Length; i++) {
            remaining = input.Length - i;
            noise[index] = GetNoise();

            discriminatorInput[index] = input[i];
            discriminatorInput[index + batchSize] = generator.Compute(noise[index]);

            index++;
            if (index == batchSize) {
                numBatches++;
                index = 0;

                //train generator
                discriminator.Trainable = false;
                genOpt.TrainBatch(noise, genTarget);
                //train discriminator
                discriminator.Trainable = true;
                loss = disOpt.TrainBatch(discriminatorInput, discriminatorTarget);

                
                yield return null;
            }
        }
        UpdateImg();
       ie = Step();
        epoch++;


        Tensor GetNoise() {
            return new Tensor(() => Random.value, 1, 1, noiseSize);
        }

        void UpdateImg() {
            var img = generator.Compute(this.noise);

            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    resultTiles[x, y].color = new Color(img[x, y, 0], img[x, y, 1], img[x, y, 2], img[x, y, 3]);
                }
            }
            if (epoch % saveInterval == 0) {
                nna.Save(gan);
            }
        }

    }
}
