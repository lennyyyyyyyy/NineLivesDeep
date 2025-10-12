using UnityEngine;

public class Curious : Passive {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.vision += 2;
	}
}
