using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DumbML;

public class FullyConnectedDrawer : LayerDrawerBase<FullyConnected> {
    static Dictionary<int, Cache> cache = new Dictionary<int, Cache>();


    public override void DrawInspector(Layer l) {
        FullyConnected fc = l as FullyConnected;

        int ID = (l).GetHashCode();

        if (!cache.ContainsKey(ID)) {
            cache.Add(ID, new Cache());
        }

        Cache c = cache[ID];
        GUIStyle gs = new GUIStyle(EditorStyles.foldout);
        gs.richText = true;
        c.isExpanded = EditorGUILayout.Foldout(c.isExpanded, "<color=green>Fully Connected</color> - " + fc.outputShape.TOSTRING(), gs);



        if (c.isExpanded) {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Size: " + fc.outputShape[0].ToString());
            EditorGUILayout.LabelField("Activation: " + fc.af.GetType().Name);

            ViewWeights(c, fc, ID);

            EditorGUI.indentLevel--;
        }
    }

    void ViewWeights(Cache c, FullyConnected fc, int ID) {
        c.weightsExpanded = EditorGUILayout.Foldout(c.weightsExpanded, "Weights");

        if (c.weightsExpanded) {
            EditorGUI.indentLevel++;
            int inOut = EditorGUILayout.IntPopup("Input/Ouput:", cache[ID].viewInput ? 0 : 1, new string[] { "Input", "Output" }, new int[] { 0, 1 });

            cache[ID].viewInput = inOut == 0;


            //select node to display weights
            string label = "Select Node: ";

            int numNodes = cache[ID].viewInput ? fc.inputShape[0] : fc.outputShape[0];

            string[] displayedOptions = new string[numNodes + 1];
            int[] optionValues = new int[numNodes + 1];

            displayedOptions[0] = "None Selected";
            optionValues[0] = -1;

            for (int i = 0; i < numNodes; i++) {
                displayedOptions[i + 1] = "Node " + i;
                optionValues[i + 1] = i;
            }

            cache[ID].Node = EditorGUILayout.IntPopup(label, cache[ID].Node, displayedOptions, optionValues);

            //display weights
            if (cache[ID].viewInput) {
                int node = cache[ID].Node = cache[ID].Node.Clamp(-1, fc.inputShape[0] - 1);
                if (node != -1) {
                    for (int i = 0; i < fc.outputShape[0]; i++) {
                        EditorGUILayout.LabelField($"({node}, {i}): {fc.weights[node, i].ToString()}");

                    }
                }
            }
            else {
                int node = cache[ID].Node = cache[ID].Node.Clamp(-1, fc.outputShape[0] - 1);
                if (node != -1) {
                    for (int i = 0; i < fc.inputShape[0]; i++) {
                        EditorGUILayout.LabelField($"({i}, {node}): {fc.weights[i, node].ToString()}");

                    }
                }
            }
            EditorGUI.indentLevel--;
        }
    }

    private class Cache {
        public int Node = -1;
        public bool viewInput = false;
        public bool isExpanded = false;
        public bool weightsExpanded = false;
    }
}
