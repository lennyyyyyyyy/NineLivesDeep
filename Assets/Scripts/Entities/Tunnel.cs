using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Tunnel : Entity {
    protected override void BeforeInit() {
        base.BeforeInit();
        Floor.s.tunnels.Add(this);
    }
    private void OnPlayerMoveToCoord(int x, int y) {
        if (GetCoord().x == x && GetCoord().y == y && Floor.s.tunnels.Count > 1) {
            if (Player.s.tunneledLastMove) {
                Player.s.tunneledLastMove = false;
            } else {
                List<Tunnel> possibleTunnels = Floor.s.tunnels.Where(t => t != this).ToList();
                Vector2Int newCoord = possibleTunnels[Random.Range(0, possibleTunnels.Count)].GetCoord();
                Player.s.tunneledLastMove = true;
                Player.s.Move(newCoord.x, newCoord.y, animate: false);
            }
        }
    }
    private void OnEnable() {
        EventManager.s.OnPlayerMoveToCoord += OnPlayerMoveToCoord;
    }
    private void OnDisable() {
        EventManager.s.OnPlayerMoveToCoord -= OnPlayerMoveToCoord;
    }
    private void OnDestroy() {
        Floor.s.tunnels.Remove(this);
    }
}
