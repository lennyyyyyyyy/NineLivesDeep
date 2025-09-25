using UnityEngine;

public class Trap : Mine
{
    public override void trigger() {
        base.trigger();
        Player.s.setTrapped(true);
        Player.s.updatePrints();
    }
}
