using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();

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

        bool isLeader = RoomPlayers.Count == 0;

        NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

        roomPlayerInstance.IsLeader = isLeader;

        NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if(!player.IsReady) { return false; }   
        }

        return true;
    }

    public void StartGame()
    {
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            if(!IsReadyToStart()) { return; }

            ServerChangeScene("GameScene");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        for (int i = RoomPlayers.Count - 1; i >= 0; i--)
        {
            var conn = RoomPlayers[i].connectionToClient;
            var gameplayerInstance = Instantiate(gamePlayerPrefab);
            gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

            NetworkServer.Destroy(conn.identity.gameObject);

            NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
        }

        base.ServerChangeScene(newSceneName);
    }
}
