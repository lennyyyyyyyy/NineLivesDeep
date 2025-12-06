using UnityEngine;

class Cataracts : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensified) {
			modifiers.cataractConfuseChance = 0.66f;
		} else {
			modifiers.cataractConfuseChance = 0.33f;
		}
	}
}

