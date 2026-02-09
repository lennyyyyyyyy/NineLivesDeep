using UnityEngine;

class Bank : Passive {
    protected virtual void OnNewMinefield() {
        if (!usable) return;
        Player.s.UpdateMoney(Mathf.FloorToInt(Player.s.money * 1.1f));
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
