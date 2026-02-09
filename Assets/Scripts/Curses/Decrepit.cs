using UnityEngine;

class Decrepit : Curse {
	public override void Modify(ref Modifiers modifiers) {
        if (!usable) return;
		if (intensifiedBy.Count > 0) {
			modifiers.noTileChance += 0.2f;
		} else {
			modifiers.noTileChance += 0.1f;
		}
	}
}

