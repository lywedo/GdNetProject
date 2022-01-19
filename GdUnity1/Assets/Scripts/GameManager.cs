using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Net.Component;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private void Start()
    {
        ClientManager.Instance.client.OnConnectedHandle += Connected;
    }

    private async void Connected()
    {
        await Task.Delay(1000);//不知道你这里什么原因， 必须延迟1秒后再实例化
        SpawnPlayer(Vector3.zero, Quaternion.identity);
    }

    public void SpawnPlayer(Vector3 position, Quaternion rotation)
    {
        GameObject _player = Instantiate(PlayerPrefab, position, rotation);
        _player.GetComponent<TransformComponent>().m_identity = ClientManager.UID;
        _player.GetComponent<PlayerController>().enabled = true;
        Debug.Log($"spawnplayer:{ClientManager.UID}");
    }

    private void OnDestroy()
    {
        ClientManager.Instance.client.OnConnectedHandle -= Connected;
    }
}