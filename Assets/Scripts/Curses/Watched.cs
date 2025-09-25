using UnityEngine;

class Watched : Curse {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.watched = true;
	}
}
