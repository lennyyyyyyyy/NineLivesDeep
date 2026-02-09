using UnityEngine;

public class Curious : Passive {
	public override void Modify(ref Modifiers modifiers) {
        if (!usable) return;
		modifiers.vision += 2;
	}
}
