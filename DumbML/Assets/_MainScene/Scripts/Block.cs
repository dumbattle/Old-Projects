using UnityEngine;

public class Block : MonoBehaviour {
    public bool ground;
    public bool water;
    [Range(0,1)]
    public float fertility;

    [Space]
    public Point2 index;
    public Vector3 position { get { return gameObject.transform.position; } }
    

    public bool obstructed;

    public Item item;

    public void SetItem(Item item) {
        this.item = item;
    }

    public Item RemoveItem () {
        Item i = item;
        item = null;
        return i;
    }
}

