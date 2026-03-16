using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System;

public class Server : MonoBehaviour
{
    public string m_serverAddress = "ws://localhost:3000";

    public SocketIOUnity m_serverSocket;
           
    
    // Start is called before the first frame update
    void Start()
    {
        m_serverSocket = new SocketIOUnity(new Uri(m_serverAddress), new SocketIOOptions
        {
            Query = new Dictionary<string, string>
            {
                {"token", "UNITY"}
            },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        m_serverSocket.OnConnected += (sender, err) =>
        {
            Debug.Log("Connected!");
        };
                
        m_serverSocket.OnAnyInUnityThread((name, response) =>
        {
            Debug.Log("Server recieved data on '" + name + "': " + response.ToString());
        });

        m_serverSocket.Connect();

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
