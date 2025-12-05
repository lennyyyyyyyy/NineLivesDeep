using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour {
	public bool setInitialData = false;
	public Texture2D tex2d;
	public TooltipData tooltipData;

	protected AddTooltipUI addTooltip;

	[System.NonSerialized]
	public RectTransform rt;

	protected virtual void Start() {
		rt = (transform as RectTransform);
		addTooltip = (GetComponent<AddTooltipUI>() == null ? gameObject.AddComponent(typeof(AddTooltipUI)) as AddTooltipUI : GetComponent<AddTooltipUI>());

		if (setInitialData) {
			SetData(tex2d, tooltipData);
		} else {
			SetDefaultData();
		}

		if (UIManager.s.uiTypeToData.ContainsKey(GetType())) {
			UIManager.s.uiTypeToData[GetType()].instances.Add(gameObject);
		}
	}		
	public virtual void SetInitialData(Texture2D? tex2d = null, TooltipData tooltipData = null) {
		setInitialData = true;
		this.tex2d = tex2d ?? this.tex2d;
		this.tooltipData = tooltipData ?? this.tooltipData; 
	}
    public virtual void SetData(Texture2D? tex2d = null, TooltipData tooltipData = null) {
		SetInitialData(tex2d, tooltipData);

		GetComponent<RawImage>().texture = this.tex2d;
		addTooltip.SetData(this.tooltipData, true);
    }
	protected virtual void SetDefaultData() {
		if (UIManager.s.uiTypeToData.ContainsKey(GetType())) {
			UIItemData uiItemData = UIManager.s.uiTypeToData[GetType()];
			SetData(uiItemData.tex2d, uiItemData.tooltipData);
		}
	}
	public virtual void Modify(ref Modifiers modifiers) {
	}
}
