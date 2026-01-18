using UnityEngine;
using TMPro;

public class MineUIItem : UIItem {
	public static MineUIItem s;
	public TMP_Text count;
	protected override void BeforeInit() {
        base.BeforeInit();
		s = this;
        transform.SetParent(GameUIManager.s.notFlagGroup.transform, false);
		count = GetComponentInChildren<TMP_Text>();
		PlayerUIItemModule.s.notFlags.Add(gameObject);
		PlayerUIItemModule.s.OrganizeNotFlags();
	}
}
