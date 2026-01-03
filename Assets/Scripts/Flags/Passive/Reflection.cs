using UnityEngine;

class Reflection : Passive {
    public override void Modify(ref Modifiers modifiers) {
        modifiers.reflectionPassiveCount += 1;
    }
}
