using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DumbML;
public class Conv2DDrawer : LayerDrawerBase<Convolution2D> {
    static Dictionary<int, Cache> cache = new Dictionary<int, Cache>();

    public override void DrawInspector(Layer l) {
        Convolution2D cd = l as Convolution2D;
        int numFilters = cd.outputShape[2];
        int ID = (l).GetHashCode();

        if (!cache.ContainsKey(ID)) {
            cache.Add(ID, new Cache() { filterImg = new Texture2D[numFilters] });
        }

        Cache c = cache[ID];

        GUIStyle gs = new GUIStyle(EditorStyles.foldout);
        gs.richText = true;

        c.isExpanded = EditorGUILayout.Foldout(c.isExpanded, "<color=green>Convolution2D</color> - " + cd.outputShape.TOSTRING(), gs);
        if (c.isExpanded) {
            EditorGUI.indentLevel++;


            if (GUILayout.Button("Visualize")) {
                Visualize();
            }

            SelectFilter();

            if (c.currentView != -1 && c.filterImg[c.currentView] != null) {
                GUI.DrawTexture(EditorGUILayout.GetControlRect(false, 100), c.filterImg[c.currentView], ScaleMode.ScaleToFit);
            }

            EditorGUI.indentLevel--;
            void Visualize() {
                for (int i = 0; i < numFilters; i++) {
                   if ( EditorUtility.DisplayCancelableProgressBar("Visualizing Layer", $"{i + 1}/ {numFilters}", (i + 1f) / numFilters)) {

                        EditorUtility.ClearProgressBar();
                        return;
                    }

                    int width = NeuralNetworkAssetDrawer.currentNetwork.inputShape[0];
                    int height = NeuralNetworkAssetDrawer.currentNetwork.inputShape[1];

                    var t = new Texture2D(width, height);
                    Tensor img = cd.Visualize(NeuralNetworkAssetDrawer.currentNetwork, i);

                    if (img.Shape[2] == 3) {
                        for (int x = 0; x < width; x++) {
                            for (int y = 0; y < height; y++) {
                                t.SetPixel(x, y, new Color(img[x, y, 0], img[x, y, 1], img[x, y, 2]));
                            }
                        }
                    }
                    else if (img.Shape[2] == 4) {

                        for (int x = 0; x < width; x++) {
                            for (int y = 0; y < height; y++) {
                                t.SetPixel(x, y, new Color(img[x, y, 0], img[x, y, 1], img[x, y, 2], img[x, y, 3]));
                            }
                        }
                    }
                    t.filterMode = FilterMode.Point;
                    t.Apply();
                    c.filterImg[i] = t;
                }
                EditorUtility.ClearProgressBar();
            }
            void SelectFilter () {
                string label = "Select Filter: ";

                string[] displayedOptions = new string[numFilters + 1];
                int[] optionValues = new int[numFilters + 1];

                displayedOptions[0] = "None Selected";
                optionValues[0] = -1;

                for (int i = 0; i < numFilters; i++) {
                    displayedOptions[i + 1] = "Node " + i;
                    optionValues[i + 1] = i;
                }
                c.currentView = EditorGUILayout.IntPopup(label, cache[ID].currentView, displayedOptions, optionValues);

            }
        }
    }


    private class Cache {
        public bool isExpanded = false;
        public int currentView = 0;
        public Texture2D[] filterImg;
    }

}