using UnityEngine;

public class Gambling : Passive {
    public override void Modify(ref Modifiers modifiers) {
        if (!usable) return;
        modifiers.gambling = true;
        modifiers.gamblingSafeChance *= 0.8f;
    }
}
