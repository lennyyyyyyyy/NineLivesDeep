using UnityEngine;

class Wobbly : Curse {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.wobbly = true;
	}
}

