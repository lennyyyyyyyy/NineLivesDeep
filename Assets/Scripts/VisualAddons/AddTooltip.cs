// This file is the base class for AddTooltipScene and AddTooltipUI. Change this class to modify both. 
using UnityEngine;

public class AddTooltip : MonoBehaviour {
	public bool setInitialData = false;
	public TooltipData tooltipData;
	public bool addHoverEffect = true;

	[System.NonSerialized]
	public GameObject tooltip;
	protected bool hovered = false;
	protected Material savedMaterial;

	protected virtual void Start() {
		if (setInitialData) {
			ApplyInitialData();
		}
		SaveMaterial();
	}
	protected virtual void Update() {}
	public virtual void SetInitialData(TooltipData tooltipData = null, bool? addHoverEffect = null) {
		setInitialData = true;
		this.tooltipData = tooltipData ?? this.tooltipData;
		this.addHoverEffect = addHoverEffect ?? this.addHoverEffect;
	}
	protected virtual void ApplyInitialData() {
		if (tooltipData == null) {
			Destroy(tooltip);
			return;
		}

		if (tooltip == null) {
			tooltip = Instantiate(PrefabManager.s.tooltipPrefab, UIManager.s.tooltipGroup.transform);
		}
		tooltip.GetComponent<Tooltip>().SetData(tooltipData);
		tooltip.active = false;
	}
	public virtual void SetData(TooltipData tooltipData = null, bool? addHoverEffect = null) {
		SetInitialData(tooltipData, addHoverEffect);
		ApplyInitialData();
	}
	protected virtual void SaveMaterial() {}
	public virtual void MouseEnter() {
		hovered = true;
		tooltip?.SetActive(true);
		if (addHoverEffect) {
			HoverEffectOn();
		}
	}
	public virtual void MouseExit() {
		hovered = false;
		tooltip?.SetActive(false);
		if (addHoverEffect) {
			HoverEffectOff();
		}
	}
	protected virtual void HoverEffectOn() {}
	protected virtual void HoverEffectOff() {}
    protected virtual void OnDestroy() {
		Destroy(tooltip);
    }
}
