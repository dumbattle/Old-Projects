using System.Collections;
using System.Linq;
using UnityEngine;
using DumbML;

public class UnitController : MonoBehaviour {
    public static int unitCount =0;

    static NeuralNetwork brain;
    //static Append appendLayer;
    static ReinforcementTrainer2 trainer;

    public float minMoveDelay = 1;
    public float maxMoveDelay = 2;
    public float moveSpeed = 1;
    public int sight = 10;
    public UnitStats stats;
    float timer = 0;
    bool isMoving;

    IEnumerator currentAction;
    public Block currentBlock;
    public float gestation = 100;



    Tensor previousState;
    int previousAction;
    float reward = 0;

    private void Start() {
        if(brain == null) {
            //9 outputs : idle + 8 move
            brain = new NeuralNetwork(sight * 2 + 1, sight * 2 + 1, 4);
            //brain.Add(new Convolution2D((3, 3), 8, () => Random.value * 2 - 1, ActivationFunction.DoubleLeakyRelu));
            brain.Add(new MaxPool());
            //brain.Add(new Convolution2D((3, 3), 8, () => Random.value * 2 - 1, ActivationFunction.DoubleLeakyRelu));
            brain.Add(new MaxPool());
            var l =brain.Add(new Flatten());
            brain.Add(new FullyConnected(10, () => Random.value * 2 - 1, ActivationFunction.DoubleLeakyRelu));
            brain.Add(new FullyConnected(9, () => Random.value * 2 - 1));

            brain.Build();

            print(l.outputShape[0]);
            trainer = new ReinforcementTrainer2(brain, 9);
            //trainer.exploreStart = .5f;
            trainer.exploreDecay = .00001f;
        }

        timer = Random.Range(minMoveDelay, maxMoveDelay);
        currentAction = Idle();
    }


    void Update() {
        Step();
        currentAction.MoveNext();
    }


    IEnumerator Idle() {
        timer = Random.Range(minMoveDelay, maxMoveDelay);

        while (true) {
            if (currentBlock.water) {
                stats.UpdateThirst(3 * MapManager.timeStep);
            }
            else {
                stats.UpdateThirst(-1 * MapManager.timeStep);
            }


            stats.UpdateStanima(5 * MapManager.timeStep * (stats.thirst.Clamp(10,50) / 50));
            timer -= MapManager.timeStep;
            if (timer <= 0) {
                Action();
                yield break;
            }
            yield return null;
        }
    }
    void Step() {
        if (stats.health <= 0) {
            Die();
            return;
        }
        stats.UpdateHunger(-.5f  * MapManager.timeStep);

        float deltaHealth = (100 - stats.hunger.Clamp(0,50) - stats.thirst.Clamp(0, 50)) / 5;
        stats.UpdateHealth((1-deltaHealth )* MapManager.timeStep);

        if (stats.health > 50) {
            gestation -= MapManager.timeStep;
        }

        if (gestation <=0) {
            gestation = 100;
            SpawnChild();
        }

    }

    void Die() {
        stats.Refresh();
        trainer.Add(previousState, previousAction, 0, null);
        previousState = null;
        previousAction = -1;

        trainer.Train();

    }

    void SpawnChild () {
        return;
        if (unitCount > 50) {
            return;
        }
        unitCount++;
        Block b = currentBlock;
        GameObject c = Instantiate(MapManager.current.chicken, new Vector3(b.position.x, .5f, b.position.z), Quaternion.identity);
        UnitController uc = c.GetComponent<UnitController>();
        uc.currentBlock = b;
    }
    void Action() {
        //appendLayer.SetValues(stats.health / 100f, stats.hunger / 100, stats.thirst / 100);
        Tensor input = GetInput();
        //Tensor output = brain.Compute(input);
        //int max = 0;
        //for (int i = 0; i < output.Shape[0]; i++) {
        //    if (output[i] > output[max]) {
        //        max = i;
        //    }
        //}
        var max = trainer.Next(input);
       

        if (previousState != null) {
            trainer.Add(previousState, previousAction, reward, input, reward > .5f ? 32 : 1);
            reward = 0;
        }

        previousState = input;
        previousAction = max;

        if (max <= 7) {
            //move
            Point2 direction = Point2.Directions[max];

            Block b = MapManager.current[direction + currentBlock.index];
            if (b == null || !b.ground || b.obstructed) {
                //idle 
                currentAction = Idle();
                reward = -1;
                return;
            }

            Vector3 newPos = new Vector3(b.position.x, transform.position.y, b.position.z);

            stats.UpdateStanima(-15);
            stats.UpdateThirst(-5);

            transform.forward = newPos - transform.position;

            currentBlock = b;
            currentAction = Move(newPos);
            return;
        }

        //Default
        currentAction = Idle();

        return;
    }

    IEnumerator Move(Vector3 newPos) {
        Vector3 oldPos = transform.position;

        timer = 0;


        while (true) {
            timer += MapManager.timeStep;
            float pos = timer * moveSpeed / (newPos - oldPos).magnitude;
            transform.position = Vector3.Lerp(oldPos, newPos, pos);

            if (pos >= 1) {
                //arrive
                EnterBlock();
                currentAction = Idle();
                yield break; ;
            }

            yield return null;
        }

    }
    void EnterBlock () {
        Item i = currentBlock.item;
        if (i == null) {
            return;
        }
        Food f = i as Food;
        if (f == null) {
            return;
        }
        else {
            currentBlock.RemoveItem();
            Destroy(i.gameObject);
            stats.UpdateHunger(f.calories);
            MapManager.current.SpawnFood();
            reward = 1;
        }

    }

    Tensor GetInput () {
        //blocks have 3 properties
        Tensor result = new Tensor(sight * 2 + 1, sight * 2 + 1, 4);

        Circle c = new Circle(Point2.zero, sight);


        foreach (Point2 p in c) {
            Block b = MapManager.current[p + currentBlock.index];

            if (b != null) {
                result[p.x + sight, p.y + sight, 0] = b.ground ? 1 : 0;
                result[p.x + sight, p.y + sight, 1] = b.water ? 1 : 0;
                result[p.x + sight, p.y + sight, 2] = b.obstructed ? 1 : 0;
                result[p.x + sight, p.y + sight, 3] = (b.item as Food) != null ? 1 : 0;
            }
        }

        return result;
    }
}

