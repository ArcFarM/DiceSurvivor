using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    #region Variables
    //움직이는 속도
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    //중력
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float turnSmoothVelocity;
    private Vector2 inputMove;
    private Camera mainCamera;

    [SerializeField] private float playerHP = 100f;
    [SerializeField] private int coinCount = 0;
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float buffedSpeed = 8f;
    private float currentSpeed;

    private Coroutine buffCoroutine;
    private Coroutine magnetCoroutine;
    #endregion

    #region Unity Event Method

    private void Start()
    {
        //참조
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }


    private void Update()
    {
        GroundCheck();
        Move();
        ApplyGravity();
    }
    #endregion

    #region Custom Method
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
    }

    private void Move()
    {
        Vector3 direction = new Vector3(inputMove.x, 0f, inputMove.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.TryGetComponent(out Box box))
        {
            box.BreakOnTouch();
        }
    }

    // 체력 회복
    public void Heal(float amount)
    {
        playerHP += amount;
        playerHP = Mathf.Min(playerHP, 100f); // 최대 체력 제한
        Debug.Log($"HP 회복: {amount} → 현재 HP: {playerHP}");
    }

    // 코인 추가
    public void AddCoins(int amount)
    {
        coinCount += amount;
        Debug.Log($"코인 획득: +{amount} → 총 코인: {coinCount}");
    }

    // 이동속도 버프
    public void ApplySpeedBuff(float multiplier, float duration)
    {
        if (buffCoroutine != null)
            StopCoroutine(buffCoroutine);

        buffCoroutine = StartCoroutine(SpeedBuffRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBuffRoutine(float multiplier, float duration)
    {
        currentSpeed = moveSpeed;
        moveSpeed *= multiplier;
        Debug.Log($"이동속도 버프 시작: {moveSpeed}");

        yield return new WaitForSeconds(duration);

        moveSpeed = currentSpeed;
        Debug.Log("이동속도 버프 종료");
    }

    // 자석 효과
    public void ActivateMagnet(float duration)
    {
        if (magnetCoroutine != null)
            StopCoroutine(magnetCoroutine);

        magnetCoroutine = StartCoroutine(MagnetRoutine(duration));
    }

    private IEnumerator MagnetRoutine(float duration)
    {
        Debug.Log("자석 효과 시작");
        // 여기서 Magnet 범위 내 코인 끌어당기기 구현 가능
        yield return new WaitForSeconds(duration);
        Debug.Log("자석 효과 종료");
    }
    #endregion

}