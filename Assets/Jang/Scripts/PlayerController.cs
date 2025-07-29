using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    #region Variables
    //����
    private CharacterController controller;

    //�Է� - �̵�
    private Vector2 inputMove;

    //�̵�
    [SerializeField]
    private float moveSpeed = 10f;

    //�߷�
    private float gravity = -9.81f;
    private Vector3 velocity;
    //�׶��� üũ
    public Transform groundCheck;   //�� �ٴ� ��ġ
    [SerializeField] private float checkRange = 0.2f;    //üũ �ϴ� ���� �ݰ�
    [SerializeField] private LayerMask groundMask;       //�׶��� ���̾� �Ǻ�
    #endregion

    #region Unity Event Method
    private void Start()
    {
        //����
        controller = this.GetComponent<CharacterController>();
    }

    private void Update()
    {
        //���� ������
        bool isGrounded = GroundCheck();
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
        Vector3 moveDir = transform.right * inputMove.x + transform.forward * inputMove.y;

        //�̵�
        controller.Move(moveDir * Time.deltaTime * moveSpeed);

        // �߷� ����
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime); 
    }
    #endregion

    public void OnMove(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
    }
    //�׶��� üũ
    bool GroundCheck()
    {
        return Physics.CheckSphere(groundCheck.position, checkRange, groundMask);
    }
}
