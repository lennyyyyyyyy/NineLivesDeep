using UnityEngine;

class Bank : Passive {
    protected virtual void OnNewMinefield() {
        Player.s.UpdateMineCount(Mathf.FloorToInt(Player.s.money * 1.1f));
    }
    protected override void OnEnable() {
        base.OnEnable();
        Floor.onNewMinefield += OnNewMinefield;
    }
    protected override void OnDisable() {
        base.OnDisable();
        Floor.onNewMinefield -= OnNewMinefield;
    }
}
