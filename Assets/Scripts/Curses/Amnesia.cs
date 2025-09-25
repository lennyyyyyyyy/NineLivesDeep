using UnityEngine;

class Amnesia : Curse {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.amnesia = true;
	}
}

