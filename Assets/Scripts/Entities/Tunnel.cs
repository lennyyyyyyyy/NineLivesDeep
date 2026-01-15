using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Tunnel : Entity {
    private static List<Tunnel> tunnels = new List<Tunnel>();
    private static bool playerTeleportedLast = false;

    protected override void Awake() {
        tunnels.Add(this);
        base.Awake();
    }
    private void OnPlayerMoveToCoord(int x, int y) {
        if (GetCoord().x == x && GetCoord().y == y && tunnels.Count > 1) {
            if (playerTeleportedLast) {
                playerTeleportedLast = false;
            } else {
                List<Tunnel> possibleTunnels = tunnels.Where(t => t != this).ToList();
                Vector2Int newCoord = possibleTunnels[Random.Range(0, possibleTunnels.Count)].GetCoord();
                playerTeleportedLast = true;
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
        tunnels.Remove(this);
    }
}
