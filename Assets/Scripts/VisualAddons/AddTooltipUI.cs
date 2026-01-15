using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class AddTooltipUI : AddTooltip {
    protected RawImage image;
    protected override void Awake() {
        image = GetComponent<RawImage>();
		HelperManager.s.SetupUIEventTriggers(gameObject,
                                             new EventTriggerType[] {EventTriggerType.PointerEnter, EventTriggerType.PointerExit},
                                             new Action<PointerEventData>[] {OnPointerEnter, OnPointerExit});
		base.Awake();
    }
    protected override void Update() {
        if (tooltip != null && tooltip.activeSelf) {
            tooltip.GetComponent<Tooltip>().Position(transform.position.x, transform.position.y, HelperManager.s.WorldSizeFromRT((transform as RectTransform)).x);
        }
    }
	protected override void SaveMaterial() {
		savedMaterial = image.material;
	}
	protected override void HoverEffectOn() {
		image.material = UIManager.s.alphaEdgeBlueMat;
	}
	protected override void HoverEffectOff() {
		image.material = savedMaterial;
	}
    protected void OnPointerEnter(PointerEventData data) {
		base.MouseEnter();	
    }
    protected void OnPointerExit(PointerEventData data) {
		base.MouseExit();
    }
}
