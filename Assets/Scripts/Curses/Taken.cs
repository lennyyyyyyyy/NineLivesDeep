using UnityEngine;

class Taken : Curse {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.taken = true;
	}
}

