// This file is the base class for AddTooltipScene and AddTooltipUI. Change this class to modify both. 
using UnityEngine;

public class AddTooltip : MonoBehaviour {
	public TooltipData tooltipData;
	public bool addHoverEffect = true;

	[System.NonSerialized]
	public GameObject tooltip;
    [System.NonSerialized]
	public bool hovered = false;
	protected Material savedMaterial;

    protected virtual void Awake() {
        SaveMaterial();
        Init();
    }
	protected virtual void Update() {}
	public virtual void Init(TooltipData tooltipData = null, bool? addHoverEffect = null) {
		this.tooltipData = tooltipData ?? this.tooltipData;
		this.addHoverEffect = addHoverEffect ?? this.addHoverEffect;
		if (this.tooltipData == null) {
			Destroy(tooltip);
			return;
		}
		if (tooltip == null) {
			tooltip = Instantiate(PrefabManager.s.tooltipPrefab, GameUIManager.s.tooltipGroup.transform);
		}
	    tooltip.GetComponent<Tooltip>().Init(this.tooltipData);
		tooltip.SetActive(false);
	}
	protected virtual void SaveMaterial() {}
	public virtual void MouseEnter() {
		hovered = true;
        if (tooltip) {
            tooltip.SetActive(true);
            tooltip.transform.position = Tooltip.lastTooltipPos;
        }
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
