using UnityEngine;

class Decrepit : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensified) {
			modifiers.noTileChance += 0.2f;
		} else {
			modifiers.noTileChance += 0.1f;
		}
	}
}

