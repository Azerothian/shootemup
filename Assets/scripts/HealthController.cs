using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : Photon.MonoBehaviour
{
  public GameObject player;
  public GameObject text;
  public int maxHealth = 100;
  public int regen = 5;
  public int health;
  public int bulletDamage = 10;
  public int healthTick = 3; // 3 seconds

  private float lastTick;
  // Use this for initialization
  void Start()
  {
    health = maxHealth;
    lastTick = Time.fixedTime;
  }
  private void FixedUpdate()
  {

    if (photonView.isMine)
    {
      text.GetComponent<UnityEngine.UI.Text>().text = string.Format("Health\n{0}", health);
    }
  }
  private void OnCollisionEnter(Collision collision)
  {

    if (photonView.isMine)
    {
      var bulletController = collision.gameObject.GetComponent<BulletController>();
      if (bulletController != null)
      {
        health -= bulletDamage;
      }
      if (Time.fixedTime - lastTick > healthTick && health < maxHealth)
      {
        health += regen;
        lastTick = Time.fixedTime;
      }
      if (health <= 0)
      {
        PhotonNetwork.Destroy(player);
        NetworkManager.Context.SpawnPlayer();
      }
    }
  }
}
//-0.04 1.6 1.133
//0.01