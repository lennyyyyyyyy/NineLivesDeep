using UnityEngine;

class Reflection : Passive {
    public override void Modify(ref Modifiers modifiers) {
        if (!usable) return;
        modifiers.reflectionPassiveCount += 1;
    }
}
