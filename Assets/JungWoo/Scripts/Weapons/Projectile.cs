using UnityEngine;

namespace DiceSurvivor.Weapons
{
    public class Projectile : MonoBehaviour
    {
        public float damage;
        public GameObject owner;

        public float speed = 10f;

        private void Update()
        {
            transform.position += transform.up * speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != owner)
            {
                // 데미지 적용, 피격 처리 등
                Destroy(gameObject);
            }
        }

    }
}