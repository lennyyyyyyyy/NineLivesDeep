using UnityEngine;

class Fragile : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensifiedBy.Count > 0) {
			modifiers.tempChangesUntilDeath = 10;
		} else {
			modifiers.tempChangesUntilDeath = 5;
		}
	}
}

