using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : Photon.MonoBehaviour {
  public GameObject mainCamera;
  public GameObject player;
  public Vector3 playerVelocity
  {
    get
    {
      return player.GetComponent<Rigidbody>().velocity;
    }
    set
    {
      player.GetComponent<Rigidbody>().velocity = value;
    }
  }
  
  // Use this for initialization
  void Start () {
    if (!photonView.isMine)
    {
      mainCamera.SetActive(false);
      mainCamera.GetComponent<CameraController>().enabled = false;
      player.GetComponent<PlayerController>().enabled = false;
      //Destroy(player.GetComponent<Rigidbody>());
    }
  }
  //private void FixedUpdate()
  //{
  //  if(!photonView.isMine)
  //  {
  //    player.GetComponent<Rigidbody>().velocity
  //  }
  //}

}
