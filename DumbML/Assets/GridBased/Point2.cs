using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public struct Point2 {
    public int x, y;
    public Point2(int X, int Y) {
        x = X;
        y = Y;
    }

    public static Point2 zero = new Point2(0, 0);
    public static Point2 up = new Point2(0, 1);
    public static Point2 down = new Point2(0, -1);
    public static Point2 left = new Point2(-1, 0);
    public static Point2 right = new Point2(1, 0);
    public static Point2 one = new Point2(1, 1);

    public static Point2[] UDLR = { up, down, left, right };
    public static Point2[] Diagonals = { up + right, down + right, left + down, left + up };
    /// <summary>
    /// 8 - Directions
    /// </summary>
    public static Point2[] Directions = { up, up + right, right, right + down, down, down + left, left, left + up };


    public override string ToString() {
        return string.Format("({0}, {1})", x, y);
    }
    public int SqrDistanceTo(Point2 p) {
        return (x - p.x) * (x - p.x) + (y - p.y) * (y - p.y);
    }
    public Point2 Clamp() {
        return new Point2(
            x >= 1 ? 
            1 :
            (x <= -1 ? -1 : 0),
            
            y >= 1 ?
            1 :
            (y <= -1 ? -1 : 0));
    }

    public int SqrMagnitude() {
        return x * x + y * y;
    }
    public float Magnitude() {
        return Mathf.Sqrt(x * x + y * y);
    }

    public static int SqrDistance(Point2 a, Point2 b) {
        return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
    }
    public Point2[] GetLine(Point2 destination, bool connected4Way = false) {
        return GetLine(this, destination, connected4Way);
    }
    public static Point2[] GetLine(Point2 start, Point2 end, bool connected4Way = false) {
        List<Point2> results = new List<Point2>();

        Point2 direction = end - start;

        //special case vertical
        if (direction.x == 0) {
            if (direction.y > 0) {
                for (int y = 0; y <= direction.y; ++y) {
                    results.Add(new Point2(0, y) + start);
                }
            }
            else {
                for (int y = 0; y >= direction.y; --y) {
                    results.Add(new Point2(0, y) + start);
                }
            }

            return results.ToArray();
        }

        bool horizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
        float delta = 0;
        float slope;

        //make direction positive to minimize work
        if (horizontal) {
            int y = 0;
            slope = Mathf.Abs((float)direction.y / direction.x);

            for (int x = 0; x < Mathf.Abs(direction.x) + 1; ++x) {
                results.Add(new Point2(direction.x > 0 ? x : -x, y) + start);
                delta += slope;

                if (delta > .5f) {
                    delta -= 1;
                    y = y + (direction.y > 0 ? 1 : -1);

                    if (connected4Way && x < Mathf.Abs(direction.x)) {
                        results.Add(new Point2(direction.x > 0 ? x : -x, y) + start);
                    }
                }
            }
        }
        else {
            int x = 0;
            slope = Mathf.Abs((float)direction.x / direction.y);

            for (int y = 0; y < Mathf.Abs(direction.y) + 1; ++y) {
                results.Add(new Point2(x, direction.y > 0 ? y : -y) + start);

                delta += slope;

                if (delta > .5f) {
                    delta -= 1;
                    x = x + (direction.x > 0 ? 1 : -1);
                    if (connected4Way && y < Mathf.Abs(direction.y)) {
                        results.Add(new Point2(x, direction.y > 0 ? y : -y) + start);
                    }
                }
            }
        }


        return results.ToArray();
    }
    public static Point2 Random(Point2 a, Point2 b) {
        int x = a.x < b.x ? UnityEngine.Random.Range(a.x, b.x) : UnityEngine.Random.Range(b.x, a.x);
        int y = a.y < b.y ? UnityEngine.Random.Range(a.y, b.y) : UnityEngine.Random.Range(b.y, a.y);
        return new Point2(x, y);
    }
    //Return between (0, 0) inclusive a exclusive;
    public static Point2 Random(Point2 a) {
        return Random(a, zero);
    }

    public Circle GetCircle(int radius) {
        return new Circle(this, radius);
    }
    public Rectangle GetRectangle(Point2 size, bool centered = false) {
        return new Rectangle((centered ? this - size / 2 : this), size);
    }

    public static implicit operator Point2 ((int x, int y) other) {
        return new Point2(other.x, other.y);
    }
    #region Operators
    public static bool operator ==(Point2 v1, Point2 v2) {
        return v1.x == v2.x && v1.y == v2.y;
    }
    public static bool operator !=(Point2 v1, Point2 v2) {
        return v1.x != v2.x || v1.y != v2.y;
    }

    public static Point2 operator +(Point2 a, Point2 b) {
        return new Point2(a.x + b.x, a.y + b.y);
    }
    public static Point2 operator +(Point2 a, int i) {
        return new Point2(a.x + i, a.y + i);
    }

    public static Point2 operator -(Point2 a, Point2 b) {
        return new Point2(a.x - b.x, a.y - b.y);
    }

    public static Point2 operator -(Point2 a) {
        return new Point2(-a.x, -a.y);
    }

    public static Point2 operator *(Point2 a, int i) {
        return new Point2(a.x * i, a.y * i);
    }
    public static Point2 operator *(Point2 a, Point2 b) {
        return new Point2(a.x * b.x, a.y * b.y);
    }
    public static Point2 operator /(Point2 a, float i) {
        return new Point2((int)(a.x / i), (int)(a.y / i));
    }

    public static implicit operator Vector2(Point2 v) {
        return new Vector2(v.x, v.y);
    }
    public static implicit operator Vector3(Point2 v) {
        return new Vector3(v.x, v.y);
    }

    public override bool Equals(object obj) {
        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return (x << 16) + y;
    }
    #endregion
}

[System.Serializable] 
public struct IntRange {
    public int min, max;
}