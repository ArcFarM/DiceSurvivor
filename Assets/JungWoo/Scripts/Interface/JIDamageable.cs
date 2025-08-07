
namespace DiceSurvivor.JWDamage
{
    /// <summary>
    /// 데미지를 받을 수 있는 객체들이 구현해야 하는 인터페이스
    /// 적, 플레이어, 파괴 가능한 오브젝트 등에서 사용
    /// </summary>
    public interface JIDamageable
    {
        /// <summary>
        /// 데미지를 받는 메서드
        /// </summary>
        /// <param name="damage">받을 데미지 양</param>
        void TakeDamage(float damage);

        /// <summary>
        /// 현재 체력을 반환
        /// </summary>
        /// <returns>현재 체력</returns>
        float GetCurrentHealth();

        /// <summary>
        /// 최대 체력을 반환
        /// </summary>
        /// <returns>최대 체력</returns>
        float GetMaxHealth();

        /// <summary>
        /// 죽었는지 확인
        /// </summary>
        /// <returns>죽었으면 true</returns>
        bool IsDead();
    }
}