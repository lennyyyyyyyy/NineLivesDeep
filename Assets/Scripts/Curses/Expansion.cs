using UnityEngine;

class Expansion : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensifiedBy.Count > 0) {
			modifiers.floorExpansion += 5;
		} else {
			modifiers.floorExpansion += 3;
		}
	}
}

