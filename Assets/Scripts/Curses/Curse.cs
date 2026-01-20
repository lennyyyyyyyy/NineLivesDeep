using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Curse : UIItem {
    [System.NonSerialized]
    public HashSet<Intensify> intensifiedBy = new HashSet<Intensify>();

    protected override void BeforeInit() {
        base.BeforeInit();
        transform.SetParent(GameUIManager.s.notFlagGroup.transform, false);
    }
    protected override void AfterInit() {
        base.AfterInit();
        PlayerUIItemModule.s.ProcessAddedCurse(this);
    }
    protected override void OnDestroy() {
        base.OnDestroy();
        if (GameManager.s.gameState == GameManager.GameState.GAME) {
            PlayerUIItemModule.s.ProcessRemovedCurse(this);
        }
    }
}
