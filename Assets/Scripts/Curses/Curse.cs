using UnityEngine;
using UnityEngine.UI;

public class Curse : UIItem {
	public bool intensified = false;

    protected virtual void Awake() {
        transform.SetParent(GameUIManager.s.notFlagGroup.transform);
    }
    protected override void Start() {
		base.Start();
		
        PlayerUIItemModule.s.ProcessAddedCurse(this);
		GameUIManager.s.OrganizeNotFlags();
	}
	protected override void SetDefaultData() {
		if (CatalogManager.s.typeToData.ContainsKey(GetType())) {
			CurseData curseData = CatalogManager.s.typeToData[GetType()] as CurseData;
			SetData(curseData.tex2d, curseData.tooltipData);
		}
	}
    protected override void OnDestroy() {
        base.OnDestroy();
        PlayerUIItemModule.s.ProcessRemovedCurse(this);
        GameUIManager.s.OrganizeNotFlags();
    }
}
