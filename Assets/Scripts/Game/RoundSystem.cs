using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundSystem : NetworkBehaviour
{
    [SerializeField] private Animator animator = null;

    private List<Player> playerList = new List<Player>();

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    #region Server
    public override void OnStartServer()
    {
        NetworkManagerLobby.OnServerStopped += CleanUpServer;
        NetworkManagerLobby.OnServerReadied += CheckStartRound;
        GetAllPlayers();
    }

    [ServerCallback]
    private void OnDestroy() => CleanUpServer();
    
    [Server]
    private void CleanUpServer()
    {
        NetworkManagerLobby.OnServerStopped -= CleanUpServer;
        NetworkManagerLobby.OnServerReadied -= CheckStartRound;
    }

    [ServerCallback]
    public void StartRound()
    {
        RpcStartRound();
    }

    [Server]
    private void CheckStartRound(NetworkConnection conn)
    {
        if (Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers.Count) { return; }

        animator.enabled = true;

        RpcStartCountDown();
    }
    #endregion

    #region Client
    [ClientRpc]
    private void RpcStartCountDown()
    {
        animator.enabled = true;
    }

    [ClientRpc]
    private void RpcStartRound()
    {
        UnlockPlayer();

        GameManager.Instance.enabled = true;
        GameManager.Instance.playerList = playerList;
    }
    #endregion

    public void CoundownEnded()
    {
        animator.enabled = false;
    }

    [Server]
    public void GetAllPlayers()
    {
        Invoke(nameof(FindPlayers), 1f);
    }

    public void FindPlayers()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        playerList = new List<Player>();
        foreach (var playerObject in allPlayers)
        {
            Player playerScript = playerObject.GetComponent<Player>();
            AnimatedSprite animatedSprite = playerObject.GetComponent<AnimatedSprite>();
            if (playerScript != null)
            {
                playerList.Add(playerScript);
            }
        }

        Debug.Log("Found " + playerList.Count + " players.");
    }

    [Server]
    public void UnlockPlayer()
    {
        foreach (var player in playerList)
        {
            player.gameObject.GetComponent<AnimatedSprite>().enabled = true;
            player.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    
    }
}