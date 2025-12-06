using UnityEngine;

class Wobbly : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensified) {
			modifiers.moveDirectionDisableDuration += 2;
		} else {
			modifiers.moveDirectionDisableDuration += 1;
		}
	}
}

