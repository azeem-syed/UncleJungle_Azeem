using System.Collections;
using UnityEngine;

public class GorillaControl : MonoBehaviour
{
    public const float jumpPowerGorilla = 200f;
    public const float lanePosition = 1.7f;
    public const float moveSpeed = 3.0f;
    public const float newSpawnDistance = 10f;
    public const float newSpawnHeight = 20f;

    public const int minLaneNumber = -2;
    public const int maxLaneNumber = 2;
    new Animator animation = null;

    public GameObject gorillaBody = null;
    public Transform playerTransform = null;

    float enemyVelocity = 12.0f;
    float calculatedLane = 0.0f;

    int laneNumber = 0;

    Vector3 lookAtRotation = Vector3.zero;
    Vector3 chaseVector = Vector3.zero;

    enum GorillaAction
    {
        AvoidRight,
        AvoidLeft,
        Jump,
        PushThrough,
    }

    GorillaAction currAction;
    bool jumpInProgress = false;
    // Start is called before the first frame update
    void Start()
    {
        currAction = GorillaAction.PushThrough;
        animation = gorillaBody.GetComponent<Animator>();
        chaseVector = playerTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpInProgress == false && currAction == GorillaAction.Jump)
        {
            jumpInProgress = true;
            animation.SetBool("Jumping", jumpInProgress);
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpPowerGorilla, ForceMode.Impulse);
            StartCoroutine("DisableGorillaJump");
        }

        animation.SetBool("Jumping", jumpInProgress);

        if (currAction == GorillaAction.AvoidLeft)
        {
            if (laneNumber > minLaneNumber)
                laneNumber--;
        }

        if (currAction == GorillaAction.AvoidRight)
        {
            if (laneNumber < maxLaneNumber)
                laneNumber++;
        }

        chaseVector = new Vector3 (playerTransform.localPosition.x, chaseVector.y, playerTransform.localPosition.z);
        gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, chaseVector, enemyVelocity * Time.deltaTime);
        gameObject.transform.LookAt(gameObject.transform.localPosition);

        if (gameObject.transform.position.y < -10f)
        {
            gameObject.transform.position = new Vector3(0.0f, newSpawnHeight, playerTransform.position.z - newSpawnDistance);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "HardObstacle")
        {
            Vector3 direction = other.transform.position - transform.position;
            if (direction.y >= 0.1f)
            {
                currAction = GorillaAction.Jump;
            }
            else if (direction.y <= -0.1f)
            {
                currAction = GorillaAction.PushThrough;
            }
            else if (direction.x >= 0f)
            {
                currAction = GorillaAction.AvoidLeft;
            }
            else if (direction.x < 0f)
            {
                currAction = GorillaAction.AvoidRight;
            }
            else
            {
                currAction = GorillaAction.Jump;
            }
        }
    }

    IEnumerator DisableGorillaJump()
    {
        yield return new WaitForSeconds(1.84f);
        currAction = GorillaAction.PushThrough;
        jumpInProgress = false;
    }
}