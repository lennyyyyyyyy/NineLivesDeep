using UnityEngine;

public class MineUIItem : UIItem {
	protected override void Start() {
		base.Start();
		Player.s.notFlags.Add(gameObject);
		UIManager.s.OrganizeNotFlags();
	}
}
