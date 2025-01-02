using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGamePlayer : NetworkBehaviour
{
    [SyncVar]
    public string displayName = "Loading...";

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(this);
        Room.GamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }
}
