using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour {
    UnitModule[] modules;

    public MovementModule Movement { get; private set; }

    public AnimationModule Animation { get; private set; }

    void Awake() {
        modules = GetComponents<UnitModule>();
        foreach (var m in modules) {
            m.controller = this;
            if (m as MovementModule is var a){
                Movement = a;
            }
            if (m as AnimationModule is var b){
                Animation = b;
            }
        }
    }

    public T GetModule<T>() where T : UnitModule {
        foreach (var m in modules) {
            var result = m as T;
            if (result != null) {
                return result;
            }
        }
        return null;
    }
    public T[] GetModules<T>() where T : UnitModule {
        var result = (from x in modules where x as T != null select x as T).ToArray();
        return result;
    }
}
