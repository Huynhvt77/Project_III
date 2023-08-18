using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    public ClientGameManager GameManager;

    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null) 
            {
                return instance;
            }

            instance = FindObjectOfType<ClientSingleton>();

            if (instance == null)
            {
                Debug.LogError("No ClientSingleton in the scene!");
            }

            return instance;
        }
    }
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task CreateClient()
    {
        GameManager = new ClientGameManager();

        await GameManager.InitAsync();
    }
}
