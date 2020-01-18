using UnityEngine;

public class Graph {
    public static float YMin = -1, YMax = +1;

    public const int MAX_HISTORY = 1024 * 4;
    public const int MAX_CHANNELS = 64;

    public static Channel[] channel = new Channel[MAX_CHANNELS];

    static Graph() {
        channel[0] = new Channel(Color.green);
        channel[1] = new Channel(Color.blue);
        channel[2] = new Channel(Color.red);
        channel[3] = new Channel(Color.yellow);
        channel[4] = new Channel(Color.magenta);
        channel[5] = new Channel(Color.cyan);
        channel[6] = new Channel(Color.grey);
        channel[7] = new Channel(Color.black);
        channel[8] = new Channel(Color.white);
    }

    static Channel GetChannel() {
        foreach (var c in channel) {
            if (!c.isActive && c.Length == 0) {
                c.SetActive();
                return c;
            }
        }
        return null;
    }
    public static Channel GetChannel(string name) {
        foreach (var c in channel) {
            if (!c.isActive && c.Length == 0) {
                c.SetActive(name);
                return c;
            }
        }
        return null;
    }
    public static void SetYRange(float min, float max) {
        YMin = min;
        YMax = max;
    }
}