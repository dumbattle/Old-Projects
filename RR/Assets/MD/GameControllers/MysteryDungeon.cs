using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MysteryDungeon {
    public class MysteryDungeon : MonoBehaviour {
        public const Orientation orientation = Orientation.horizontal;

        Game game;
        void Start() {
            game = new Game(20);
            game.Start();
        }


        void Update() {
            game.Update();
        }
        //private void OnDrawGizmos() {
        //    foreach (var h in new HexGrid(10)) {
        //        Gizmos.DrawSphere(h.ToCartesian(orientation, HexCoord.ROOT3 / 2), .1f);
        //    }
        //}
    }


}