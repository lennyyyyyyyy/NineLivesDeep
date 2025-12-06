using UnityEngine;

class Windy : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensified) {
			modifiers.windStrength += 2f;
			modifiers.windFluctuation *= 0.3f;
		} else {
			modifiers.windStrength += 1f;
			modifiers.windFluctuation *= 0.5f;
		}
	}
}

