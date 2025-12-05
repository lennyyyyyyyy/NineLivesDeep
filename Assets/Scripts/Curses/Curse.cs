using UnityEngine;
using UnityEngine.UI;

public class Curse : UIItem {
	public bool intensified = false;
    protected override void Start() {
		base.Start();
		
		Player.s.notFlags.Add(gameObject);
		Player.s.curses.Add(gameObject);
		Player.s.NoticeCurse(GetType());
		UIManager.s.OrganizeNotFlags();
	}
	protected override void SetDefaultData() {
		if (UIManager.s.uiTypeToData.ContainsKey(GetType())) {
			CurseData curseData = UIManager.s.uiTypeToData[GetType()] as CurseData;
			SetData(curseData.tex2d, curseData.tooltipData);
		}
	}
}
