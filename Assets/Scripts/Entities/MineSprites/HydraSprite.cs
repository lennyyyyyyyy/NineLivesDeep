using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HydraSprite : MineSprite {
    public override void Trigger() {
        if (PassiveActive()) {
            Debug.Log("Hydra triggered");
            List<Vector2Int> potentialSpawnCoords = new List<Vector2Int>();
            Vector2Int coord = GetCoord();
            for (int x = coord.x - 1; x <= coord.x + 1; x++) {
                for (int y = coord.y - 1; y <= coord.y + 1; y++) {
                    if (Floor.s.TileExistsAt(x, y) && Floor.s.GetTile(x, y).GetComponent<Tile>().uniqueMine == null) {
                        potentialSpawnCoords.Add(new Vector2Int(x, y));
                    }
                }
            }
            HelperManager.s.Shuffle(ref potentialSpawnCoords, count: 2);
            for (int i=0; i<Mathf.Min(2, potentialSpawnCoords.Count); i++) {
                Floor.s.PlaceMine(typeof(MineSprite), potentialSpawnCoords[i].x, potentialSpawnCoords[i].y);
            }
        }
        base.Trigger();
    }
}
