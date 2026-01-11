using UnityEngine;

public class You : Placeable {
    protected override void Start() {
        base.Start();
    }
    protected override bool IsUsable() {
        return !Player.s.alive;
    }
}
