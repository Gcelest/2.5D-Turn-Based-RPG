using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;
    [SerializeField] private int minStepsToEncounter;
    [SerializeField] private int maxStepsToEncounter;
    [SerializeField] private float jumpForce = 5f; // Adjust as needed
    [SerializeField] private float jumpingSpeed;
    [SerializeField] private float fallingSpeed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstructionMask;
    private bool facingRight = true;
    private bool isGrounded;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;
    private bool movingInGrass;
    private float stepTimer;
    private int stepsToEncounter;
    private PartyManager partyManager; 


    private const string IS_JUMP_PARAM = "Jump";
    private const string IS_WALK_PARAM = "Move";
    private const string BATTLE_SCENE = "BattleScene";
    private const float TIME_PER_STEP = 0.5f;



    private void Awake()
    {
        playerControls = new PlayerControls();
        CalculateStepsToNextEncounter();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();  // 👈 Important!
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        if(partyManager.GetPosition() != Vector3.zero)//if position saved
        {
            transform.position = partyManager.GetPosition();//move player
        }
    }
    // Update is called once per frame
    void Update()
    {
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;

        //Debug.Log(x + "" + z);

        movement = new Vector3(x, 0, z).normalized;

        anim.SetBool(IS_WALK_PARAM, movement != Vector3.zero);

        if (x < 0 && facingRight)
        {
            StartCoroutine(RotateCharacter(180));
            facingRight = false;
        }
        else if (x > 0 && !facingRight)
        {
            StartCoroutine(RotateCharacter(0));
            facingRight = true;
        }

        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        anim.SetBool("isGrounded", isGrounded);

        if (playerControls.Player.Jump.triggered && isGrounded)
        {
            anim.SetTrigger(IS_JUMP_PARAM);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z); // Instant, powerful jump
        }
    }

    //made it look like a paper mario where they flip when looking left and right
    private IEnumerator RotateCharacter(float targetYRotation)
    {
        float duration = 0.2f; // Adjust rotation speed
        float elapsedTime = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }
    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        anim.SetBool("isGrounded", isGrounded);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 1, grassLayer);
        movingInGrass = colliders.Length != 0 && movement != Vector3.zero;


         if (!isGrounded)
    {
        if (rb.linearVelocity.y > 0) // Going up
        {
            rb.AddForce(Vector3.down * jumpingSpeed, ForceMode.Acceleration); // Stronger gravity while going up
        }
        else if (rb.linearVelocity.y < 0) // Falling
        {
            rb.AddForce(Vector3.down * fallingSpeed, ForceMode.Acceleration); // Even stronger gravity while falling
        }
    }

        if (movingInGrass)
        {
            stepTimer += Time.fixedDeltaTime;

            if (stepTimer > TIME_PER_STEP)
            {
                stepsInGrass++;
                stepTimer = 0;

                if (stepsInGrass >= stepsToEncounter)
                {
                    partyManager.SetPosition(transform.position);
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
            }
        }

    }

    private void CalculateStepsToNextEncounter()
    {
        stepsToEncounter = Random.Range(minStepsToEncounter, maxStepsToEncounter);
    }
}
