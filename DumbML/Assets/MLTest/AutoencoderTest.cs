using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DumbML;
using DumbML.Unity;


public class AutoencoderTest : MonoBehaviour {
    const int WIDTH = 40, HEIGHT = 32;
    public int testInd = 0;

    Point2 size = (WIDTH, HEIGHT);
    NeuralNetwork encoder, decoder, autoencoder;
    public Optimizer o;

    public GameObject tile;
    public int codeSize = 10;
    

    //Tensor testImg;
    SpriteRenderer[,] srcTiles, resultTiles;
    public float loss;

    public NeuralNetworkAsset nna;
    public RGBADataSet Images;

    Tensor[] input;
    Tensor testImg;
    public float delay = 0;
    //public int count = 0;
    public int epoch = 0;
    public int batchSize = 8;
    public int saveInterval = 1;

    Stopwatch sw = new Stopwatch();
    Layer l;

    IEnumerator<float> ie;

    void Start() {
        Physics.autoSimulation = false;
        Physics2D.autoSimulation = false;
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
        img = autoencoder.Compute(img);

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                resultTiles[x, y].color = new Color(img[x, y, 0], img[x, y, 1], img[x, y, 2], img[x, y, 3]);
            }
        }
        ie = o.TrainIE(input,input,batchSize);
        sw.Start();
        testImg = input[testInd];
    }


    void Update() {
        if (!ie.MoveNext()) {
            input = input.OrderBy(x => Random.value).ToArray();
            //testImages =( from x in testImages orderby UnityEngine.Random.value select x).ToArray();
            ie = o.TrainIE(input, input, batchSize);
            ie.MoveNext();
            epoch++;
            if (epoch % saveInterval == 0) {
                nna.Save(autoencoder);
            }
            loss = ie.Current;
            UpdateImg();
        }

        delay *= .9f;
        delay += sw.ElapsedMilliseconds * .1f;
        sw.Restart();

    }


    
    private void UpdateImg() {
        var img = autoencoder.Compute(testImg);

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                resultTiles[x, y].color = new Color(img[x, y, 0], img[x, y, 1], img[x, y, 2], img[x, y, 3]);
            }
        }
        //count++;
        //if (count % 1000 == 0) {
        //    nna.Save(autoencoder);
        //}
    }

    void SetupNetwork() {
        if (nna.HasNetwork) {
            autoencoder = nna.Load();
        }
        else {


            WeightInitializer wi = new WeightInitializer.Normal(-.5f, .5f);


            encoder = new NeuralNetwork(size.x, size.y, 4);
            decoder = new NeuralNetwork(size.x, size.y, 16);
            const bool Pad = true;
            const bool Bias = true;


            ActivationFunction af = Relu.Default;
            encoder.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(16).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Stride(2, 2).Build());
            //encoder.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(16).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Pad(true).Build());
            encoder.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(32).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Stride(2, 2).Build());
            //encoder.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(32).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Pad(true).Build());
            encoder.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(64).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Stride(2, 2).Build());
            //encoder.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(64).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Pad(true).Build());
            encoder.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(128).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Build());




            decoder.Add(new Deconv2DBuilder().FilterSize(3, 3).Channels(64).WeightInitializer(wi).AF(af).Bias(Bias).Stride(2, 2).Pad(true).Build());
            //decoder.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(64).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Pad(true).Build());
            decoder.Add(new Deconv2DBuilder().FilterSize(3, 3).Channels(32).WeightInitializer(wi).AF(af).Bias(Bias).Stride(2, 2).Build());
            //decoder.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(32).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Pad(true).Build());
            decoder.Add(new Deconv2DBuilder().FilterSize(4, 4).Channels(16).WeightInitializer(wi).AF(af).Bias(Bias).Stride(2, 2).Build());
            //decoder.Add(new Conv2DBuilder().FilterSize(3, 3).Channels(16).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Pad(true).Build());

            decoder.Add(new Deconv2DBuilder().FilterSize(3, 3).Channels(4).WeightInitializer(wi).AF(Sigmoid.Default).Pad(true).Bias(Bias).Build());

            //decoder.Add(new Deconv2DBuilder().FilterSize(3, 3).Channels(32).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Build());
            //decoder.Add(new UpSample());
            //decoder.Add(new Deconv2DBuilder().FilterSize(3, 3).Channels(16).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Build());
            //decoder.Add(new UpSample());
            //decoder.Add(new Deconv2DBuilder().FilterSize(3, 3).Channels(16).WeightInitializer(wi).AF(af).Pad(Pad).Bias(Bias).Build());
            //decoder.Add(new UpSample());
            //decoder.Add(new Deconv2DBuilder().FilterSize(3, 3).Channels(4).WeightInitializer(wi).AF(Sigmoid.Default).Pad(Pad).Bias(Bias).Build());



            autoencoder = new NeuralNetwork(size.x, size.y, 4);
            autoencoder.Add(encoder);
            autoencoder.Add(decoder);
            autoencoder.Build();

            nna.Save(autoencoder);
        }
        float lr = .001f;

        //o = new SGD(autoencoder, lr: .1f, lossFunction: new BinaryAlphaLoss());
        o = new Adam(autoencoder, lr: lr, lossFunction: new BinaryAlphaLoss());
        //o = new AdaLerp(autoencoder, lr: lr, finalLR: lr, lossFunction: new BinaryAlphaLoss());
    }

    Tensor CreateImage() {
        Tensor img = new Tensor(() => Random.value, size.x, size.y, 3);
        return img;
    }

}

public static class BatchTrain {

    public static IEnumerator<float> TrainIE(this Optimizer o, Tensor[] input, Tensor[] target, int batchSize = 8) {
        Tensor[] batchInput = new Tensor[batchSize];
        Tensor[] batchTarget = new Tensor[batchSize];
        float loss = 0;
        int count = 0;
        int numBatches = 0;


        for (int i = 0; i < input.Length; i++) {
            batchInput[count] = input[i];
            batchTarget[count] = target[i];

            count++;

            if (count == batchSize) {
                numBatches++;
                loss += o.TrainBatch(batchInput, batchTarget);
                count = 0;
                batchInput = new Tensor[batchSize];
                batchTarget = new Tensor[batchSize];

                yield return loss / numBatches;
            }
        }

    }
}