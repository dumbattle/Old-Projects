using UnityEngine;

[System.Serializable]
public class TileSet<T> {
    const int TOP = 1;
    const int TOP_RIGHT = 2;
    const int RIGHT = 4;
    const int BOTTOM_RIGHT = 8;
    const int BOTTOM = 16;
    const int BOTTOM_LEFT = 32;
    const int LEFT = 64;
    const int TOP_LEFT = 128;

    public T defaultSprite;

    [Header("Edge")]
    public T[] topEdge;
    public T[] rightEdge;
    public T[] bottomEdge;
    public T[] leftEdge;
    [Header("Convex Corner")]
    public T[] topRightConvex;
    public T[] bottomRightConvex;
    public T[] bottomLeftConvex;
    public T[] topLeftConvex;
    [Header("Concave Corner")]
    public T[] topRightConcave;
    public T[] bottomRightConcave;
    public T[] bottomLeftConcave;
    public T[] topLeftConcave;
    [Header("Thin walls")]
    public T[] capTop;
    public T[] capRight;
    public T[] capBottom;
    public T[] capLeft;
    public T[] straightH;
    public T[] straightV;
    [Header("Compound edge + corner")]
    public T[] top_bottomLeft;
    public T[] top_bottomRight;
    public T[] right_topLeft;
    public T[] right_bottomLeft;
    public T[] bottom_topLeft;
    public T[] bottom_topRight;
    public T[] left_topRight;
    public T[] left_bottomRight;
    [Header("Double Corner")]
    public T[] topDC;
    public T[] rightDC;
    public T[] bottomDC;
    public T[] leftDC;
    public T[] topLeftXC;
    public T[] topRightXC;
    [Header("Triple Corner")]
    public T[] topRightTC;
    public T[] bottomRightTC;
    public T[] bottomLeftTC;
    public T[] topLeftTC;


    public T GetSprite(int wallCode, string name) {
        T result = default(T);
        switch (wallCode) {
            //<<<<<<< EDGES >>>>>>>
            case TOP:
            case TOP + TOP_RIGHT:
            case TOP + TOP_LEFT:
            case TOP + TOP_RIGHT + TOP_LEFT:
                result = bottomEdge.Random(); break;

            case RIGHT:
            case RIGHT + BOTTOM_RIGHT:
            case RIGHT + TOP_RIGHT:
            case RIGHT + TOP_RIGHT + BOTTOM_RIGHT:
                result = leftEdge.Random(); break;

            case BOTTOM:
            case BOTTOM + BOTTOM_RIGHT:
            case BOTTOM + BOTTOM_LEFT:
            case BOTTOM + BOTTOM_RIGHT + BOTTOM_LEFT:
                result = topEdge.Random(); break;

            case LEFT:
            case LEFT + BOTTOM_LEFT:
            case LEFT + TOP_LEFT:
            case LEFT + BOTTOM_LEFT + TOP_LEFT:
                result = rightEdge.Random(); break;

            //<<<<<<< CORNERS >>>>>>>
            case TOP_RIGHT:
            case TOP + RIGHT:
                result = bottomLeftConvex.Random(); break;

            case TOP_RIGHT + TOP + RIGHT:
            case TOP_RIGHT + TOP + RIGHT + BOTTOM_RIGHT:
            case TOP_RIGHT + TOP + RIGHT + TOP_LEFT:
            case TOP_RIGHT + TOP + RIGHT + BOTTOM_RIGHT + TOP_LEFT:
                result = bottomLeftConcave.Random(); break;

            case BOTTOM_RIGHT:
            case RIGHT + BOTTOM:
                result = topLeftConvex.Random(); break;

            case BOTTOM_RIGHT + RIGHT + BOTTOM:
            case BOTTOM_RIGHT + RIGHT + BOTTOM + TOP_RIGHT:
            case BOTTOM_RIGHT + RIGHT + BOTTOM + BOTTOM_LEFT:
            case BOTTOM_RIGHT + RIGHT + BOTTOM + TOP_RIGHT + BOTTOM_LEFT:
                result = topLeftConcave.Random(); break;

            case BOTTOM_LEFT:
            case BOTTOM + LEFT:
                result = topRightConvex.Random(); break;

            case BOTTOM_LEFT + BOTTOM + LEFT:
            case BOTTOM_LEFT + BOTTOM + LEFT + BOTTOM_RIGHT:
            case BOTTOM_LEFT + BOTTOM + LEFT + TOP_LEFT:
            case BOTTOM_LEFT + BOTTOM + LEFT + BOTTOM_RIGHT + TOP_LEFT:
                result = topRightConcave.Random(); break;

            case TOP_LEFT:
            case LEFT + TOP:
                result = bottomRightConvex.Random(); break;

            case TOP_LEFT + LEFT + TOP:
            case TOP_LEFT + LEFT + TOP + TOP_RIGHT:
            case TOP_LEFT + LEFT + TOP + BOTTOM_LEFT:
            case TOP_LEFT + LEFT + TOP + TOP_RIGHT + BOTTOM_LEFT:
                result = bottomRightConcave.Random(); break;

            //<<<<<<< THIN WALLS >>>>>>>
            case TOP + TOP_RIGHT + RIGHT + TOP_LEFT + LEFT:
            case TOP + TOP_RIGHT + RIGHT + TOP_LEFT + LEFT + BOTTOM_RIGHT:
            case TOP + TOP_RIGHT + RIGHT + TOP_LEFT + LEFT + BOTTOM_LEFT:
            case TOP + TOP_RIGHT + RIGHT + TOP_LEFT + LEFT + BOTTOM_RIGHT + BOTTOM_LEFT:
                result = capTop.Random(); break;

            case RIGHT + TOP_RIGHT + TOP + BOTTOM_RIGHT + BOTTOM:
            case RIGHT + TOP_RIGHT + TOP + BOTTOM_RIGHT + BOTTOM + TOP_LEFT:
            case RIGHT + TOP_RIGHT + TOP + BOTTOM_RIGHT + BOTTOM + BOTTOM_LEFT:
            case RIGHT + TOP_RIGHT + TOP + BOTTOM_RIGHT + BOTTOM + TOP_LEFT + BOTTOM_LEFT:
                result = capRight.Random(); break;

            case BOTTOM + BOTTOM_RIGHT + RIGHT + BOTTOM_LEFT + LEFT:
            case BOTTOM + BOTTOM_RIGHT + RIGHT + BOTTOM_LEFT + LEFT + TOP_RIGHT:
            case BOTTOM + BOTTOM_RIGHT + RIGHT + BOTTOM_LEFT + LEFT + TOP_LEFT:
            case BOTTOM + BOTTOM_RIGHT + RIGHT + BOTTOM_LEFT + LEFT + TOP_RIGHT + TOP_LEFT:
                result = capBottom.Random(); break;

            case LEFT + BOTTOM_LEFT + BOTTOM + TOP_LEFT + TOP:
            case LEFT + BOTTOM_LEFT + BOTTOM + TOP_LEFT + TOP + BOTTOM_RIGHT:
            case LEFT + BOTTOM_LEFT + BOTTOM + TOP_LEFT + TOP + TOP_RIGHT:
            case LEFT + BOTTOM_LEFT + BOTTOM + TOP_LEFT + TOP + BOTTOM_RIGHT + TOP_RIGHT:
                result = capLeft.Random(); break;

            //STRAIGHTS
            case TOP + BOTTOM:
            case TOP + BOTTOM + TOP_RIGHT + TOP_LEFT + BOTTOM_RIGHT + BOTTOM_LEFT:
            case TOP + BOTTOM + TOP_LEFT + BOTTOM_LEFT:
            case TOP + BOTTOM + TOP_LEFT + BOTTOM_RIGHT:
            case TOP + BOTTOM + TOP_RIGHT + BOTTOM_RIGHT:
            case TOP + BOTTOM + TOP_RIGHT + BOTTOM_LEFT:
            case TOP + BOTTOM + TOP_LEFT + BOTTOM_RIGHT + BOTTOM_LEFT:
            case TOP + BOTTOM + TOP_RIGHT + BOTTOM_RIGHT + BOTTOM_LEFT:
            case TOP + BOTTOM + TOP_RIGHT + TOP_LEFT + BOTTOM_LEFT:
            case TOP + BOTTOM + TOP_RIGHT + TOP_LEFT + BOTTOM_RIGHT:
                result = straightH.Random(); break;

            case LEFT + RIGHT:
            case LEFT + RIGHT + BOTTOM_RIGHT + BOTTOM_LEFT:
            case LEFT + RIGHT + BOTTOM_RIGHT + TOP_LEFT:
            case LEFT + RIGHT + TOP_RIGHT + BOTTOM_LEFT:
            case LEFT + RIGHT + TOP_RIGHT + TOP_LEFT:
            case LEFT + RIGHT + TOP_RIGHT + TOP_LEFT + BOTTOM_RIGHT + BOTTOM_LEFT:
            case LEFT + RIGHT + TOP_LEFT + BOTTOM_RIGHT + BOTTOM_LEFT:
            case LEFT + RIGHT + TOP_RIGHT + BOTTOM_RIGHT + BOTTOM_LEFT:
            case LEFT + RIGHT + TOP_RIGHT + TOP_LEFT + BOTTOM_LEFT:
            case LEFT + RIGHT + TOP_RIGHT + TOP_LEFT + BOTTOM_RIGHT:
                result = straightV.Random(); break;

            //<<<<<<< COMPOUND >>>>>>> 1 EDGE 1 CORNER
            case TOP + BOTTOM_RIGHT:
            case TOP + BOTTOM_RIGHT + TOP_LEFT:
            case TOP + BOTTOM_RIGHT + TOP_RIGHT:
            case TOP + BOTTOM_RIGHT + TOP_LEFT + TOP_RIGHT:
                result = top_bottomRight.Random(); break;

            case RIGHT + BOTTOM_LEFT:
            case RIGHT + BOTTOM_LEFT + TOP_RIGHT:
            case RIGHT + BOTTOM_LEFT + BOTTOM_RIGHT:
            case RIGHT + BOTTOM_LEFT + TOP_RIGHT + BOTTOM_RIGHT:
                result = right_bottomLeft.Random(); break;

            case BOTTOM + TOP_LEFT:
            case BOTTOM + TOP_LEFT + BOTTOM_LEFT:
            case BOTTOM + TOP_LEFT + BOTTOM_RIGHT:
            case BOTTOM + TOP_LEFT + BOTTOM_LEFT + BOTTOM_RIGHT:
                result = bottom_topLeft.Random(); break;

            case LEFT + TOP_RIGHT:
            case LEFT + TOP_RIGHT + TOP_LEFT:
            case LEFT + TOP_RIGHT + BOTTOM_LEFT:
            case LEFT + TOP_RIGHT + TOP_LEFT + BOTTOM_LEFT:
                result = left_topRight.Random(); break;

            case TOP + BOTTOM_LEFT:
            case TOP + BOTTOM_LEFT + TOP_LEFT:
            case TOP + BOTTOM_LEFT + TOP_RIGHT:
            case TOP + BOTTOM_LEFT + TOP_LEFT + TOP_RIGHT:
                result = top_bottomLeft.Random(); break;

            case RIGHT + TOP_LEFT:
            case RIGHT + TOP_LEFT + TOP_RIGHT:
            case RIGHT + TOP_LEFT + BOTTOM_RIGHT:
            case RIGHT + TOP_LEFT + TOP_RIGHT + BOTTOM_RIGHT:
                result = right_topLeft.Random(); break;

            case BOTTOM + TOP_RIGHT:
            case BOTTOM + TOP_RIGHT + BOTTOM_LEFT:
            case BOTTOM + TOP_RIGHT + BOTTOM_RIGHT:
            case BOTTOM + TOP_RIGHT + BOTTOM_LEFT + BOTTOM_RIGHT:
                result = bottom_topRight.Random(); break;

            case LEFT + BOTTOM_RIGHT:
            case LEFT + BOTTOM_RIGHT + TOP_LEFT:
            case LEFT + BOTTOM_RIGHT + BOTTOM_LEFT:
            case LEFT + BOTTOM_RIGHT + TOP_LEFT + BOTTOM_LEFT:
                result = left_bottomRight.Random(); break;

            //<<<<<<< DOUBLE CORNER >>>>>>>
            case BOTTOM_LEFT + BOTTOM_RIGHT:
                result = topDC.Random(); break;
            case TOP_LEFT + BOTTOM_LEFT:
                result = rightDC.Random(); break;
            case TOP_LEFT + TOP_RIGHT:
                result = bottomDC.Random(); break;
            case TOP_RIGHT + BOTTOM_RIGHT:
                result = leftDC.Random(); break;

            case TOP_LEFT + BOTTOM_RIGHT:
                result = topLeftXC.Random(); break;
            case TOP_RIGHT + BOTTOM_LEFT:
                result = topRightXC.Random(); break;

            //<<<<<<< TRIPLE CORNER >>>>>>>
            case BOTTOM_RIGHT + BOTTOM_LEFT + TOP_LEFT:
                result = topRightTC.Random(); break;
            case TOP_RIGHT + BOTTOM_LEFT + TOP_LEFT:
                result = bottomRightTC.Random(); break;
            case TOP_RIGHT + BOTTOM_RIGHT + TOP_LEFT:
                result = bottomLeftTC.Random(); break;
            case TOP_RIGHT + BOTTOM_RIGHT + BOTTOM_LEFT:
                result = topLeftTC.Random(); break;
            default:
                //Debug.Log("Wall Code " + wallCode + " not Accounted for in sprites");
                result = defaultSprite; break;
        }
        if (result == null) {
            Debug.Log("Did not set sprite for wall code " + wallCode + ". SpriteSet: " + name);
            return defaultSprite;
        }
        return result;
    }
}
