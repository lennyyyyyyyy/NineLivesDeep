using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Telemine : Mine {
	public static readonly float distSpread = 10f;
	public override void trigger() {
		base.trigger();
		List<GameObject> tilesUnvisitedCopy = new List<GameObject>(Player.s.tilesUnvisited);
		//sort by distance
		tilesUnvisitedCopy = tilesUnvisitedCopy.OrderBy(tile => Vector2.Distance(Player.s.coord, tile.GetComponent<Tile>().coord)).ToList();
		float r = Random.value;
		GameObject destinationTile = tilesUnvisitedCopy[0];
		//choose destination using probability falloff dist
		for (int i=0; i<tilesUnvisitedCopy.Count; i++) {
			if (r < (1 - Mathf.Pow(1-1/distSpread, i+1))) {
				destinationTile = tilesUnvisitedCopy[i];
				break;
			}
		}
		Player.s.setCoord(destinationTile);	
	}
}
		
