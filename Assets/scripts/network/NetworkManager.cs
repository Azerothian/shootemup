using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
  static NetworkManager _context;
  public static  NetworkManager Context {
    get {
      return _context;
    }
  }
  const string VERSION = "0.0.1";
  const string ROOMNAME = "main";
  public GameObject playerPrefab;
  public Transform[] spawnLocations;
  const byte playerGroup = 2;
  void Start()
  {
    _context = this;
    PhotonNetwork.ConnectUsingSettings(VERSION);
  }
  void OnJoinedLobby()
  {
    PhotonNetwork.JoinOrCreateRoom(ROOMNAME, new RoomOptions()
    {
      IsVisible = true,
      MaxPlayers = 20,
    }, TypedLobby.Default);
  }

  void OnJoinedRoom()
  {
    SpawnPlayer();
  }
  void OnGUI()
  {
    GUI.Label(new Rect(10, 5, 1000, 20), string.Format("{0}ms", PhotonNetwork.networkingPeer.RoundTripTime));
  }
  public void SpawnPlayer()
  {
    var index = Random.Range(0, spawnLocations.Length - 1);
    var result = PhotonNetwork.Instantiate(playerPrefab.name, spawnLocations[index].position, spawnLocations[index].rotation, 0);
    Debug.Log(result);
  }
  
}