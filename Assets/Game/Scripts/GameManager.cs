using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool ShowDebugHUD = true;
    public GameObject touchCanvas;

    void Start()
    {
        touchCanvas = NamedGameObjects.TouchCanvas;
        touchCanvas.SetActive(false);
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        var networkManager = NetworkManager.Singleton;
        if (!networkManager.IsClient && !networkManager.IsServer)
        {
            if (GUILayout.Button("Host"))
            {
                networkManager.StartHost();
                touchCanvas.SetActive(true);
            }

            if (GUILayout.Button("Client"))
            {
                networkManager.StartClient();
                touchCanvas.SetActive(true);
            }

            if (GUILayout.Button("Server"))
            {
                networkManager.StartServer();
            }
        }
        else
        {
            GUILayout.Label($"Mode: {(networkManager.IsHost ? "Host" : networkManager.IsServer ? "Server" : "Client")}");
        }

        GUILayout.EndArea();

    }

    void Update()
    {
        DebugHUD.StartFrame(ShowDebugHUD);
    }
}
