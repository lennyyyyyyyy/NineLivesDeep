using UnityEngine; 
using System;

class Mine : UIItem {
	protected Type spriteType;
	protected virtual void init(Texture2D t, string n, string f, string i, Color c, Type sType) {
		init(t, n, f, i, c);
		spriteType = sType;
	}
	protected virtual void init() {
		if (UIManager.s.mineUIVars.ContainsKey(GetType())) {
			MineUIVars uivars = UIManager.s.mineUIVars[GetType()];
			init(uivars.tex2d, uivars.name, uivars.flavor, uivars.info, uivars.color, uivars.spriteType);
		}
	}
	protected override void Start() {
		if (tex2d == null) {
			init();
		}

		base.Start();
		
		Player.s.notFlags.Add(gameObject);
		Player.s.mines.Add(gameObject);
		Player.s.NoticeMine(GetType());
		UIManager.s.OrganizeNotFlags();

		UIManager.s.mineUIVars[GetType()].instances.Add(gameObject);
	}
}
