using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Consumable : Flag {
    protected override void BeforeInit() {
        base.BeforeInit();
		HelperManager.s.SetupUIEventTriggers(gameObject,
                                             new EventTriggerType[] {EventTriggerType.PointerClick},
                                             new Action<PointerEventData>[] {OnPointerClick});
    }
    protected virtual void OnPointerClick(PointerEventData data) {}
    protected override bool IsUsable() {
        return base.IsUsable() && count > 0 && GameManager.s.floorState == GameManager.GameState.FLOOR_STABLE;
    }
    public override void UpdateCount(int newCount) {
        base.UpdateCount(newCount);
        if (count <= 0) {
            Destroy(gameObject);
        }
    }
    private void Start() {
        if (PlayerUIItemModule.s.typeToInstances[GetType()].Count > 1 &&
            PlayerUIItemModule.s.typeToInstances[GetType()][0] != gameObject) {
            Consumable existingConsumable = PlayerUIItemModule.s.typeToInstances[GetType()][0].GetComponent<Consumable>();
            existingConsumable.UpdateCount(existingConsumable.count + count);
            Destroy(gameObject);
        }
    }
}
