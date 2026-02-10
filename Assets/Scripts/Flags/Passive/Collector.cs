using UnityEngine;

class Collector : Passive {
    public override void UpdateCount(int newCount) {
        HelperManager.s.InstantiateBubble(gameObject, "Caught " + (newCount - count).ToString() + " more mine!", Color.white);
        count = newCount;
        tmpro.text = count.ToString();
    }
    protected virtual void OnMineDefused(bool defusedBefore) {
        if (!defusedBefore) {
            UpdateCount(count+1);
            Player.s.UpdateMoney(Player.s.money + count * Player.s.modifiers.mineDefuseMult);
        }
    }
    protected override void OnEnable() {
        base.OnEnable();
        EventManager.s.OnMineDefused += OnMineDefused;
    }
    protected override void OnDisable() {
        base.OnDisable();
        EventManager.s.OnMineDefused -= OnMineDefused;
    }
}
