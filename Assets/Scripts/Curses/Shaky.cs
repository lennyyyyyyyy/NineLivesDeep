using UnityEngine;

class Shaky : Curse {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.cameraShakeStrength = 1f;
	}
}

