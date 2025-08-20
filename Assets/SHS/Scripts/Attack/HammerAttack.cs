using UnityEngine;

namespace DiceSurvivor.Attack
{
    public class HammerAttack : WeaponAttackBase
    {
        #region Variables
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        protected override void Awake()
        {
            base.Awake();
        }
        private void Start()
        {
            Transform parentTransform = this.transform.parent;
            if (Weapon.range == 6)
            {
                parentTransform.localScale = new Vector3(parentTransform.localScale.x + 30, parentTransform.localScale.y + 30, parentTransform.localScale.z + 30);
            }
            else if (Weapon.range == 7)
            {
                parentTransform.localScale = new Vector3(parentTransform.localScale.x + 30 * 2, parentTransform.localScale.y + 30 * 2, parentTransform.localScale.z + 30 * 2);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                Debug.Log($"적 피해 받음 : {Weapon.damage}");
            }
        }
        #endregion

        #region Custom Methods
        #endregion
    }

}
