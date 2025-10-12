using UnityEngine;

public class Milk : Passive {
	public override void Modify(ref Modifiers modifiers) {
		modifiers.interactRange++;
	}
}
