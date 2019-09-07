using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DumbML;

public class NeuralNetworkDrawer : LayerDrawerBase<NeuralNetwork> {
    static Dictionary<int, Cache> cache = new Dictionary<int, Cache>();

    public override void DrawInspector(Layer l) {
        NeuralNetwork nn = l as NeuralNetwork;

        int ID = (l).GetHashCode();
        if (!cache.ContainsKey(ID)) {
            cache.Add(ID, new Cache());
        }

        Cache c = cache[ID];


        GUIStyle gs = new GUIStyle(EditorStyles.foldout);
        gs.richText = true;

        string label =
            c.isExpanded ?
            "<color=green>Neural Network</color>" :
            "<color=green>Neural Network</color> - " + nn.outputShape.TOSTRING();
        c.isExpanded = EditorGUILayout.Foldout(c.isExpanded, label, gs);

        if (c.isExpanded) {
            EditorGUI.indentLevel++;
            List<Layer> layers = nn.Layers;

            if (layers != null) {
                for (int i = 0; i < layers.Count; i++) {

                    bool success = NeuralNetworkLayerDrawer.DrawEditor(layers[i]);
                    if (!success) {
                        gs = new GUIStyle();
                        EditorGUILayout.LabelField($"<color=green>{layers[i].GetType().Name}</color> - " + layers[i].outputShape.TOSTRING(), gs);
                    }
                }
            }
            EditorGUI.indentLevel--;
        }
    }

    private class Cache {
        public bool isExpanded = false;
    }
}
