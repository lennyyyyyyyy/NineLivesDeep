using UnityEngine;

class Wobbly : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensifiedBy.Count > 0) {
			modifiers.moveDirectionDisableDuration += 2;
		} else {
			modifiers.moveDirectionDisableDuration += 1;
		}
	}
}

