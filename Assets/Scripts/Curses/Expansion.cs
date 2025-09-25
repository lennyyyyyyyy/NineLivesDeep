using UnityEngine;

class Expansion : Curse {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.floorExpansion += 3;
	}
}

