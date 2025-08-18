using DiceSurvivor.Weapon;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace DiceSurvivor.Attack
{
    public class WeaponAttack : WeaponAttackBase
    {
        #region Variables
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        protected override void Awake()
        {
            base.Awake();       //상속 클래스에 Awake 실행(Weapon값을 할당하기 위해)
        }
        private  void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Enemy")
            {
                Debug.Log($"적 피해 받음 : {Weapon.damage}");
            }
        }
        #endregion

        #region Custom Methods
        #endregion
    }

}
