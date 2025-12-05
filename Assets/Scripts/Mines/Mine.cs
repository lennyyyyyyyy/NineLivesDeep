using UnityEngine; 
using UnityEngine.UI;
using System;

public class Mine : UIItem {
	public Type spriteType;
	protected override void Start() {
		base.Start();
		
		Player.s.notFlags.Add(gameObject);
		Player.s.mines.Add(gameObject);
		Player.s.NoticeMine(GetType());
		UIManager.s.OrganizeNotFlags();
	}
	public virtual void SetInitialData(Texture2D tex2d, TooltipData tooltipData, Type spriteType) {
		setInitialData = true;
		this.tex2d = tex2d;
		this.tooltipData = tooltipData;
		this.spriteType = spriteType;
	}
	public virtual void SetData(Texture2D tex2d, TooltipData tooltipData, Type spriteType) {
		SetInitialData(tex2d, tooltipData, spriteType);

		GetComponent<RawImage>().texture = tex2d;
		addTooltip.SetData(tooltipData, true);
	}
	protected override void SetDefaultData() {
		if (UIManager.s.uiTypeToData.ContainsKey(GetType())) {
			MineData mineData = UIManager.s.uiTypeToData[GetType()] as MineData;
			SetData(mineData.tex2d, mineData.tooltipData, mineData.spriteType);
		}
	}
}
