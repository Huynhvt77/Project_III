using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefap;

    [SerializeField] private HostSingleton hostPrefap;
    
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);  

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else{
            HostSingleton hostSingleton = Instantiate(hostPrefap);
            hostSingleton.CreateHost();

            ClientSingleton clientSingleton = Instantiate(clientPrefap);
            bool authenticated = await clientSingleton.CreateClient();

            if (authenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }
}
