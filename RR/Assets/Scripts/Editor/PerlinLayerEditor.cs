//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;


//[CustomPropertyDrawer(typeof(CompoundPerlin.Layer))]
//public class PerlinLayerEditor : PropertyDrawer {

//    static float a;
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        
//        bool expanded = property.isExpanded;
//        property.isExpanded = EditorGUILayout.Foldout(expanded, label);

//        if (expanded) {
//            EditorGUI.indentLevel++;
//            EditorGUILayout.RectField(position);

//            var perlin = property.FindPropertyRelative("perlin");
//            var height = property.FindPropertyRelative("height");
//            var minValue = property.FindPropertyRelative("minValue");
//            var minScale = property.FindPropertyRelative("minScale");
//            var useMask = property.FindPropertyRelative("useMask");
//            var maskIndex = property.FindPropertyRelative("maskIndex");

//            EditorGUILayout.PropertyField(perlin, true);
//            height.floatValue = EditorGUILayout.FloatField("Height", height.floatValue);

//            minValue.floatValue = EditorGUILayout.Slider("Min Value", minValue.floatValue, 0f, 1f);

//            minScale.floatValue = EditorGUILayout.FloatField("Min Scale", minScale.floatValue);

//            //mask
//            useMask.boolValue = EditorGUILayout.Toggle("Use Mask", useMask.boolValue);
//            if (useMask.boolValue) {
//                maskIndex.intValue = EditorGUILayout.IntField("Mask", maskIndex.intValue);
//            }
//            EditorGUI.indentLevel--;
//        }
//    }

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
//        return 0;
//    }
//}
