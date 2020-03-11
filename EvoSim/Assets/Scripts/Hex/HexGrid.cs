using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

public struct HexGrid : IEnumerable<HexCoord> {
    public int radius;
    public int area;
    public int circumference;
    public HexCoord center;


    public HexGrid(int radius, HexCoord center) : this(radius) {
        this.center = center;
    }
    public HexGrid(int radius) {
        this.radius = radius;
        circumference = 6 * radius;
        area = 1 + 3 * radius * (radius + 1);
        center = new HexCoord(0, 0);
    }



    public void ParallelForeach(Action<HexCoord> function) {
        Thread[] threads = new Thread[area];
        int ind = 0;

        foreach (var h in this) {
            var t = new Thread(() => function(h));
            threads[ind] = t;

            t.Start();
            ind++;
        }
        foreach (var t in threads) {
            t.Join();
        }
    }
    public IEnumerator<HexCoord> GetEnumerator() {
        yield return new HexCoord(0, 0) + center;

        for (int r = 1; r <= radius; r++) {
            var hex = new HexCoord(0, 0) + HexCoord.Directions[4] * r;

            for (int d = 0; d < 6; d++) {
                for (int i = 0; i < r; i++) {
                    yield return hex + center;
                    hex += HexCoord.Directions[d];
                }
            }
        }
    }
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }


    public IEnumerable Ring() {
        var hex = new HexCoord(0, 0) + HexCoord.Directions[4] * radius;

        for (int d = 0; d < 6; d++) {
            for (int i = 0; i < radius; i++) {
                yield return hex + center;
                hex += HexCoord.Directions[d];
            }
        }
    }
    public HexCoord RandomCoord() {
        //choose ring
        //choose arm
        //choose hex
        int r = UnityEngine.Random.Range(0, (radius + 1) * radius * 3 + 1);
        if (r == 0) {
            return new HexCoord(0, 0) + center;
        }
        r--;

        for (int i = 1; i <= radius; i++) {
            r -= i * 6;
            if (r < 0) {
                r = i;
                break;
            }
        }


        int arm = UnityEngine.Random.Range(0, 6);

        HexCoord result = HexCoord.Directions[arm] * r;
        int step = UnityEngine.Random.Range(0, r);
        return result + HexCoord.Directions[(arm + 2) % 6] * step + center;
    }

    public bool IsInRange(HexCoord hex) {
        return (hex - center).ManhattanDist() <= radius;
    }
}
