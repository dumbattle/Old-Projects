using System.Collections.Generic;
using UnityEditor;
using DumbML;

public abstract class LayerDrawerBase<TLayerType> where TLayerType : Layer {
    protected Dictionary<int, bool> foldouts = new Dictionary<int, bool>();
    public abstract void DrawInspector(Layer l);

}
