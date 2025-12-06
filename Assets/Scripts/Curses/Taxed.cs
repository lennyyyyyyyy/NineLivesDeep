using UnityEngine;

class Taxed : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensified) {
			modifiers.mineSpawnMult *= 1.4f;
			modifiers.mineDefuseMult *= 0.7f;
		} else {
			modifiers.mineSpawnMult *= 1.2f;
			modifiers.mineDefuseMult *= 0.8f;
		}
	}
}
