// This file is the base class for AddTooltipScene and AddTooltipUI. Change this class to modify both. 
using UnityEngine;

public class AddTooltip : MonoBehaviour {
	public bool setDefaultData = false;
	public TooltipData data;
	public bool addHoverEffect;
	[System.NonSerialized]
	public GameObject tooltip;
	protected bool hovered = false;
	protected Material savedMaterial;

	protected virtual void Start() {
		if (setDefaultData) {
			SetData(data);
		}
		SaveMaterial();
	}
	protected virtual void Update() {}
	public virtual void SetData(TooltipData tooltipData) {
        tooltip = Instantiate(GameManager.s.tooltip_p, UIManager.s.tooltipGroup.transform);
        tooltip.GetComponent<Tooltip>().SetData(tooltipData);
        tooltip.active = false;
	}
	protected virtual void SaveMaterial() {}
	protected virtual void MouseEnter() {
		hovered = true;
		tooltip.active = true;
		if (addHoverEffect) {
			HoverEffectOn();
		}
	}
	protected virtual void MouseExit() {
		hovered = false;
		tooltip.active = false;
		if (addHoverEffect) {
			HoverEffectOff();
		}
	}
	protected virtual void HoverEffectOn() {}
	protected virtual void HoverEffectOff() {}
    protected virtual void OnDestroy() {
        if (tooltip != null) {
            Destroy(tooltip);
        }
    }
}
