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
  public float testRayLength = 0.5f;
  public GameObject targetModel;
  private Rigidbody rigidBody;
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
    rigidBody = this.GetComponent<Rigidbody>();
  }

  void Update()
  {


  }


  bool testLine(Vector3 left, Vector3 right)
  {
    var test = Physics.Linecast(left, right);
    Debug.DrawLine(left, right, (test) ? Color.red : Color.white);
    return test;
  }

  void FixedUpdate()
  {

    var boxCollider = this.GetComponent<BoxCollider>();
    var depth = (boxCollider.size.z / 2);
    var height = boxCollider.size.y / 2;
    var width = boxCollider.size.x / 2;
    var forward = (depth * transform.forward);

    var innerCenter = rigidBody.worldCenterOfMass + forward;
    var innerCenterLeft = innerCenter - (width * transform.right);
    var innerCenterRight = innerCenter + (width * transform.right);

    var testCenter = testLine(innerCenter, innerCenter + (0.1f * transform.forward));
    var testCenterLeft = testLine(innerCenterLeft, innerCenterLeft + (0.1f * transform.forward));
    var testCenterRight = testLine(innerCenterRight, innerCenterRight + (0.1f * transform.forward));


    var innerBottom = innerCenter - ((height - 0.5f) * transform.up);
    var innerBottomLeft = innerBottom - (width * transform.right);
    var innerBottomRight = innerBottom + (width * transform.right);

    var testBottom = testLine(innerBottom, innerBottom + (0.1f * transform.forward));
    var testBottomLeft = testLine(innerBottomLeft, innerBottomLeft + (0.1f * transform.forward));
    var testBottomRight = testLine(innerBottomRight, innerBottomRight + (0.1f * transform.forward));

    var center = testCenter || testCenterLeft || testCenterRight;
    var bottom = testBottom || testBottomLeft || testBottomRight;

    var innerDownCenter = rigidBody.worldCenterOfMass - (height * transform.up);
    var outerDownCenter = innerDownCenter - (0.1f * transform.up);

    var downPoint1Inner = innerDownCenter + (width * transform.right) + (depth * transform.forward);
    var downPoint1Outer = downPoint1Inner - (0.1f * transform.up);

    var downPoint2Inner = innerDownCenter - (width * transform.right) - (depth * transform.forward);
    var downPoint2Outer = downPoint2Inner - (0.1f * transform.up);

    var downPoint3Inner = innerDownCenter - (width * transform.right) + (depth * transform.forward);
    var downPoint3Outer = downPoint3Inner - (0.1f * transform.up);

    var downPoint4Inner = innerDownCenter + (width * transform.right) - (depth * transform.forward);
    var downPoint4Outer = downPoint4Inner - (0.1f * transform.up);

    var downCenter = testLine(rigidBody.worldCenterOfMass, rigidBody.worldCenterOfMass - ((height + 0.2f) * transform.up));

    var downPoint1 = testLine(downPoint1Inner, downPoint1Outer);
    var downPoint2 = testLine(downPoint2Inner, downPoint2Outer);
    var downPoint3 = testLine(downPoint3Inner, downPoint3Outer);
    var downPoint4 = testLine(downPoint4Inner, downPoint4Outer);

    var down = downCenter || downPoint1 || downPoint2 || downPoint3 || downPoint4;
    if (down && !isGrounded)
    {
      isGrounded = true;
      jumpCount = defaultJumpCount;
    }
    if (!down && isGrounded)
    {
      isGrounded = false;
    }
    if (!isWall && bottom && center)
    {
      isWall = true;
    }
    if (isWall && !bottom && !center)
    {
      isWall = false;
    }



    var movementSpeed = runSpeed;

    //if (Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(1))
    //{
    //  movementSpeed = runSpeed;
    //}
    if (!isGrounded)
    {
      movementSpeed = airSpeed;
    }
    if (Input.GetKey(KeyCode.LeftShift))
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
          rigidBody.AddForce(Quaternion.AngleAxis(-45, transform.up) * new Vector3(0, jumpSpeed, 0));
        }
        else
        {
          rigidBody.AddForce(new Vector3(0, jumpSpeed, 0));
        }
      }
    }

    float horizontal = Input.GetAxis("Horizontal") * movementSpeed;// * Time.deltaTime;
    float vertical = Input.GetAxis("Vertical") * movementSpeed;// * Time.deltaTime;


    if (horizontal != 0)
    {

      //Debug.Log(string.Format("horri {0}", horizontal));
      Vector3 v3SideForce = horizontal * transform.right;
      rigidBody.transform.position += v3SideForce;
    }
    if (horizontal > 0)
    {
      PlayerState = PlayerAnimationState.StepLeft;
    }
    else
    {
      PlayerState = PlayerAnimationState.StepRight;

    }
    if (vertical != 0)
    {

      Vector3 v3Force = vertical * transform.forward;

      if (!center && !bottom)
      {
        transform.position += v3Force;
      }
      else
      {
        Debug.Log("unable to go forward");
      }
    }


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
