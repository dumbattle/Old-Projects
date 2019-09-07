using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Circle : IEnumerable<Point2> {
    static Dictionary<int, Point2[]> circles = new Dictionary<int, Point2[]>();
    public List<Point2> points = new List<Point2>();

    //constructor
    public Circle(Point2 position, int radius) {
        if (radius <= 0) {
            Debug.Log(string.Format("ERROR: Trying to create circle with radius: {0}", radius));
            radius = 1;
        }

        //check if is in dictionary
        if (!circles.ContainsKey(radius)) {
            //add if not
            AddCircle(radius);
        }

        //create list
        for (int i = 0; i < circles[radius].Length; i++) {
            points.Add(circles[radius][i] + position);
        }
    }

    public static int Size (int radius) {
        //check if is in dictionary
        if (!circles.ContainsKey(radius)) {
            //add if not
            AddCircle(radius);
        }

        return circles[radius].Length;
    }
    static void AddCircle(int radius) {
        //check if already exist
        if (circles.ContainsKey(radius)) {
            return;
        }
        float _radius = radius + .5f;

        //create list
        List<Point2> v = new List<Point2> { Point2.zero };

        //curves
        int height = 0;
        for (int x = 1; x <= _radius; x++) {
            //get height
            height = (int)(Mathf.Sqrt(_radius * _radius - x * x));

            for (int y = 1; y <= height; y++) {
                //use symmetry
                v.Add(new Point2(x, y));
                v.Add(new Point2(x, -y));
                v.Add(new Point2(-x, y));
                v.Add(new Point2(-x, -y));
            }
        }

        //straight
        for (int i = 1; i <= _radius; i++) {
            v.Add(new Point2(i, 0));
            v.Add(new Point2(0, i));
            v.Add(new Point2(-i, 0));
            v.Add(new Point2(0, -i));
        }

        //add to dictionary
        circles.Add(radius, v.ToArray());

    }


    public IEnumerator<Point2> GetEnumerator() {
        for (int i = 0; i < points.Count; i++) {
            yield return points[i];
        }
    }
    IEnumerator IEnumerable.GetEnumerator() {
        for (int i = 0; i < points.Count; i++) {
            yield return points[i];
        }
    }

}
