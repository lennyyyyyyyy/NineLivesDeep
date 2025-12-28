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
	public virtual void SetInitialData(Texture2D? tex2d = null,
										TooltipData tooltipData = null,
										Type spriteType = null) {
		setInitialData = true;
		this.tex2d = tex2d ?? this.tex2d;
		this.tooltipData = tooltipData ?? this.tooltipData;
		this.spriteType = spriteType ?? this.spriteType;
	}
	public virtual void SetData(Texture2D? tex2d = null,
								 TooltipData tooltipData = null,
								 Type spriteType = null) {
		SetInitialData(tex2d, tooltipData, spriteType);
		ApplyInitialData();
	}
	protected override void SetDefaultData() {
		if (CatalogManager.s.typeToData.ContainsKey(GetType())) {
			MineData mineData = CatalogManager.s.typeToData[GetType()] as MineData;
			SetData(mineData.tex2d, mineData.tooltipData, mineData.spriteType);
		}
	}
}
