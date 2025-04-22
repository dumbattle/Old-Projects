using UnityEditor;
using System;
using System.Linq;
using DumbML;

public static class NeuralNetworkLayerDrawer {
    static (Action<Layer> DrawEditor, Type type)[] storage;
    static Type[] a;


    static NeuralNetworkLayerDrawer() {
        a = System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
             .Where(t => t.BaseType != null && t.BaseType.IsGenericType &&
                 t.BaseType.GetGenericTypeDefinition() == typeof(LayerDrawerBase<>)).ToArray();



        storage = new (Action<Layer>, Type)[a.Length];
        for (int i = 0; i < a.Length; i++) {
            Action<Layer> d = (Action<Layer>)Delegate.CreateDelegate(typeof(Action<Layer>), Activator.CreateInstance(a[i]), a[i].GetMethod("DrawInspector"));
            storage[i] = (d, a[i].BaseType.GetGenericArguments()[0]);
        }

    }

    public static bool DrawEditor<T>(T t) where T : Layer {
        foreach (var s in storage) {
            if (t.GetType() == s.type) {
                s.DrawEditor(t);
                return true;
            }
        }
        return false;
    }

}
