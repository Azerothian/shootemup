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
  List<GameObject> _isWall = new List<GameObject>();
  public bool isGrounded;
  public bool isWall;
  // Use this for initialization
  public float airSpeed = 0.1f;
  public float walkSpeed = 5;
  public float runSpeed = 10;
  public float sideStepSpeed = 10;
  public float jumpSpeed = 2;
  public int jumpCount = 3;
  public int defaultJumpCount = 3;
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

  void Update()
  {

  }
  private void OnCollisionEnter(Collision collision)
  {
    //collision.
    var relativePosition = transform.InverseTransformPoint(collision.contacts[0].point);
    var angle = Vector3.Angle(relativePosition, transform.up);

    if (angle > 80)
    {
      isGrounded = true;
      jumpCount = defaultJumpCount;
    }
    else
    {
      if(!_isWall.Contains(collision.gameObject))
      {
        _isWall.Add(collision.gameObject);
      }
      isWall = true;
    }
    Debug.Log("collision enter" + angle);
  }
  private void OnCollisionExit(Collision collision)
  {
    //TODO: need to exit
    if(_isWall.Contains(collision.gameObject))
    {
      isWall = false;
    } else
    {
      isGrounded = false;
    }

    //var relativePosition = transform.InverseTransformPoint(collision.contacts[0].point);
    //var angle = Vector3.Angle(relativePosition, transform.up);
    //Debug.Log("collision exit" + angle);
    //if (angle > 80)
    //{
    //  isGrounded = false;
    //  //jumpCount = defaultJumpCount;
    //}
    //else
    //{
    //  isWall = false;
    //}

    //var relativePosition = transform.InverseTransformPoint(collision.transform.position);
    //if (relativePosition.y < 0)
    //{
    //  isGrounded = false;
    //}
    //if (relativePosition.x != 0 && relativePosition.z != 0)
    //{
    //  isWall = false;
    //}
  }
  //private Vector3 avg(ContactPoint[] cp)
  //{
  //  Vector3 v = Vector3.zero;
  //  foreach(var c in cp)
  //  {
  //    v += c.point;
  //  }
  //  return v / cp.Length;
  //}
  private void checkCollision(Collision collision) {
    

  }
  void FixedUpdate()
  {

    var movementSpeed = runSpeed;

    //if (Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(1))
    //{
    //  movementSpeed = runSpeed;
    //}
    if(!isGrounded)
    {
      movementSpeed = airSpeed;
    } if(Input.GetKey(KeyCode.LeftShift)) 
    {
      movementSpeed = walkSpeed;
    }

    if ((isGrounded || isWall) && Input.GetKeyDown(KeyCode.Space))
    {

      jumpCount--;
      if (jumpCount > -1)
      {
        //isGrounded = false;
        if (!isWall)
        {
          this.GetComponent<Rigidbody>().AddForce(Quaternion.AngleAxis(-45, transform.up) * new Vector3(0, jumpSpeed, 0));
        } else
        {
          this.GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpSpeed, 0));
        }
      }
    }

    float horizontal = Input.GetAxis("Horizontal") * movementSpeed;// * Time.deltaTime;
    float vertical = Input.GetAxis("Vertical") * movementSpeed;// * Time.deltaTime;


    if (horizontal != 0)
    {

      //Debug.Log(string.Format("horri {0}", horizontal));
      Vector3 v3SideForce = horizontal * transform.right;
      this.GetComponent<Rigidbody>().transform.position += v3SideForce;
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
    this.GetComponent<Rigidbody>().transform.position += v3Force;
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
      }
      else
      {
        PlayerState = PlayerAnimationState.Idle;
      }

    }
    else
    {
      if (Input.GetKey(KeyCode.LeftShift))
      {
        PlayerState = PlayerAnimationState.Walking;
      }
      else
      {
        if (Input.GetMouseButton(1))
        {
          PlayerState = PlayerAnimationState.Aiming;
        }
        else
        {
          PlayerState = PlayerAnimationState.Running;
        }
      }


    }
    if (!isGrounded)
    {
      PlayerState = PlayerAnimationState.Jumping;
    }



  }
}
