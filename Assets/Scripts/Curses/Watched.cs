using UnityEngine;

class Watched : Curse {
	public override void Modify(ref Modifiers modifiers) {
        if (!usable) return;
		modifiers.watched = true;
		if (intensifiedBy.Count > 0) {
			modifiers.watchedMineJumpTime = 4f;
			modifiers.watchedMineJumpChancePerSecond = 0.2f;
		} else {
			modifiers.watchedMineJumpTime = 6f;
			modifiers.watchedMineJumpChancePerSecond = 0.1f;
		}
	}
}
