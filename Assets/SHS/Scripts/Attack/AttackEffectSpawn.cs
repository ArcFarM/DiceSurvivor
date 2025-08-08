using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackEffectSpawn : MonoBehaviour
{
    #region Variables
    //참조
    private Animator animator;

    public ParticleSystem attackEffect;         //Spawn할 AttackEffect
    public Transform effectSpawnTransform;      //Spawn할 위치    
    #endregion

    #region Properties
    #endregion

    #region Unity Event Methods
    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("IsAttack");
        }
    }
    #endregion

    #region Custom Methods
    public void HammerEffectSpawn()
    {


        ParticleSystem effect = Instantiate(attackEffect, effectSpawnTransform.position, effectSpawnTransform.rotation);
        Destroy(effect.gameObject, 5f);
    }
    #endregion
}
