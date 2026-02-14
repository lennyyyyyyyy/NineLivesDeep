using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TelemineSprite : MineSprite {
	public static readonly float distSpread = 10f;
	public override void Trigger() {
        if (PassiveActive()) {
            GameObject destinationTile;
            if (Player.s.tilesUnvisited.Count != 0) {
                //sort by distance
                List<GameObject> tilesUnvisitedSorted = Player.s.tilesUnvisited.OrderBy(tile => Vector2.Distance(Player.s.GetCoord(), tile.GetComponent<Tile>().coord)).ToList();
                float r = Random.value;
                destinationTile = tilesUnvisitedSorted[0];
                //choose destination using probability falloff dist
                for (int i=0; i<tilesUnvisitedSorted.Count; i++) {
                    if (r < (1 - Mathf.Pow(1-1/distSpread, i+1))) {
                        destinationTile = tilesUnvisitedSorted[i];
                        break;
                    }
                }
            } else {
                destinationTile = Player.s.tilesVisited.ToList()[0];
            }
            Player.s.Move(destinationTile);	
            base.Trigger();
        }
	}
}
		
