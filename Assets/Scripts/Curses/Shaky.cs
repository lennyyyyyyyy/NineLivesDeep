using UnityEngine;

class Shaky : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensified) {
			modifiers.cameraShakeStrength += 1.5f;
			modifiers.cameraShakePeriod *= 0.3f;
		} else {
			modifiers.cameraShakeStrength += 1f;
			modifiers.cameraShakePeriod *= 0.5f;
		}
	}
}

