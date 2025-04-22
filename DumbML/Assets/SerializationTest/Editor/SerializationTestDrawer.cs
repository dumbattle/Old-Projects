using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(SerializationTestObj))]
public class SerializationTestDrawer : Editor {
    public override void OnInspectorGUI() {
        serializedObject.Update();

        var v2 = serializedObject.FindProperty("v2");
        EditorGUILayout.PropertyField(v2, new GUIContent("vect"), true);


        var listProp = serializedObject.FindProperty("list");


        var list = ((SerializationTestObj)target).list;

        for (int i = 0; i < list.Length; i++) {
            if (!DrawerHolder.DrawEditor(list[i])) {
                Debug.Log("A");
                SerializedProperty sp = listProp.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(sp, new GUIContent("Item"), true);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }


}

public static class TestSerialization {
    public static void DrawTestEditor<T> (this T t) where T : BaseClass {
        if (t.GetType() == typeof(BaseClass)) {
            EditorGUILayout.LabelField("Base Class");
            EditorGUI.indentLevel++;

            t.BaseInt = EditorGUILayout.IntField("Base Int", t.BaseInt);

            EditorGUI.indentLevel--;
        }
        else if (t.GetType() == typeof(DerivedA)) {
            var a = t as DerivedA;
            EditorGUILayout.LabelField("Derived A");
            EditorGUI.indentLevel++;

            a.BaseInt = EditorGUILayout.IntField("Base Int", a.BaseInt);
            a.DerivedStringA = EditorGUILayout.TextField("Derived String A", a.DerivedStringA);
            EditorGUI.indentLevel--;
        }
        else if (t.GetType() == typeof(DerivedB)) {
            var b = t as DerivedB;
            EditorGUILayout.LabelField("Derived B");
            EditorGUI.indentLevel++;

            b.BaseInt = EditorGUILayout.IntField("Base Int", b.BaseInt);
            b.DerivedFloatB = EditorGUILayout.FloatField("Derived Float B", b.DerivedFloatB);
            EditorGUI.indentLevel--;

        }
        //DrawerHolder.PrintTypes();
    }


    public static void TestExtension(this BaseClass bc) {
        Debug.Log("Base");
    }

    public static void TestExtension(this DerivedB bc) {
        Debug.Log("Derived");
    }
    public static T CastToDerived<T>(this T t) where T : BaseClass{
        t.TestExtension();
        //Debug.Log(t.GetType().ToString());
        return t;
    }
}


public static class DrawerHolder {
    static (Action<BaseClass> DrawEditor, Type type)[] storage;
    static Type[] a;


    static DrawerHolder() {
        a = System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
             .Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
                 t.BaseType.GetGenericTypeDefinition() == typeof(TestDrawer<>)).ToArray();



        storage = new (Action<BaseClass>, Type)[a.Length];
        for (int i = 0; i < a.Length; i++) {
            Action<BaseClass> d =(Action<BaseClass>)Delegate.CreateDelegate(typeof(Action<BaseClass>), Activator.CreateInstance(a[i]), a[i].GetMethod("DrawInspector"));
            storage[i] = (d, a[i].BaseType.GetGenericArguments()[0]);
        }

    }


    public static bool DrawEditor<T> (T t) where T : BaseClass{
        foreach (var s in storage) {
            if (t.GetType() == s.type) {
                s.DrawEditor(t);
                return true;
            }
        }
        return false;
    }
    
}


public abstract class TestDrawer<T> where T : BaseClass{
    protected Dictionary<int, bool> foldouts = new Dictionary<int, bool>();
    public abstract void DrawInspector(BaseClass t);        
    
}
public class BCDrawer : TestDrawer<BaseClass> {
    public override void DrawInspector(BaseClass bc) {
        int hashCode = bc.GetHashCode();
        if (!foldouts.ContainsKey(hashCode)) {
            foldouts.Add(hashCode, false);
        }
        bool b = foldouts[hashCode];

        b = EditorGUILayout.Foldout(b, "Base Class");
        foldouts[hashCode] = b;



        if (b) {
            EditorGUI.indentLevel++;

            bc.BaseInt = EditorGUILayout.IntField("Base Int", bc.BaseInt);

            EditorGUI.indentLevel--;
        }
    }
}
public class DADrawer : TestDrawer<DerivedA> {
    public override void DrawInspector(BaseClass t) {
        var a = t as DerivedA;


        int hashCode = t.GetHashCode();
        if (!foldouts.ContainsKey(hashCode)) {
            foldouts.Add(hashCode, false);
        }
        bool b = foldouts[hashCode];

        b = EditorGUILayout.Foldout(b, "Derived A");
        foldouts[hashCode] = b;



        if (b) {
            EditorGUI.indentLevel++;

            a.BaseInt = EditorGUILayout.IntField("Base Int", a.BaseInt);
            a.DerivedStringA = EditorGUILayout.TextField("Derived String A", a.DerivedStringA);
            EditorGUI.indentLevel--;
        }
    }
}
public class DBDrawer : TestDrawer<DerivedB> {
    public override void DrawInspector(BaseClass t) {
        var b = t as DerivedB;


        int hashCode = t.GetHashCode();
        if (!foldouts.ContainsKey(hashCode)) {
            foldouts.Add(hashCode, false);
        }
        bool foldOut = foldouts[hashCode];

        foldOut = EditorGUILayout.Foldout(foldOut, "Derived B");
        foldouts[hashCode] = foldOut;




        if (foldOut) {
            EditorGUI.indentLevel++;

            b.BaseInt = EditorGUILayout.IntField("Base Int", b.BaseInt);
            b.DerivedFloatB = EditorGUILayout.FloatField("Derived Float B", b.DerivedFloatB);
            EditorGUI.indentLevel--;
        }
    }
}
public class DerivedCDrawer : TestDrawer<DerivedC> {

    public override void DrawInspector(BaseClass t) {
        var c = t as DerivedC;


        int hashCode = t.GetHashCode();
        if (!foldouts.ContainsKey(hashCode)) {
            foldouts.Add(hashCode, false);
        }
        bool b = foldouts[hashCode];

        b = EditorGUILayout.Foldout(b, "Derived C");
        foldouts[hashCode] = b;
        
        if (b) {
            EditorGUI.indentLevel++;

            c.BaseInt = EditorGUILayout.IntField("Base Int", c.BaseInt);
            c.StringC = EditorGUILayout.TextField("Derived String C", c.StringC);
            c.FloatC = EditorGUILayout.FloatField("Derived Float C", c.FloatC);
            EditorGUI.indentLevel--;
        }
    }
}
