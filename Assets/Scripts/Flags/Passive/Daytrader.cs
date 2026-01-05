using UnityEngine;

class Daytrader : Passive {
    protected virtual void OnNewMinefield() {
        if (Random.value < 0.5f) {
            Player.s.UpdateMoney(Mathf.FloorToInt(Player.s.money * 1.5f));
        } else {
            Player.s.UpdateMoney(Mathf.CeilToInt(Player.s.money * 0.75f));
        }
    }
    protected override void OnEnable() {
        base.OnEnable();
        EventManager.s.OnNewMinefield += OnNewMinefield;
    }
    protected override void OnDisable() {
        base.OnDisable();
        EventManager.s.OnNewMinefield -= OnNewMinefield;
    }
}
