using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Flag : UIItem {
    protected bool showCount = false;
    [System.NonSerialized]
    public bool usable = true;
    public int count;
    protected TMP_Text tmpro;
    [System.NonSerialized]
    protected EventTrigger trigger;
    protected EventTrigger.Entry entry;
    protected virtual void OnPointerEnter(PointerEventData data) {
		//amensia curse passive
		if (Player.s.modifiers.amnesia && addTooltip.tooltip.GetComponent<Tooltip>().name.text != "???") {
			addTooltip.tooltip.GetComponent<Tooltip>().Init("???", "???", "???", UIManager.s.flagUIVars[GetType()].color);
		} else if (!Player.s.modifiers.amnesia && addTooltip.tooltip.GetComponent<Tooltip>().name.text == "???") {
			UIVars uivars = UIManager.s.flagUIVars[GetType()];
			addTooltip.tooltip.GetComponent<Tooltip>().Init(uivars.name, uivars.flavor, uivars.info, UIManager.s.flagUIVars[GetType()].color);
		}	
    } 
    protected virtual void OnPointerExit(PointerEventData data) {
    }
    public virtual void UpdateCount(int newCount) {
        UIManager.s.InstantiateBubble(gameObject, (newCount - count >= 0 ? "+" : "-") + Mathf.Abs(newCount - count).ToString(), Color.white);
        count = newCount;
        tmpro.text = count.ToString();
    }
    protected virtual void init() {
        if (UIManager.s.flagUIVars.ContainsKey(GetType())) {
			FlagUIVars uivars = UIManager.s.flagUIVars[GetType()];
            init(uivars.tex2d, uivars.name, uivars.flavor, uivars.info, uivars.color, uivars.showCount);
        }
    }
    protected virtual void init(Texture2D t, string n, string f, string i, Color c, bool showC) {
		tex2d = t;
		name = n;
		flavor = f;
		info = i;
		color = c;
		showCount = showC;
    }
    protected override void Start() {
		if (tex2d == null) {
			init();
		}

		base.Start();
		
        tmpro = GetComponentInChildren<TMP_Text>(); // enable or disable count
        UpdateCount(count);
        tmpro.enabled = showCount;
        
        trigger = GetComponent<EventTrigger>() == null ? gameObject.AddComponent<EventTrigger>() : GetComponent<EventTrigger>();  // setup trigger pointer enter
        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerEnter};
        entry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerExit};
        entry.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        trigger.triggers.Add(entry);
    
        Player.s.flags.Add(gameObject); // add to player flags
        Player.s.NoticeFlag(GetType()); // mark flag type as seen
        UIManager.s.AddPaw(); // add paw to ui
        UIManager.s.OrganizeFlags(); // organize flags in ui

        UIManager.s.flagUIVars[GetType()].instances.Add(gameObject);
    }
    protected virtual bool IsUsable() {
        return Player.s.alive && !(Player.s.modifiers.taken && Player.s.takenDisabledFlag == gameObject);
    }
    public virtual void UpdateUsable() {
        usable = IsUsable();
    } 
}
