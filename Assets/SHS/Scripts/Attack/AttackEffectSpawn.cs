using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DiceSurvivor.SHS
{
    public class AttackEffectSpawn : MonoBehaviour
    {
        #region Variables
        //참조
        private Animator animator;
        private ParticleSystem effect;

        public GameObject weaponSocket;             //Weapon 위치
        public ParticleSystem attackEffect;         //Spawn할 AttackEffect
        public Transform effectSpawnTransform;      //Spawn할 위치

        [SerializeField]
        private float destroyTime = 5f;             //effect 제거할 시간
        [SerializeField]
        private WeaponType currentWeapon;           //Weapon Type
        
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

        private void LateUpdate()
        {
            Vector3 pos = this.transform.position;

            switch (currentWeapon)
            {
                case WeaponType.Hammer:
                    break;
                case WeaponType.GreatSword:
                    break;
                case WeaponType.Scythe:
                    break;
                case WeaponType.Whip:
                    break;
                case WeaponType.Staff:
                case WeaponType.Spear:
                    pos.x = weaponSocket.transform.position.x;
                    destroyTime = 0.5f;
                    break;
            }

            this.transform.position = pos;
        }
        #endregion

        #region Custom Methods
        public void HammerEffectSpawn()
        {
            effect = Instantiate(attackEffect, effectSpawnTransform.position, effectSpawnTransform.rotation);
            Destroy(effect.gameObject, destroyTime);

        }
        #endregion
    }

}
