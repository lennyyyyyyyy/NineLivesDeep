using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class ChiefSprite : MineSprite {
	public static int range = 2;
	public override void trigger() {
		base.trigger();
		//maybe make more efficient?
		List<GameObject> signaledMines = new List<GameObject>();
		foreach (GameObject mine in Floor.s.mines) {
			if (mine != null && Mathf.Abs(Player.s.coord.x - mine.GetComponent<MineSprite>().coord.x) <= range && Mathf.Abs(Player.s.coord.y - mine.GetComponent<MineSprite>().coord.y) <= range) {
				signaledMines.Add(mine);
			}
		}
		GameManager.s.Shuffle(ref signaledMines);
		//call them towards the player
		foreach (GameObject mine in signaledMines) {
			Vector2 normalizedMoveDir = ((Vector2)(coord - mine.GetComponent<MineSprite>().coord)).normalized;
			Vector2Int destCoord = mine.GetComponent<MineSprite>().coord + new Vector2Int((int)Mathf.Round(normalizedMoveDir.x), (int)Mathf.Round(normalizedMoveDir.y));
			if (Floor.s.mineAvailable(destCoord.x, destCoord.y)) {
				mine.GetComponent<MineSprite>().move(destCoord.x, destCoord.y);
			}
		}
	}
}
