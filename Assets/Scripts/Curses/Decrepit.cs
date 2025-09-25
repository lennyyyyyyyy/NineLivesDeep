using UnityEngine;

class Decrepit : Curse {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.noTileChance += 0.1f;
	}
}

