using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class ChiefSprite : MineSprite {
	public static int range = 2;
	public override void Trigger() {
		base.Trigger();
		//maybe make more efficient?
		List<GameObject> signaledMines = new List<GameObject>();
		for (int i=-range; i<=range; i++) {
			for (int j=-range; j<=range; j++) {
				GameObject mine = Floor.s.GetUniqueMine(coord.x + i, coord.y + j);
				if (mine != null && mine != this.gameObject) {
					signaledMines.Add(mine);
				}
			}
		}
		GameManager.s.Shuffle(ref signaledMines);
		//call them towards the player
		foreach (GameObject mine in signaledMines) {
			Vector2 normalizedMoveDir = ((Vector2)(coord - mine.GetComponent<MineSprite>().coord)).normalized;
			Vector2Int destCoord = mine.GetComponent<MineSprite>().coord + new Vector2Int((int)Mathf.Round(normalizedMoveDir.x), (int)Mathf.Round(normalizedMoveDir.y));
			mine.GetComponent<MineSprite>().Move(destCoord.x, destCoord.y);
		}
	}
}
