﻿#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
using Net.Component;
using System.Threading.Tasks;
using UnityEngine;
namespace Example1
{
    public class NetworkStartPosition1 : MonoBehaviour
    {
        public GameObject playerPrefab;
        public Vector2 offsetX = new Vector2(-20, 20);
        public Vector2 offsetZ = new Vector2(-20, 20);

        // Start is called before the first frame update
         void Start()
        {
            
            if (ClientManager.I.client.Connected)
                OnConnectedHandle();
            else
                ClientManager.I.client.OnConnectedHandle += OnConnectedHandle;
        }

        private async void OnConnectedHandle()
        {
            await Task.Delay(1000);
            var offset = new Vector3(Random.Range(offsetX.x, offsetX.y), 0, Random.Range(offsetZ.x, offsetZ.y));
            var player1 = Instantiate(playerPrefab, transform.position + offset, transform.rotation);
            player1.GetComponent<PlayerController1>().isLocalPlayer = true;
            Camera.main.GetComponent<ARPGcamera>().target = player1.transform;
        }

        private void OnDestroy()
        {
            ClientManager.I.client.OnConnectedHandle -= OnConnectedHandle;
        }
    }
}
#endif