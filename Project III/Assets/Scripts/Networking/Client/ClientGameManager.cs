using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

public class ClientGameManager :IDisposable
{
    private JoinAllocation allocation;

    private NetworkClient networkClient;
    private MatchplayMatchmaker matchmaker;
    private UserData userData;

    private const string MenuSceneName = "Menu";
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();  

        networkClient = new NetworkClient(NetworkManager.Singleton);
        matchmaker = new MatchplayMatchmaker();

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if (authState == AuthState.Authenticated)
        {
            userData= new UserData
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing name"),
                userAuthId = AuthenticationService.Instance.PlayerId
            };
            return true;
        }

        return false;
    }

    public void GotoMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relaySeverData = new RelayServerData(allocation, "udp");
        transport.SetRelayServerData(relaySeverData);

        
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();

        //NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult =  await matchmaker.Matchmake(userData);

        if(matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            // connect to server
        }

        return matchmakingResult.result;
    }

    internal void Disconnect()
    {
        networkClient.Disconnect();
    }

    public void Dispose()
    {
        networkClient?.Dispose();
    }
}
