using System.Collections.Generic;

namespace MysteryDungeon {
    public class TurnManager {
        Game game;
        UnitManager unitManager => game.unitManager;

        LinkedList<GameAction> actions = new LinkedList<GameAction>();
        GameAction nextAction = null;

        bool playingAnims = false;

        public TurnManager(Game g) {
            game = g;
        }

        public void Update() {
            if (playingAnims) {
                var finished = UpdateAnims();

                if (finished) {
                    playingAnims = false;
                }
            }
            else {
                var finished = GetActions();

                if (finished) {
                    playingAnims = true;
                    Update();//potential infinite loop
                }
            }
        }



        bool UpdateAnims() {
            bool finished = true;

            foreach (var a in actions) {
                if (!a.Update()) {
                    finished = false;
                }
            }

            if (finished) {
                actions.Clear();
                if (nextAction != null) {
                    actions.AddFirst(nextAction);
                }
                nextAction = null;
                return true;
            }

            return false;
        }



        bool GetActions() {
            var unit = unitManager.currentUnit;

            var a = unit.ai.Next();

            if (a == null) {
                if (actions.Count != 0) {
                    return true;
                }

                return false;
            }

            unitManager.MoveNext();

            foreach (var p in actions) {
                if (!GameAction.Stackable(a,p)) {
                    nextAction = a;
                    return true;
                }
            }
            actions.AddLast(a);
            return GetActions();//potential infinite loop
        }
    }
}