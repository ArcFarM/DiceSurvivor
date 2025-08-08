using UnityEngine;

public class Hammer : MonoBehaviour
{
    #region Variables
    #endregion

    #region Properties
    #endregion

    #region Unity Event Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("데미지를 20 준다");
        }
    }
    #endregion

    #region Custom Methods
    #endregion
}
