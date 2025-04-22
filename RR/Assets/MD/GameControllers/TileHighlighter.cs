using UnityEngine;
using System.Collections.Generic;
using System;

namespace MysteryDungeon {
    public class TileHighlighter {
        static ObjectPool<Highlight> pool;
        static TileHighlighter() {
            Func<Highlight> f = () => {
                return new Highlight(MDObjHolder.GetHighlight());
            };
            pool = new ObjectPool<Highlight>(1, f);
        }

        Game game;
        LinkedList<Highlight> Active = new LinkedList<Highlight>();


        public TileHighlighter(Game g) {
            game = g;
        }



        public void HighlightTile(HexCoord h, Color c) {
            var hl = pool.Get();

            hl.obj.SetActive(true);
            hl.SetColor(c);

            hl.obj.transform.position = game.HexToCartesion(h);

            game.sorter.Sort(hl.obj,game.map[h].height);


            Active.AddLast(hl);
        }
        
        public void Clear() {
            foreach (var hl in Active) {
                hl.obj.SetActive(false);
                pool.Return(hl);
            }
        }


        class Highlight : IPoolable {
            public bool Active { get; set; }
            public GameObject obj;
            public SpriteRenderer sr;

            public Highlight(GameObject obj) {
                this.obj = obj;
                sr = obj.GetComponent<SpriteRenderer>();
            }

            public void SetColor(Color c) {
                sr.color = c;
            }
        }
    }
}