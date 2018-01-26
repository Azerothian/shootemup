using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : Photon.MonoBehaviour
{
  public float bulletSpeed = 1000;
  public GameObject bulletPrefab;
  public GameObject text;
  public Transform targetCamera;
  public Transform bulletEmitter;
  public int clipCount = 5;
  public int clipSize = 30;
  public int shotsInClip;
  public float shotsPerSecond = 13;
  private float shotInterval;
  private float lastShot;
  // Use this for initialization
  void Start()
  {
    lastShot = Time.fixedTime;
    shotInterval = 1 / shotsPerSecond; //in ms
    shotsInClip = clipSize;
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    if (photonView.isMine)
    {
      text.GetComponent<UnityEngine.UI.Text>().text = string.Format("Ammo\n{0} / {1}", shotsInClip, clipCount * clipSize);
      if (Input.GetMouseButton(0))
      {
        if (shotsInClip > 0)
        {
          if (Time.fixedTime - lastShot > shotInterval)
          {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("fireBullet_RPC", PhotonTargets.All, bulletEmitter.position, targetCamera.rotation, targetCamera.forward);
            lastShot = Time.fixedTime;
            shotsInClip--;
          }

        }
      }
      if (Input.GetKeyDown(KeyCode.R) && clipCount > 0)
      {
        clipCount--;
        shotsInClip = clipSize;
      }
    }
  }


  [PunRPC]
  void fireBullet_RPC(Vector3 start, Quaternion sourceRotation, Vector3 forward)
  {
    var newBullet = Instantiate(bulletPrefab, start, sourceRotation);
    newBullet.GetComponent<BulletController>().source = start;
    Vector3 v3Force = bulletSpeed * forward;
    newBullet.GetComponent<Rigidbody>().AddForce(v3Force);
  }
}
//-0.04 1.6 1.133
//0.01;