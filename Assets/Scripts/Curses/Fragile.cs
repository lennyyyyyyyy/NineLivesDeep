using UnityEngine;

class Fragile : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensified) {
			modifiers.tempChangesUntilDeath = 10;
		} else {
			modifiers.tempChangesUntilDeath = 5;
		}
	}
}

