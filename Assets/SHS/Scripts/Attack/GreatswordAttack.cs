using UnityEngine;

namespace DiceSurvivor.Attack
{
    public class GreatswordAttack : WeaponAttackBase
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
            if(parentTransform != null)
            {
                if(Weapon.range == 6)
                {
                    parentTransform.localScale = new Vector3(parentTransform.localScale.x + 80, parentTransform.localScale.y + 80, parentTransform.localScale.z + 80);
                }
                else if(Weapon.range == 7)
                {
                    parentTransform.localScale = new Vector3(parentTransform.localScale.x + 80 * 2, parentTransform.localScale.y + 80 * 2, parentTransform.localScale.z + 80 * 2);
                }
                else if(Weapon.range == 8)
                {
                    parentTransform.localScale = new Vector3(parentTransform.localScale.x + 80 * 3, parentTransform.localScale.y + 80 * 3, parentTransform.localScale.z + 80 * 3);
                }
                else if(Weapon.range == 9)
                {
                    parentTransform.localScale = new Vector3(parentTransform.localScale.x + 80 * 4, parentTransform.localScale.y + 80 * 4, parentTransform.localScale.z + 80 * 4);
                }
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
