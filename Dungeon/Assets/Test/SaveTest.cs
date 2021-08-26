using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;


namespace Test {
    public class SaveTest : MonoBehaviour {
        private void Start() {

            TestA a = new TestA() { a = 2 };
            TestB b = new TestB() { b = "hi" };
            TestC c = new TestC() { c = new Vector2(-21, 31) };

            string sa = a.GetType().AssemblyQualifiedName + "|" + a.Save();
            string sb = b.GetType().AssemblyQualifiedName + "|" + b.Save();
            string sc = c.GetType().AssemblyQualifiedName + "|" + c.Save();



            var a2 = SaveUtility.Load(sa);
            var b2 = SaveUtility.Load(sb);
            var c2 = SaveUtility.Load(sc);

            Debug.Log(a2.GetType());
            Debug.Log(b2.GetType());
            Debug.Log(c2.GetType());
        }
    }


    public static class SaveUtility {
        static Dictionary<Type, Func<string, TestBase>> loadDict;

        static SaveUtility() {
            loadDict = new Dictionary<Type, Func<string, TestBase>>();

            // get all types with attribute
            var types =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttribute(typeof(SaveAttribute), false)
                where attributes != null
                select t;


            foreach (var t in types) {
                foreach (var m in t.GetMethods()) {
                    // check name
                    if (m.Name != ((SaveAttribute)t.GetCustomAttribute(typeof(SaveAttribute), false)).loadMethodName) {
                        continue;
                    }

                    // check signature
                    if (!typeof(TestBase).IsAssignableFrom(m.ReturnType)) {
                        continue;
                    }
                    if (m.GetParameters().Length != 1) {
                        continue;
                    }

                    if (m.GetParameters()[0].ParameterType != typeof(string)) {
                        continue;
                    }

                    var d = (Func<string, TestBase>)Delegate.CreateDelegate(typeof(Func<string, TestBase>), m);
                    loadDict.Add(t, d);
                }
            }
        }

        public static TestBase Load(string data) {
            var td = data.Split(new[] { '|' }, 2);
            var d = loadDict[Type.GetType(td[0])];
            return d(td[1]);
        }
    }
    public class SaveAttribute : Attribute {
        public string loadMethodName;

        public SaveAttribute(string loadMethodName) {
            this.loadMethodName = loadMethodName;
        }
    }
    
    public abstract class TestBase {
        public abstract string Save();
    }

    [Save("Load")]
    public class TestA : TestBase {
        public int a;

        public override string Save() {
            return a.ToString();
        }

        public static TestBase Load(string data) {
            TestA result = new TestA();

            int a = int.Parse(data);
            result.a = a;
            return result;
        }
    }
    [Save("Load")]
    public class TestB : TestBase {
        public string b;
        public override string Save() {
            return b;
        }
        public static TestBase Load(string data) {
            TestB result = new TestB();

            result.b = data;
            return result;
        }

    }
    [Save("Load")]
    public class TestC : TestB {
        public Vector2 c;
        public override string Save() {
            return $"{c.x}_{c.y}";
        }
        public static new TestBase Load(string data) {
            TestC result = new TestC();
            var xy = data.Split('_');
            int x = int.Parse(xy[0]);
            int y = int.Parse(xy[0]);
            result.c = new Vector2(x,y);
            return result;
        }
    }
}