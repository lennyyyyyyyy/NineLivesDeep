using UnityEngine;

class Windy : Curse {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.windStrength += 1f;
	}
}

