using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorGraph : EditorWindow {
    const int LEFT_MARGIN = 50;
    const int RIGHT_MARGIN = 20;
    const int TOP_MARGIN = 20;
    const int BOTTOM_MARGIN = 20;

    const float GRAPH_BUFFER = 20;
    bool longWindow = false;
    Vector2 scrollPos;
    int W, H;

    [MenuItem("Window/Graph")]
    static void ShowGraph() {
        var w =GetWindow<EditorGraph>();
    }


    Material lineMaterial;

    void OnEnable() {
        EditorApplication.update += MyDelegate;
    }

    void OnDisable() {
        EditorApplication.update -= MyDelegate;
    }

    void MyDelegate() {
        Repaint();
    }

    void CreateLineMaterial() {
        if (!lineMaterial) {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }


    void OnGUI() {
        if (Graph.channel[0] == null)
            return;

        int numActive = 0;

        W = (int)position.width;
        H = (int)position.height;

        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.LoadPixelMatrix();

        GL.Begin(GL.LINES);

        for (int chan = 0; chan < Graph.MAX_CHANNELS; chan++) {
            Channel C = Graph.channel[chan];

            if (C == null) {
                C = Graph.channel[chan] = new Channel();
            }

            if (C.Length > 0) {
                numActive++;
            }

            if (!C.isActive)
                continue;

            PlotChannel(C);
        }
        DrawAxes();

        GL.End();
        GL.PopMatrix();

        Rect pos = new Rect(
            LEFT_MARGIN,
            position.height / 2 + GRAPH_BUFFER,
            position.width - LEFT_MARGIN - RIGHT_MARGIN,
            position.height / 2 - BOTTOM_MARGIN - GRAPH_BUFFER);
        Rect window = new Rect(
            LEFT_MARGIN,
            position.height / 2 + GRAPH_BUFFER,
            position.width - LEFT_MARGIN - RIGHT_MARGIN + (longWindow?100:0),
            EditorGUIUtility.singleLineHeight * (numActive + 2));


        scrollPos = GUI.BeginScrollView(pos, scrollPos, window);
        longWindow = false;
        DisplayChannelInfo(pos);

        GUI.EndScrollView();

    }

    void PlotChannel(Channel C) {
        GL.Color(C.color);
        int yPix = GetYPix(0);
        int xPix = LEFT_MARGIN;

        for (int h = 1; h < C.Length; h++) {
            int nextY = GetYPix(h);

            int nextX = (int)((float)(h) / (C.Length - 1) * (W - RIGHT_MARGIN - LEFT_MARGIN)) + LEFT_MARGIN;


            Plot(new Vector2Int(xPix, yPix), new Vector2Int(nextX, nextY));
            xPix = nextX;
            yPix = nextY;
        }

        int GetYPix(int x) {
            float y = C.data[x];
            if (C.overrideYRange) {
                y = 1 - Mathf.InverseLerp(C.yMin, C.yMax, y);
            }
            else {
                y = 1 - Mathf.InverseLerp(Graph.YMin, Graph.YMax, y);
            }
            return (int)(y * (H / 2 - TOP_MARGIN) + TOP_MARGIN);

        }
    }


    void DisplayChannelInfo(Rect pos) {
        const int TOGGLE_WIDTH = 20;
        const int NAME_WIDTH = 75;
        const int COLOR_WIDTH = 60;

        const int COLOR_POS = 20 + TOGGLE_WIDTH + NAME_WIDTH;
        const int RANGE_POS = COLOR_POS + COLOR_WIDTH + 110;
        const int YMIN_POS = RANGE_POS + TOGGLE_WIDTH + 100;

        Rect r = new Rect(pos.x, pos.y,pos.width, 20);

        foreach (var c in Graph.channel) {
            if (c.Length > 0) {
                r = MoveDown(r, 1);

                c.isActive = EditorGUI.Toggle(new Rect(r.x, r.y, TOGGLE_WIDTH, r.height), c.isActive);
                c.name = EditorGUI.TextField(new Rect(r.x + TOGGLE_WIDTH, r.y, NAME_WIDTH, r.height), c.name);
                c.color = EditorGUI.ColorField(new Rect(r.x + COLOR_POS, r.y, COLOR_WIDTH, r.height), new GUIContent("Color"), c.color, true, false, false);

                EditorGUI.LabelField(new Rect(r.x + RANGE_POS, r.y, 100, r.height), "Custom Y-range");
                c.overrideYRange = EditorGUI.Toggle(new Rect(r.x + RANGE_POS + 100, r.y, TOGGLE_WIDTH, r.height), c.overrideYRange);

                if (c.overrideYRange) {
                    c.CalculateYRange();
                    EditorGUI.LabelField(new Rect(r.x + YMIN_POS, r.y, 25, r.height), "Min");
                    c.yMin = EditorGUI.FloatField(new Rect(r.x + YMIN_POS + 25, r.y, 50, r.height), c.yMin);

                    EditorGUI.LabelField(new Rect(r.x + YMIN_POS + 75, r.y, 25, r.height), "Max");
                    c.yMax = EditorGUI.FloatField(new Rect(r.x + YMIN_POS + 100, r.y, 50, r.height), c.yMax);

                    longWindow = true;
                }
            }
        }

    }
    Rect MoveDown(Rect src, int numLines) {
        Rect result = new Rect(
            src.x,
            src.y + src.height,
            src.width,
            numLines * EditorGUIUtility.singleLineHeight
        );
        return result;
    }

    void Plot(Vector2Int p1, Vector2Int p2) {
        GL.Vertex3(p1.x, p1.y, 0);
        GL.Vertex3(p2.x, p2.y, 0);
    }
    void DrawAxes() {
        Vector2Int origin = new Vector2Int(LEFT_MARGIN, (int)position.height / 2);
        Vector2Int top = new Vector2Int(LEFT_MARGIN, TOP_MARGIN);
        Vector2Int right = new Vector2Int((int)position.width - RIGHT_MARGIN, (int)position.height / 2);
        GL.Color(Color.black);
        GL.Vertex3(origin.x, origin.y, 0);
        GL.Vertex3(top.x, top.y, 0);
        GL.Vertex3(origin.x, origin.y, 0);
        GL.Vertex3(right.x, right.y, 0);

    }

    // plot an X
    void PlotPoint(float x, float y) {
        // first line of X
        GL.Vertex3(x - 1, y - 1, 0);
        GL.Vertex3(x + 1, y + 1, 0);

        // second
        GL.Vertex3(x - 1, y + 1, 0);
        GL.Vertex3(x + 1, y - 1, 0);
    }
}
