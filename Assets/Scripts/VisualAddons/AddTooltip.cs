// This file is the base class for AddTooltipScene and AddTooltipUI. Change this class to modify both. 
using UnityEngine;

public class AddTooltip : MonoBehaviour {
	public bool setInitialData = false;
	public TooltipData tooltipData;
	public bool addHoverEffect;

	[System.NonSerialized]
	public GameObject tooltip;
	protected bool hovered = false;
	protected Material savedMaterial;

	protected virtual void Start() {
		if (setInitialData) {
			SetData(tooltipData, addHoverEffect);
		}
		SaveMaterial();
	}
	protected virtual void Update() {}
	public virtual void SetInitialData(TooltipData tooltipData, bool addHoverEffect) {
		setInitialData = true;
		this.tooltipData = tooltipData;
		this.addHoverEffect = addHoverEffect;
	}
	public virtual void SetData(TooltipData tooltipData, bool addHoverEffect) {
		this.tooltipData = tooltipData;
		this.addHoverEffect = addHoverEffect;

		if (tooltip == null) {
			tooltip = Instantiate(GameManager.s.tooltip_p, UIManager.s.tooltipGroup.transform);
		}
        tooltip.GetComponent<Tooltip>().SetData(tooltipData);
        tooltip.active = false;
	}
	protected virtual void SaveMaterial() {}
	protected virtual void MouseEnter() {
		hovered = true;
		tooltip?.SetActive(true);
		if (addHoverEffect) {
			HoverEffectOn();
		}
	}
	protected virtual void MouseExit() {
		hovered = false;
		tooltip?.SetActive(false);
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
