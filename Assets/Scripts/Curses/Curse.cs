using UnityEngine;
using UnityEngine.UI;

public class Curse : UIItem {
    protected virtual void init() {
        if (UIManager.s.curseUIVars.ContainsKey(GetType())) {
			UIVars uivars = UIManager.s.curseUIVars[GetType()];
            init(uivars.tex2d, uivars.name, uivars.flavor, uivars.info, uivars.color);
        }
    }
    protected override void Start() {
		if (tex2d == null) {
			init();
		}

		base.Start();
		
		Player.s.notFlags.Add(gameObject);
		Player.s.NoticeCurse(GetType());
		UIManager.s.OrganizeNotFlags();

		UIManager.s.curseUIVars[GetType()].instances.Add(gameObject);
	}
}
