using System;
using System.Collections.Generic;
using UnityEngine;

namespace AntSim {
    public class FoodField : MonoBehaviour{
        public static FoodField Main;
        public GameObject foodObj;
        public int foodAmount;
        HexMap<Food> field;

        public Food this[HexCoord h] {
            get => field[h];
            set => field[h] = value;
        }
        ObjectPool<Food> pool;

        void Awake() {
            Main = this;
            foodObj.SetActive(false);
        }

        public void Initialize(int radius) {
            field = new HexMap<Food>(radius);

            Func<Food> constructor = () => {
                var o = Instantiate(foodObj);
                return new Food(o);
            };

            pool = new ObjectPool<Food>(10, constructor);
        }
        public void SpawnFood(HexCoord h) {
            Food f = pool.Get();
            f.Activate(foodAmount, h);
            field[h] = f;
        }
        public void Remove(HexCoord h) {
            var f = field[h];
            pool.Return(f);
            f.obj.SetActive(false);
            field[h] = null;
        }

        public void Clear () {
            foreach (var f in field) {
                if (f != null) {
                    f.Harvest(f.amount);
                }
            }
        }
    }




    public class Food : IPoolable {
        public bool Active {get; set;}
        public int max;
        public int amount;
        public HexCoord hex;
        public GameObject obj;

        public Food(GameObject obj) {
            this.obj = obj;
            obj.SetActive(false);
        }

        public void Activate(int amount, HexCoord hex) {
            this.amount = max = amount;
            this.hex = hex;
            obj.SetActive(true);
            obj.transform.position = hex.ToCartesian(AntSim.orientation, AntSim.Main.scale);
            obj.transform.localScale = Vector3.one;
        }

        public int Harvest(int amnt) {
            int result = 0;

            if (amount < amnt) {
                result = amount;
            }
            else {
                result = amnt;
            }

            amount -= result;

            if (amount <= 0) {
                FoodField.Main.Remove(hex);
            }

            obj.transform.localScale = Vector3.one * (.5f * (amount + max) / max);
            return result;
        }
    }

}
