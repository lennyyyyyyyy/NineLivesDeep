using UnityEngine;
using System.Collections.Generic;

public class OneUp : Passive {
    protected virtual void OnFloorChangeAfterEntities() {
        if (!usable) return;
        List<GameObject> tiles = new List<GameObject>(Floor.s.tiles.Values);
        for (int i = 0; i < 2; i++) {
            tiles[Random.Range(0, tiles.Count)].GetComponent<Tile>().oneUpCount++;
        }
    }
    protected override void OnEnable() {
        base.OnEnable();
        EventManager.s.OnFloorChangeAfterEntities += OnFloorChangeAfterEntities;
    }
    protected override void OnDisable() {
        base.OnDisable();
        EventManager.s.OnFloorChangeAfterEntities -= OnFloorChangeAfterEntities;
    }
}
