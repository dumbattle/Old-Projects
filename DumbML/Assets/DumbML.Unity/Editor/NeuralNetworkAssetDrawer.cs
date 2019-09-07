using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DumbML;
using DumbML.Unity;


[CustomEditor(typeof(NeuralNetworkAsset))]
public class NeuralNetworkAssetDrawer : Editor {
    public static NeuralNetwork currentNetwork;
    public override void OnInspectorGUI() {
        EditorUtility.SetDirty(target);
        serializedObject.Update();

        NeuralNetworkAsset nna = ((NeuralNetworkAsset)target);
        if (nna.SerializedData == "null") {
            EditorGUILayout.LabelField("No Network Saved");
            return;
        }


        NeuralNetwork nn = nna.nn;
        currentNetwork = nn;
        SerializedProperty stringProp = serializedObject.FindProperty("SerializedData");
        SerializedProperty readOnly = serializedObject.FindProperty("ReadOnly");


        if (GUILayout.Button("Clear")) {
            if (EditorUtility.DisplayDialog("Delete Network", "Are you sure you want to delete this network?", "Delete Network", "Cancel")) {
                serializedObject.Update();
                nna.Clear();
                serializedObject.ApplyModifiedProperties();


            }
        }


        nna.ReadOnly = EditorGUILayout.Toggle("Read Only", nna.ReadOnly);
        EditorGUILayout.LabelField("Input Shape: " + nn.inputShape.TOSTRING());
        EditorGUILayout.LabelField("Output Shape: " + nn.outputShape.TOSTRING());
        EditorGUILayout.Space();

        DrawLayers(nn, stringProp);


        
        serializedObject.ApplyModifiedProperties();
    }

    
    void DrawLayers(NeuralNetwork nn, SerializedProperty stringProp) {
        stringProp.isExpanded = EditorGUILayout.Foldout(stringProp.isExpanded, "Layers");

        if (stringProp.isExpanded) {
            EditorGUI.indentLevel++;
            GUIStyle gs = new GUIStyle();
            EditorGUILayout.LabelField($"<color=green>Input</color> - " + nn.inputShape.TOSTRING(), gs);


            List<Layer> layers = nn.Layers;
            if (layers != null) {
                for (int i = 0; i < layers.Count; i++) {
                    bool success = NeuralNetworkLayerDrawer.DrawEditor(layers[i]);
                    if (!success) {
                        EditorGUILayout.LabelField($"<color=green>{layers[i].GetType().Name}</color> - " + layers[i].outputShape.TOSTRING(), gs);
                    }
                }
            }
            EditorGUI.indentLevel--;
    }
}
}
