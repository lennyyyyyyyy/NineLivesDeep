using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Fertilizer : Consumable {
    protected override void OnPointerClick(PointerEventData data) {
        if (!usable) return;
        Vector2Int playerCoord = Player.s.GetCoord();
        List<Vector2Int> adjacentTiles = new List<Vector2Int>();
        for (int dx=-1; dx <= 1; dx++) {
            for (int dy=-1; dy <= 1; dy++) {
                if (dx == 0 && dy == 0) continue;
                if (!Floor.s.TileExistsAt(playerCoord.x + dx, playerCoord.y + dy)) continue;
                Tile tile = Floor.s.GetTile(playerCoord.x + dx, playerCoord.y + dy).GetComponent<Tile>();
                if (tile is MossyTile || tile is ActionTile) continue;
                adjacentTiles.Add(new Vector2Int(playerCoord.x + dx, playerCoord.y + dy));
            }
        }
        HelperManager.s.Shuffle(ref adjacentTiles);
        if (adjacentTiles.Count == 0) return;
        Floor.s.ReplaceTile(PrefabManager.s.tileMossyPrefab, adjacentTiles[0].x, adjacentTiles[0].y);
        if (adjacentTiles.Count == 1) return;
        if (Player.s.GetTile().GetComponent<Tile>() is MossyTile) {
            Floor.s.ReplaceTile(PrefabManager.s.tileMossyPrefab, adjacentTiles[1].x, adjacentTiles[1].y);
        }
        UpdateCount(count - 1);
    }
}
