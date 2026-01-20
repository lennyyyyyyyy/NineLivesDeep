using UnityEngine; 
using UnityEngine.UI;
using System;

public class Mine : UIItem {
	public Type spriteType;

    protected override void BeforeInit() {
        base.BeforeInit();
        transform.SetParent(GameUIManager.s.notFlagGroup.transform, false);
    }
    protected override void AfterInit() {
        base.AfterInit();
        PlayerUIItemModule.s.ProcessAddedMine(this);
    }
    public virtual void Init(Texture2D? tex2d = null,
                            TooltipData tooltipData = null,
                            Type spriteType = null) {
		this.tex2d = tex2d ?? this.tex2d;
		this.tooltipData = tooltipData ?? this.tooltipData;
		this.spriteType = spriteType ?? this.spriteType;
		GetComponent<RawImage>().texture = this.tex2d;
		addTooltip.Init(this.tooltipData);
    }
    public override void Init() {
        Debug.Log("new mine init");
        if (CatalogManager.s.typeToData.ContainsKey(GetType())) {
            MineData mineData = CatalogManager.s.typeToData[GetType()] as MineData;
            Init(tex2d: mineData.tex2d, tooltipData: mineData.tooltipData, spriteType: mineData.spriteType);
        }
    }
}
