using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationControler : MonoBehaviour
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
            ClientSingleton clientSingleton = Instantiate(clientPrefap);
            await clientSingleton.CreateClient();

            HostSingleton hostSingleton = Instantiate(hostPrefap);
            hostSingleton.CreateHost();
        }
    }
}
