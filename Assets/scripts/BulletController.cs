using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
  public Vector3 source;
	void FixedUpdate () {
		if(Vector3.Distance(transform.position, source) > 1000)
    {
      Destroy(gameObject);
    }
	}

  private void OnCollisionExit(Collision collision)
  {
    Destroy(gameObject);
  }
}
