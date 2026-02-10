using UnityEngine;

public class Aromatic : Passive {
	public override void Modify(ref Modifiers modifiers) {
        if (!usable) return;
		modifiers.discoverRange++;
	}
}
