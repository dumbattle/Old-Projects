using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using DumbML;

public class ShaderTest : MonoBehaviour {
    Stopwatch sw = new Stopwatch();
    public ComputeShader cs;
    // Start is called before the first frame update
    void Start() {
        Tensor t1 = new Tensor(() => 1, 1024 * 1024);
        Tensor t2 = t1.Copy();

        sw.Start();
        Control(t1, t2);
        sw.Stop();
        print(sw.ElapsedMilliseconds);

        sw.Reset();
        sw.Start();
        RunShader(t1, t2);
        sw.Stop();
        print(sw.ElapsedMilliseconds);
    }


    void Control(Tensor t1, Tensor t2) {
         var a =t1 * t2;
        //print(t1);
        //print(t2);
        //print(a);
    }

  void RunShader (Tensor t1, Tensor t2) {
        int kernalHandle = cs.FindKernel("Multiply");

        ComputeBuffer buffer1 = new ComputeBuffer(t1.Size, 4);
        buffer1.SetData(t1._value);
        ComputeBuffer buffer2 = new ComputeBuffer(t1.Size, 4);
        buffer2.SetData(t2._value);

        cs.SetBuffer(kernalHandle, "buffer1", buffer1);
        cs.SetBuffer(kernalHandle, "buffer2", buffer2);

        cs.Dispatch(kernalHandle, t1.Size / 32, 1, 1);
        buffer1.GetData(t1._value);

        buffer1.Dispose();
        buffer2.Dispose();
    }
}

