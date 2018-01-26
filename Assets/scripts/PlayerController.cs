using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 
 */


public enum PlayerAnimationState
{
  Idle = 0,
  Walking = 1,
  Running = 2,
  WalkingBackwards = 3,
  Jumping = 4,
  StepLeft = 5,
  StepRight = 6,
  Aiming = 7,
}


public class PlayerController : MonoBehaviour
{
  public bool isGrounded { get; set; }
  // Use this for initialization
  public float walkSpeed = 5;
  public float runSpeed = 10;
  public float sideStepSpeed = 10;
  public float jumpSpeed = 2;
  public GameObject targetModel;
  public PlayerAnimationState PlayerState
  {
    get
    {
      return (PlayerAnimationState)targetModel.GetComponent<Animator>().GetInteger("ActionState");
    }
    set
    {
      if (value != PlayerState)
      {
        targetModel.GetComponent<Animator>().SetInteger("ActionState", (int)value);
      }
    }
  }
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    //var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
    //var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

    //transform.Rotate(0, x, 0);
    //transform.Translate(0, 0, z);


  }
  private void OnCollisionEnter(Collision collision)
  {
    isGrounded = true;
  }
  private void OnCollisionExit(Collision collision)
  {
  }

  void FixedUpdate()
  {
   
    var movementSpeed = walkSpeed;

    if (Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(1))
    {
      movementSpeed = runSpeed;
    }

    if (isGrounded && Input.GetKey(KeyCode.Space))
    {
      isGrounded = false;
      this.GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpSpeed, 0));
    }

    float horizontal = Input.GetAxis("Horizontal") * sideStepSpeed;// * Time.deltaTime;
    float vertical = Input.GetAxis("Vertical") * movementSpeed;// * Time.deltaTime;
    
    if (isGrounded)
    {
      if (horizontal != 0)
      {

        //Debug.Log(string.Format("horri {0}", horizontal));
        Vector3 v3SideForce = horizontal * transform.right;
        this.GetComponent<Rigidbody>().AddForce(v3SideForce);
      }
      if (horizontal > 0)
      {
        PlayerState = PlayerAnimationState.StepLeft;
      }
      else
      {
        PlayerState = PlayerAnimationState.StepRight;

      }
      Vector3 v3Force = vertical * transform.forward;
      this.GetComponent<Rigidbody>().AddForce(v3Force);
      //transform.Translate(0, 0, vertical);
      if (vertical < 0)
      {
        PlayerState = PlayerAnimationState.WalkingBackwards;
      }
      else if (horizontal == 0 && vertical == 0)
      {
        if (Input.GetMouseButton(1))
        {
          PlayerState = PlayerAnimationState.Aiming;
        } else
        {
          PlayerState = PlayerAnimationState.Idle;
        }

      }
      else
      {
        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(1))
        {
          PlayerState = PlayerAnimationState.Running;
        }
        else
        {
          if (Input.GetMouseButton(1))
          {
            PlayerState = PlayerAnimationState.Aiming;
          }
          else
          {
            PlayerState = PlayerAnimationState.Walking;
          }
        }


      }
    }
    else
    {
      PlayerState = PlayerAnimationState.Jumping;
    }



  }
}
