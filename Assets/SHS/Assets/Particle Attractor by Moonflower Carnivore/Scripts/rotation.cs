using UnityEngine;

public class rotation : MonoBehaviour
{


    #region Variables
    public Transform topEnd; // 따라갈 대상
                                       //public float xRotation = 0f;
                                       //public float yRotation = 0f;
                                       //public float zRotation = 0f;

    #endregion

    #region Unity Event Method
    void OnEnable()
    {
        InvokeRepeating("rotate", 0f, 0.0167f); // 약 60fps
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    public void clickOn()
    {
        InvokeRepeating("rotate", 0f, 0.0167f);
    }

    public void clickOff()
    {
        CancelInvoke();
    }

    private void Start()
    {
        this.gameObject.transform.SetParent(topEnd.transform);
    }
    #endregion

}
