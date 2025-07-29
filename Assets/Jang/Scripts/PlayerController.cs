using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    #region Variables
    //참조
    private CharacterController controller;

    //입력 - 이동
    private Vector2 inputMove;

    //이동
    [SerializeField]
    private float moveSpeed = 10f;

    //중력
    private float gravity = -9.81f;
    private Vector3 velocity;
    //그라운드 체크
    public Transform groundCheck;   //발 바닥 위치
    [SerializeField] private float checkRange = 0.2f;    //체크 하는 구의 반경
    [SerializeField] private LayerMask groundMask;       //그라운드 레이어 판별
    #endregion

    #region Unity Event Method
    private void Start()
    {
        //참조
        controller = this.GetComponent<CharacterController>();
    }

    private void Update()
    {
        //땅에 있으면
        bool isGrounded = GroundCheck();
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
        Vector3 moveDir = transform.right * inputMove.x + transform.forward * inputMove.y;

        //이동
        controller.Move(moveDir * Time.deltaTime * moveSpeed);

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime); 
    }
    #endregion

    public void OnMove(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
    }
    //그라운드 체크
    bool GroundCheck()
    {
        return Physics.CheckSphere(groundCheck.position, checkRange, groundMask);
    }
}
