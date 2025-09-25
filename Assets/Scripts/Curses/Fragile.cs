using UnityEngine;

class Fragile : Curse {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.tempChangesUntilDeath = 5;
	}
}

