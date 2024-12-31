using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        Debug.Log("Player Disconnected 3");
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(numPlayers > maxConnections)
        {
            Debug.Log("Player Disconnected 2");
            conn.Disconnect();
            return;
        }

        //if (SceneManager.GetActiveScene().name != menuScene)
        //{
        //    Debug.Log("Player Disconnected 1");
        //    conn.Disconnect();
        //    return;
        //}
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //if(SceneManager.GetActiveScene().name == menuScene)
        //{
        //    NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

        //    NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        //}

        NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);
        NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
    }

}
