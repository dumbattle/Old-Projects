using DumbML;
using Flappy;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Swimming {
    public enum MoveInput {
        none,
        up,
        down
    }
    public class SalmonGame {
        public float playerPosition;
        public List<(int, Vector2)> activeObstacles = new List<(int, Vector2)>();
        public int score;
        public bool done;
        public SalmonParameters parameters;
        Obstacle[] obstacles;

        Channel scoreChannel;
        float playerVelocity;
        int obstacleSpawnTimer;
        Tensor stateTensor = new Tensor(4);

        public SalmonGame(SalmonParameters parameters, Obstacle[] obstacles) {
            this.parameters = parameters;
            this.obstacles = obstacles;
            scoreChannel = Channel.New("Score");
            scoreChannel.autoYRange = true;
        }

        public void Reset() {
            scoreChannel.Feed(score);
            done = false;
            playerPosition = parameters.playableHeight / 2f;
            activeObstacles.Clear();
            obstacleSpawnTimer = 0;
            SpawnObstacle();
            score = 0;
            playerVelocity = 0;
        }


        public float Update(MoveInput movement) {
            score++;

            if (movement == MoveInput.up) {
                playerVelocity += parameters.envParams.moveSpeed;
            }
            else if (movement == MoveInput.down) {
                playerVelocity -= parameters.envParams.moveSpeed;
            }
            else {
                playerVelocity *= parameters.swimDecay;
            }
            playerVelocity = Mathf.Clamp(playerVelocity, -parameters.envParams.moveSpeed, parameters.envParams.moveSpeed);
            playerPosition += playerVelocity;
            SpawnObstacle();
            MoveObstacles();

            var collided = CheckPlayerCollision();

            if (collided) {
                done = true;
                return parameters.envParams.dieScore;
            }
            return parameters.envParams.aliveScore;
        }


        void MoveObstacles() {
            bool removeFirst = false;
            for (int i = 0; i < activeObstacles.Count; i++) {
                var (ind, pos) = activeObstacles[i];
                var ob = obstacles[ind];
                pos.x -= ob.speed + parameters.envParams.swimSpeed;

                activeObstacles[i] = (ind, pos);


                if (i == 0) {
                    removeFirst = pos.x + ob.size.x < 0;
                }
            }

            if (removeFirst) {
                activeObstacles.RemoveAt(0);
            }
        }

        void SpawnObstacle() {
            obstacleSpawnTimer -= 1;

            if (obstacleSpawnTimer > 0) {
                return;
            }
            obstacleSpawnTimer = parameters.envParams.obstaclePeriod;


            var obstacleIndex = UnityEngine.Random.Range(0, obstacles.Length);
            var ob = obstacles[obstacleIndex];
            var pos = UnityEngine.Random.Range(
                ob.minDistFromBottom,
                parameters.playableHeight - ob.minDistFromTop - ob.size.y);

            activeObstacles.Add((obstacleIndex, new Vector2(parameters.playableWidth, pos)));
        }

        bool CheckPlayerCollision() {
            if (playerPosition < 0) {
                return true;
            }
            if (playerPosition > parameters.playableHeight) {
                return true;
            }
            var pr = parameters.playerDiameter / 2;
            Vector2 playerMin = new Vector2(0, playerPosition - pr);
            Vector2 playerMax = new Vector2(pr * 2, playerPosition + pr);


            foreach (var (index, pos) in activeObstacles) {
                Vector2 obMin = pos;
                Vector2 obMax = pos + obstacles[index].size;

                bool isColliding =
                    playerMin.x < obMax.x &&
                    playerMax.x > obMin.x &&
                    playerMin.y < obMax.y &&
                    playerMax.y > obMin.y;

                if (isColliding) {
                    return true;
                }
            }
            return false;
        }
        
        public Tensor GetState() {
            /// bird height
            /// bird velocity
            /// closest block
            /// block height

            if (activeObstacles.Count == 0) {
                stateTensor[1] = 1;
                stateTensor[2] = 0;
                stateTensor[3] = 0;
            }
            else {
                var (index, pos) = activeObstacles[0];
                stateTensor[1] = pos.x / parameters.playableWidth;
                stateTensor[2] = pos.y / parameters.playableHeight;
                stateTensor[3] = (pos.y + obstacles[index].size.y) / parameters.playableHeight;
            }
            stateTensor[0] = playerPosition / parameters.playableHeight;
            return stateTensor;
        }
        //public void ToTensor(Tensor<float> result) {
        // 
        //}
    }
}