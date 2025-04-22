using System.Collections.Generic;
using System.Collections;

public class Rectangle : IEnumerable<Point2> {
    Point2[,] points;

    public Rectangle(Point2 position, Point2 size) {
        points = new Point2[size.x, size.y];

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                points[x, y] = new Point2(position.x + x, position.y + y);
            }
        }
    }


    public IEnumerator<Point2> GetEnumerator() {
        for (int i = 0; i < points.GetLength(0); i++) {
            for (int j = 0; j < points.GetLength(1); j++) {
                yield return points[i, j];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        for (int i = 0; i < points.GetLength(0); i++) {
            for (int j = 0; j < points.GetLength(1); j++) {
                yield return points[i, j];
            }
        }
    }
    
}