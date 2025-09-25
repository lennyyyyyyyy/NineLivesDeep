using UnityEngine;

class Daytrader : Passive {
    protected virtual void OnNewMinefield() {
        if (Random.value < 0.5f) {
            Player.s.UpdateMineCount(Mathf.FloorToInt(Player.s.mines * 1.5f));
        } else {
            Player.s.UpdateMineCount(Mathf.CeilToInt(Player.s.mines * 0.75f));
        }
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