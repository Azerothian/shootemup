using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class CameraController : MonoBehaviour
{
  
  public Transform CameraTarget;
  public Transform CursorTarget;
  //public float cursorDistance = 5;
  public float sensitivity = 5;
  public float runningSensitivity = 0.1f;
  
  public float ViewDistance = 4f;
  public float FocusedViewDistance = 2f;
  public int ZoomRate = 20;

  private float x = 0.0f;
  private float y = 0.0f;
  private int lerpRate = 5;
  private float distance = 4f;
  private float desireDistance;
  private float correctedDistance;
  private float currentDistance;

  public float cameraTargetHeight = 1.0f;

  //checks if first person mode is on
  private bool click = false;
  //stores cameras distance from player
  private float curDist = 0;

  // Use this for initialization
  void Start()
  {
    Vector3 Angles = transform.eulerAngles;
    x = Angles.x;
    y = Angles.y;
    currentDistance = distance;
    correctedDistance = distance;
  }

  // Update is called once per frame
  void LateUpdate()
  {



    if (Input.GetKeyDown(KeyCode.Escape)|| Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.LeftControl)) {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }
    if (Cursor.lockState == CursorLockMode.Locked)
    {
      if (CameraTarget.GetComponent<PlayerController>().PlayerState == PlayerAnimationState.Running)
      {
        x += Input.GetAxis("Mouse X") * runningSensitivity;
        y -= Input.GetAxis("Mouse Y") * runningSensitivity;
      } else
      {

        x += Input.GetAxis("Mouse X") * sensitivity;
        y -= Input.GetAxis("Mouse Y") * sensitivity;
      }
        //desireDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * ZoomRate * Mathf.Abs(desireDistance);
      }
    else if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButton(0) || Input.GetMouseButton(1))
    {

      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
    }

    y = ClampAngle(y, -15, 25);

    Quaternion rotation = Quaternion.Euler(y, x, 0);


    //desireDistance = Mathf.Clamp(desireDistance, MinViewDistance, MaxViewDistance);
    var desireDistance = ViewDistance;
    if(Input.GetMouseButton(1))
    {
      desireDistance = FocusedViewDistance;
    }
    correctedDistance = desireDistance;

    Vector3 position = CameraTarget.position - (rotation * Vector3.forward * desireDistance);

    RaycastHit collisionHit;
    Vector3 cameraTargetPosition = new Vector3(CameraTarget.position.x, CameraTarget.position.y + cameraTargetHeight, CameraTarget.position.z);

    bool isCorrected = false;
    if (Physics.Linecast(cameraTargetPosition, position, out collisionHit))
    {
      position = collisionHit.point;
      correctedDistance = Vector3.Distance(cameraTargetPosition, position);
      isCorrected = true;
    }
    currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * ZoomRate) : correctedDistance;

    position = CameraTarget.position - (rotation * Vector3.forward * currentDistance + new Vector3(0, -cameraTargetHeight, 0));

    transform.rotation = rotation;
    transform.position = position;

    float cameraX = transform.rotation.x;
    CameraTarget.eulerAngles = new Vector3(cameraX, transform.eulerAngles.y, transform.eulerAngles.z);
    
  }

  private static float ClampAngle(float angle, float min, float max)
  {
    if (angle < -360)
    {
      angle += 360;
    }
    if (angle > 360)
    {
      angle -= 360;
    }
    return Mathf.Clamp(angle, min, max);
  }
}
