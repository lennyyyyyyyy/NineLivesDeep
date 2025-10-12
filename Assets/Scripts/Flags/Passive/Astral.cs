using UnityEngine;

public class Astral : Passive {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.reviveRange++;
	}
}
