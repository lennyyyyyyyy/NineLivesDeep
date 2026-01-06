using UnityEngine;
using TMPro;

public class MineUIItem : UIItem {
	public static MineUIItem s;
	public TMP_Text count;
	protected virtual void Awake() {
		s = this;
        transform.SetParent(GameUIManager.s.notFlagGroup.transform);
	}
	protected override void Start() {
		base.Start();
		count = GetComponentInChildren<TMP_Text>();

		PlayerUIItemModule.s.notFlags.Add(gameObject);
		GameUIManager.s.OrganizeNotFlags();
	}
}
