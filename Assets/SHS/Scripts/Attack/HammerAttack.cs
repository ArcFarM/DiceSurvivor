using System.Collections;
using UnityEngine;

public class HammerAttack : MonoBehaviour
{
    #region Variables
    public ParticleSystem hammerEffect;
    public Transform hammer;

    [SerializeField]
    private bool isGround = false;
    #endregion

    #region Properties
    #endregion

    #region Unity Event Methods
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ground")
        {
            Debug.Log("Hello");
            isGround = true;
        }
    }
    private void Update()
    {
        if (isGround)
        {
            ParticleSystem effect = Instantiate(hammerEffect, hammer.position, Quaternion.identity);
            Destroy(effect.gameObject, 1f);
            isGround = false;
        }
    }
    #endregion

    #region Custom Methods
    #endregion
}
