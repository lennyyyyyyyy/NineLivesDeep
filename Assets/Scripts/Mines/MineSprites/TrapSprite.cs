using UnityEngine;

public class TrapSprite : MineSprite {
    public override void trigger() {
        base.trigger();
        Player.s.setTrapped(true);
        Player.s.updatePrints();
    }
}
