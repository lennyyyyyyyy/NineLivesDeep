using UnityEngine;

public class TrapSprite : MineSprite {
    public override void Trigger() {
        base.Trigger();
        Player.s.setTrapped(true);
        Player.s.updatePrints();
    }
}
