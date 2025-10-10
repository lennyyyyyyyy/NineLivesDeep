using UnityEngine;
using TMPro;

public class MineUIItem : UIItem {
	public static MineUIItem s;
	public TMP_Text count;
	protected virtual void Awake() {
		s = this;
	}
	protected override void Start() {
		base.Start();
		count = GetComponentInChildren<TMP_Text>();
		Player.s.notFlags.Add(gameObject);
		UIManager.s.OrganizeNotFlags();
	}
}
