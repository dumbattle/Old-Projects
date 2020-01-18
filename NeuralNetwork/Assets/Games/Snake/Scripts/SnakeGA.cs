using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class SnakeGA : MonoBehaviour {    
    public int mapSize = 10;
    public int numGames = 10;
    SnakeGame[] games;
    TileState[,] map;
    

    void Start() {
        BuildMap();
        games = new SnakeGame[numGames];
        for (int i = 0; i < numGames; i++) {
            games[i] = new SnakeGame(mapSize);
        }
    }

    void Update() {
        bool stillGoing = false;
        foreach (var g in games) {
            if (g.Alive) {
                g.Next();
                stillGoing = true;
            }
        }
        if (!stillGoing) {
            PrintData();
            //evolution
            var parents = ChooseParent();
            for (int i = 0; i < games.Length; i++) {
                games[i].Mutate(parents[i]);
                if (i< 10) {
                    games[i] = new SnakeGame(mapSize);
                }
            }
            for (int i = 0; i < games.Length; i++) {
                if(i< 10) {
                    continue;
                }
                games[i].Reset();

                games[i].ApplyNewGenes();
            }
        }
    }

    SnakeGame[] ChooseParent() {
        SnakeGame[] result = new SnakeGame[numGames];

        games = (from x in games orderby x.score descending select x).ToArray();
        float total = 0;
        for (int i = 0; i < games.Length / 2; i++) {
            total += games[i].score *games[i].score;
        }
        for (int i = 0; i < numGames; i++) {
            float dart = Random.Range(0, total);

            for (int j = 0; j < games.Length / 2; j++) {
                dart -= games[j].score * games[j].score;
                if (dart <= 0) {
                    result[i] = games[j];
                    break;
                }
            }
        }

        return result;
    }
    void PrintData() {
        //average

        float avg = 0;
        float max = 0;
        foreach (var g in games) {
            avg += g.score;
            max = Mathf.Max(max, g.score);
        }
        avg /= games.Length;
        //stdDev
        float stdDev = 0;
        foreach (var g in games) {
            stdDev += (g.score - avg) * (g.score - avg);
        }
        stdDev /= games.Length;
        stdDev = Mathf.Sqrt(stdDev);
        //max
        print($"Max: {max} Average: {avg} StdDev: {stdDev}");
    }
    void OnDrawGizmos() {
        if (map == null) {
            return;
        }
        UpdateMap();
        for (int x = 0; x < mapSize; x++) {
            for (int y = 0; y < mapSize; y++) {
                Gizmos.color = GetColor(map[x, y]);
                Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one);
            }
        }

        void UpdateMap() {
            //set all as empty
            for (int x = 0; x < mapSize; x++) {
                for (int y = 0; y < mapSize; y++) {
                    map[x, y] = TileState.empty;
                }
            }
            //set food tiles
            foreach (var g in games) {
                if (g.Alive) {
                    map[g.food.x, g.food.y] = TileState.food;
                }
            }
            //set snake tiles
            foreach (var g in games) {
                if (g.Alive) {
                    map[g.snake.position.x, g.snake.position.y] = TileState.snake;
                }
            }
        }
        Color GetColor(TileState ts) {
            switch (ts) {
                case TileState.empty:
                    return Color.black;
                case TileState.food:
                    return Color.red;
                case TileState.snake:
                    return Color.green;
            }

            return Color.black;
        }
    }


    void BuildMap() {
        map = new TileState[mapSize, mapSize];
    }

}

public class SnakeGame {
    public bool Alive { get; private set; }
    public Snake snake;
    public Vector2Int food;

    public int score = 0;
    int hunger = 100;
    int mapSize;

    public SnakeGame(int mapSize) {
        this.mapSize = mapSize;
        Reset();
    }
    public void Reset () {
        Alive = true;
        snake = new Snake(mapSize / 2, mapSize / 2, 1, mapSize);
        //snake = new SnakeConv(mapSize / 2, mapSize / 2, 1,mapSize);
        NewFood();
        score = 1;
        hunger = 100;
    }
    void NewFood() {
        Vector2Int newFood = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));

        if (snake.CheckTile(newFood.x, newFood.y)) {
            NewFood();
        }
        else {
            food = newFood;
        }

    }

    public bool Next () {
        hunger--;
        if (hunger <= 0) {
            Alive = false;
            return false;
        }
        var dest = snake.GetNextMove(food) + snake.position;
        //eat food
        if (dest == food) {          
            snake.Move(dest);
            score += 1;
            NewFood();
            hunger += 100;
            return true;
        }

        //out of bounds
        if (dest.x < 0 || dest.x >= mapSize || dest.y < 0 || dest.y >= mapSize) {
            Alive = false;
            return false;
        }

        snake.Move(dest);
        return true;
    }
    Tensor[] newGenes;
    public void Mutate(SnakeGame parent) {
        Variable[] parentGenes = parent.snake.brain.GetWeights();
        newGenes = new Tensor[parentGenes.Length];

        for (int i = 0; i < newGenes.Length; i++) {
            newGenes[i] = parentGenes[i].Value + new Tensor(() => Random.Range(-.1f, .1f), parentGenes[i].shape);
        }
    }
    public void ApplyNewGenes() {
        Variable[] genes = snake.brain.GetWeights();

        for (int i = 0; i < genes.Length; i++) {
            genes[i].Value = newGenes[i];
        }
    }
    public bool CheckTileForSnake (int x, int y) {
        return snake.CheckTile(x, y);
    }
    public bool CheckTileForFood (int x, int y) {
        return food.x == x && food.y == y;
    }
}


public class Snake {
    public NeuralNetwork brain;
    public Vector2Int position;
    protected Tensor input;
    protected int mapSize;

    public Snake(int x, int y, int bodySize, int mapSize) {
        position = new Vector2Int(x, y);
        this.mapSize = mapSize;
        CreateBrain();
    }
    public bool CheckTile (int x, int y) {
        return position.x == x && position.y == y;
    }

    protected virtual void CreateBrain() {
        input = new Tensor(4);

        brain = new NeuralNetwork(4);
        brain.Add(new FullyConnected(4, bias: false));
        brain.Build();

        var a = brain.GetWeights()[0];

        //return;
        //manual brain setup
        //move up
        a.Value[0, 0] = 0;
        a.Value[1, 0] = -1;
        a.Value[2, 0] = 0;
        a.Value[3, 0] = 1;
        //move right
        a.Value[0, 1] = -1;
        a.Value[1, 1] = 0;
        a.Value[2, 1] = 1;
        a.Value[3, 1] = 0;
        //move down
        a.Value[0, 2] = 0;
        a.Value[1, 2] = 1;
        a.Value[2, 2] = 0;
        a.Value[3, 2] = -1;
        //move left
        a.Value[0, 3] = 1;
        a.Value[1, 3] = 0;
        a.Value[2, 3] = -1;
        a.Value[3, 3] = 0;
    }
    public virtual Vector2Int GetNextMove(Vector2Int food) {
        input[0] = 1f * position.x / mapSize;
        input[1] = 1f * position.y / mapSize;
        input[2] = 1f * food.x / mapSize;
        input[3] = 1f * food.y / mapSize;

        var output = brain.Compute(input);

        int max = 0;
        for (int i = 1; i < 4; i++) {
            if (output[i] > output[max]) {
                max = i;
            }
        }
        switch( max) {
            case 0:
                return Vector2Int.up;
            case 1:
                return Vector2Int.right;
            case 2:
                return Vector2Int.down;
            case 3:
                return Vector2Int.left;
        }
        return Vector2Int.up;
    }
    public void Move(Vector2Int head) {
        position = head;
    }
}
public class SnakeConv :Snake {

    public SnakeConv(int x, int y, int bodySize, int mapSize) : base(x,y,bodySize, mapSize) {}
    protected override void CreateBrain() {
        input = new Tensor(mapSize, mapSize,2);
        brain = new NeuralNetwork(mapSize, mapSize,2);
        brain.Add(new Convolution2D(2, stride: (2, 2), pad: true)); // [18,18,1]
        brain.Add(new Convolution2D(2, stride: (2, 2), pad: true)); // [6,6,1]
        brain.Add(new Convolution2D(2, stride: (2, 2), pad: true)); // [3,3,1]
        brain.Add(new Flatten());
        brain.Add(new FullyConnected(4, bias: false));
        brain.Build();


    }
    public override Vector2Int GetNextMove(Vector2Int food) {
        input.PointWise((a) => 0);
        input[food.x, food.y, 0] = 1;
        input[position.x, position.y, 1] = 1;

        var output = brain.Compute(input);

        int max = 0;
        for (int i = 1; i < 4; i++) {
            if (output[i] > output[max]) {
                max = i;
            }
        }
        switch (max) {
            case 0:
                return Vector2Int.up;
            case 1:
                return Vector2Int.right;
            case 2:
                return Vector2Int.down;
            case 3:
                return Vector2Int.left;
        }
        return Vector2Int.up;
    }
}
enum TileState {
    empty,
    food,
    snake
}