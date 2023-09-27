using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using Unity.Collections;
using System;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private SpriteRenderer minimapIconRenderer;
    [SerializeField] private Texture2D crosshair;
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinWallet Wallet { get; private set; }

    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;
    [SerializeField] private Color ownerColour;
    //[SerializeField] public int life = 3;

    private Leaderboard leaderboard;
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> TeamIndex = new NetworkVariable<int>();

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    private double currenttime;
    public static int connections = 0;
    private double waitTime = 0f;

    private void Start() 
    {
        connections += 1;
        leaderboard = FindAnyObjectByType<Leaderboard>();
    }

    private void Update() 
    {
        Debug.Log(connections);
        if (connections == 2)
        {
            if (waitTime == 0f)
                waitTime = NetworkManager.ServerTime.Time;
        }

        if (connections > 1)
        {
            currenttime = NetworkManager.ServerTime.Time - waitTime;
            Debug.Log(currenttime);
        }

        if(currenttime > 10)
        {
            connections = 0;

            if (NetworkManager.Singleton.IsHost)
            {
                leaderboard.CenterLeaderboard();
                StartCoroutine(IShowBoardForHost());
            }

            leaderboard.CenterLeaderboard();
            StartCoroutine(IShowBoardForClient());
        }
    }
    IEnumerator IShowBoardForClient(){
        yield return new WaitForSeconds(5);
        ClientSingleton.Instance.GameManager.Disconnect();
    }
    IEnumerator IShowBoardForHost(){
        yield return new WaitForSeconds(5);
        HostSingleton.Instance.GameManager.Shutdown();
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = null;

            if (IsHost)
            {
                userData =
                    HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
            else
            {
                userData =
                    ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }

            PlayerName.Value = userData.userName;
            TeamIndex.Value = userData.teamIndex;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;

            minimapIconRenderer.color = ownerColour;

            Cursor.SetCursor(crosshair, new Vector2(crosshair.width / 2, crosshair.height / 2), CursorMode.Auto);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}

