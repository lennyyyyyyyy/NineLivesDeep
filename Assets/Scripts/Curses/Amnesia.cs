using UnityEngine;

class Amnesia : Curse {
	public override void Modify(ref Modifiers modifiers) {
		if (intensified) {
			modifiers.amnesiaUITypes.Add(typeof(Flag));
			modifiers.amnesiaUITypes.Add(typeof(Mine));
			modifiers.mapNumberDisappearChancePerSecond = 0.01f;
		} else {
			modifiers.amnesiaUITypes.Add(typeof(Flag));
			modifiers.mapNumberDisappearChancePerSecond = 0.005f;
		}
	}
}

