using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool ShowDebugHUD = true;
    public string HostIP;

    private GameObject touchCanvas;

    public static string GetLocalIPAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
        }
        catch { }
        return "127.0.0.1";
    }

    void Start()
    {
        touchCanvas = NamedGameObjects.TouchCanvas;
        touchCanvas.SetActive(false);
        HostIP = GetLocalIPAddress();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        var networkManager = NetworkManager.Singleton;
        if (!networkManager.IsClient && !networkManager.IsServer)
        {
            HostIP = GUILayout.TextField(HostIP, 25);

            if (GUILayout.Button("Host"))
            {
                networkManager.GetComponent<UnityTransport>().ConnectionData.Address = HostIP;
                networkManager.StartHost();
                touchCanvas.SetActive(true);
            }

            if (GUILayout.Button("Client"))
            {
                networkManager.GetComponent<UnityTransport>().ConnectionData.Address = HostIP;
                networkManager.StartClient();
                touchCanvas.SetActive(true);
            }

            if (GUILayout.Button("Server"))
            {
                networkManager.GetComponent<UnityTransport>().ConnectionData.Address = HostIP;
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
