using UnityEngine;

class Expansion : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensified) {
			modifiers.floorExpansion += 5;
		} else {
			modifiers.floorExpansion += 3;
		}
	}
}

