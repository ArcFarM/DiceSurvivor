using System.Collections;
using UnityEngine;

namespace DiceSurvivor.Attack
{
    public class SpearStaffAttack : WeaponAttackBase
    {
        #region Variables
        public GameObject spear;
        #endregion

        #region Properties
        #endregion

        #region Unity Event Methods
        protected override void Awake()
        {
            base.Awake();
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
        public void ExecuteAttack()
        {
            StartCoroutine(SetPositionZCoroutine());
        }

        private IEnumerator SetPositionZCoroutine()
        {
            Vector3 start = spear.transform.localPosition;
            Vector3 end = start + new Vector3(0, 0, Weapon.range);
            float duration = 0.2f;

            float elapsed = 0f;
            while (elapsed < duration * 0.5f)
            {
                spear.transform.localPosition = Vector3.Lerp(start, end, elapsed / (duration* 0.5f));
                elapsed += Time.deltaTime;
                yield return null;
            }

            spear.transform.localPosition = end;
            yield return new WaitForSeconds(0.05f);

            elapsed = 0f;
            while (elapsed < duration * 0.5f)
            {
                spear.transform.localPosition = Vector3.Lerp(end, start, elapsed / (duration * 0.5f));
                elapsed += Time.deltaTime;
                yield return null;
            }

            spear.transform.localPosition = start;
        }
        #endregion
    }

}
