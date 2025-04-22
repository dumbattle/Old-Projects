using UnityEngine;
using System.Linq;

namespace MysteryDungeon {
    public partial class GameAction {
        class MOVE : GameAction {
            const int NUM_FRAMES = 10;
            Vector2 start, end;
            HexCoord dir;

            float f = 0;

            public MOVE(Unit u, HexCoord dir) {
                type = Type.Move;
                stackable = true;

                unit = u;
                var dest = dir + u.pos;

                start = Game.main.HexToCartesion(u.pos);
                end = Game.main.HexToCartesion(dest);

                u.SetPosition(dest);
                u.game.sorter.Sort(u.obj, u.game.map[u.pos].height);
                this.dir = dir;

            }

            public override bool AnimUpdate() {
                if (f == 0) {
                    unit.MoveAnim();
                    unit.SetDirection(dir);
                }
                f++;
                unit.obj.transform.position = Vector2.Lerp(start, end, f / NUM_FRAMES);


                if (f >= NUM_FRAMES) {
                    unit.game.Invoke(() => unit.IdleAnim(), 1);
                    return true;
                }

                return false;
            }
        }
    }
}