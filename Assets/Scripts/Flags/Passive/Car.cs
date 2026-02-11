using UnityEngine;

public class Car : Passive {
    public override void Modify(ref Modifiers modifiers) {
        if (usable && Time.time - Floor.s.floorStartTime < 120) {
            modifiers.extraShopFlags++;
        }
    }
}
