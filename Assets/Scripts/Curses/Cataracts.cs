using UnityEngine;

class Cataracts : Curse {
	public override void Modify(ref Modifiers modifiers) {
        if (!usable) return;
		if (intensifiedBy.Count > 0) {
			modifiers.cataractConfuseChance = 0.66f;
		} else {
			modifiers.cataractConfuseChance = 0.33f;
		}
	}
}

