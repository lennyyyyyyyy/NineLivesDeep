using UnityEngine;

class Wildcat : Passive {
    protected override void Start() {
        showCount = true;
        count = 5;
        base.Start();
    }
    public override void UpdateCount(int newCount) {
        if (newCount == 0) {
            count = 5;
            Flag y = UIManager.s.flagUIVars[typeof(You)].instances[0].GetComponent<Flag>();
            y.UpdateCount(y.count+1);
            
        } else {
            count = newCount;
            UIManager.s.InstantiateBubble(gameObject, newCount.ToString() + " left...", Color.white);
        }
        tmpro.text = count.ToString();
    }
}