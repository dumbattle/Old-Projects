using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlantGrid : HexMap<Plant> {
    public Map map { get; private set; }

    LinkedList<Plant> _plants = new LinkedList<Plant>();
    LinkedList<Plant> _markedForRemoval = new LinkedList<Plant>();
    LinkedList<(HexCoord, Plant)> _pendingAdd = new LinkedList<(HexCoord, Plant)>();

    bool _clearToModify = true;

    public PlantGrid(Map m) : base(m.Radius) {
        map = m;
    }

    public void SpawnPlant(HexCoord h, Plant p) {
        _pendingAdd.AddLast((h, p));
    }

    public void RemovePlant(HexCoord h) {
        Plant p = this[h];

        if (p == null) {
            return;
        }

        _markedForRemoval.AddLast(p);
    }
    public void RemovePlant(Plant p) {
        RemovePlant(p.pos);
    }

    public void Update() {
        RemoveMarkedPlants();
        AddPlants();
        _clearToModify = false;

        LinkedList<Plant> shuffled = new LinkedList<Plant>(_plants.OrderBy(x => Random.value));
        foreach (Plant p in shuffled) {
            p.Update();
        }
        _clearToModify = true;
        RemoveMarkedPlants();
        AddPlants();
    }

    void RemoveMarkedPlants() {
        if (!_clearToModify) {
            return;
        }

        foreach (Plant marked in _markedForRemoval) {
            _plants.Remove(marked);
            this[marked.pos] = null;
        }
        _markedForRemoval.Clear();
    }
    void AddPlants() {
        foreach (var hp in _pendingAdd) {
            HexCoord h = hp.Item1;
            Plant p = hp.Item2;

            if (!IsInRange(h)) {
                p.Die();
                continue;
            }
            if (this[h] != null) {
                p.Die();
                continue;
            }

            this[h] = p;
            p.Spawn(h, this);
            _plants.AddLast(p);
        }
        _pendingAdd.Clear();
    }
}