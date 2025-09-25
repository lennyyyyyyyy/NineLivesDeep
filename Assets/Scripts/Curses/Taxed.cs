using UnityEngine;

class Taxed : Curse {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.mineSpawnMult *= 1.2f;
		modifiers.mineDefuseMult *= 0.8f;
	}
}
