using UnityEngine;

public class Base : Placeable {
    protected override void OnNewMinefield() {
        base.OnNewMinefield();
        int mineCount = 0;
        foreach (GameObject tile in Floor.s.tiles.Values) {
            mineCount += tile.GetComponent<Tile>().uniqueMine != null ? 1 : 0;
        }
        UpdateCount(Mathf.Max(mineCount, count));
    }
}
