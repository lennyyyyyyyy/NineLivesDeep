using UnityEngine;

public class You : Placeable {
    protected override bool IsUsable() {
        return !Player.s.alive && count > 0;
    }
}
