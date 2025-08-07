/*using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiceSurvivor.WeaponDataSystem
{
    /// <summary>
    /// 경험치 획득 및 레벨업 시스템 관리
    /// </summary>
    public class ExperienceManager : MonoBehaviour
    {
        // 싱글톤 패턴
        private static ExperienceManager instance;
        public static ExperienceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ExperienceManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("ExperienceManager");
                        instance = go.AddComponent<ExperienceManager>();
                    }
                }
                return instance;
            }
        }

        [Header("플레이어 레벨 설정")]
        [SerializeField] private int currentPlayerLevel = 1;      // 현재 플레이어 레벨
        [SerializeField] private float currentExperience = 0f;    // 현재 경험치
        [SerializeField] private float experienceToNextLevel;     // 다음 레벨까지 필요한 경험치

        [Header("경험치 설정")]
        [SerializeField] private float baseExperienceRequired = 100f;    // 기본 필요 경험치
        [SerializeField] private float experienceMultiplier = 1.2f;      // 레벨당 경험치 증가 배수
        [SerializeField] private AnimationCurve experienceCurve;         // 경험치 곡선 (선택적)

        [Header("경험치 획득 설정")]
        [SerializeField] private float experiencePickupRange = 3f;       // 경험치 획득 범위
        [SerializeField] private LayerMask experienceOrbLayer;           // 경험치 오브 레이어

        [Header("UI 참조")]
        [SerializeField] private Slider experienceBar;                   // 경험치 바 UI
        [SerializeField] private Text levelText;                         // 레벨 텍스트
        [SerializeField] private Text experienceText;                    // 경험치 텍스트

        [Header("레벨업 보상")]
        [SerializeField] private GameObject levelUpPanel;                 // 레벨업 UI 패널
        [SerializeField] private Transform weaponChoiceContainer;         // 무기 선택 컨테이너
        [SerializeField] private GameObject weaponChoicePrefab;           // 무기 선택 버튼 프리팹

        // 이벤트
        public event Action<int> OnLevelUp;                             // 레벨업 이벤트
        public event Action<float> OnExperienceGained;                  // 경험치 획득 이벤트

        // 플레이어가 보유한 무기들
        private List<MeleeWeapon> playerWeapons = new List<MeleeWeapon>();

        private Transform player;

        private void Awake()
        {
            // 싱글톤 설정
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            // 플레이어 찾기
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Start()
        {
            // 초기 경험치 설정
            CalculateExperienceToNextLevel();
            UpdateUI();

            // 플레이어가 보유한 무기들 찾기
            FindPlayerWeapons();
        }

        private void Update()
        {
            // 주변 경험치 오브 자동 획득
            CollectNearbyExperience();
        }

        /// <summary>
        /// 다음 레벨까지 필요한 경험치 계산
        /// </summary>
        private void CalculateExperienceToNextLevel()
        {
            // 경험치 곡선이 설정되어 있으면 사용
            if (experienceCurve != null && experienceCurve.length > 0)
            {
                experienceToNextLevel = experienceCurve.Evaluate(currentPlayerLevel) * baseExperienceRequired;
            }
            else
            {
                // 기본 공식: 기본값 * (배수 ^ (레벨-1))
                experienceToNextLevel = baseExperienceRequired * Mathf.Pow(experienceMultiplier, currentPlayerLevel - 1);
            }

            // 최소 경험치 보장
            experienceToNextLevel = Mathf.Max(experienceToNextLevel, baseExperienceRequired);
        }

        /// <summary>
        /// 경험치 획득
        /// </summary>
        public void GainExperience(float amount)
        {
            currentExperience += amount;

            // 경험치 획득 이벤트 발생
            OnExperienceGained?.Invoke(amount);

            // 레벨업 체크
            while (currentExperience >= experienceToNextLevel)
            {
                LevelUp();
            }

            UpdateUI();
        }

        /// <summary>
        /// 레벨업 처리
        /// </summary>
        private void LevelUp()
        {
            // 남은 경험치 계산
            currentExperience -= experienceToNextLevel;
            currentPlayerLevel++;

            // 다음 레벨 필요 경험치 재계산
            CalculateExperienceToNextLevel();

            // 레벨업 이벤트 발생
            OnLevelUp?.Invoke(currentPlayerLevel);

            // 레벨업 효과
            ShowLevelUpEffect();

            // 무기 선택 UI 표시
            ShowWeaponUpgradeChoices();

            Debug.Log($"레벨업! 현재 레벨: {currentPlayerLevel}");
        }

        /// <summary>
        /// 무기 업그레이드 선택지 표시
        /// </summary>
        private void ShowWeaponUpgradeChoices()
        {
            // 게임 일시정지
            Time.timeScale = 0f;

            // 레벨업 패널 활성화
            if (levelUpPanel != null)
            {
                levelUpPanel.SetActive(true);

                // 기존 선택지 제거
                foreach (Transform child in weaponChoiceContainer)
                {
                    Destroy(child.gameObject);
                }

                // 랜덤으로 3개의 무기 선택지 생성
                List<MeleeWeapon> availableWeapons = GetUpgradeableWeapons();
                int choiceCount = Mathf.Min(3, availableWeapons.Count);

                for (int i = 0; i < choiceCount; i++)
                {
                    // 랜덤 무기 선택
                    int randomIndex = UnityEngine.Random.Range(0, availableWeapons.Count);
                    MeleeWeapon selectedWeapon = availableWeapons[randomIndex];
                    availableWeapons.RemoveAt(randomIndex);

                    // 선택 버튼 생성
                    CreateWeaponChoiceButton(selectedWeapon);
                }

                // 선택지가 없으면 새 무기 제공
                if (choiceCount == 0)
                {
                    OfferNewWeapons();
                }
            }
        }

        /// <summary>
        /// 업그레이드 가능한 무기 목록 가져오기
        /// </summary>
        private List<MeleeWeapon> GetUpgradeableWeapons()
        {
            List<MeleeWeapon> upgradeableWeapons = new List<MeleeWeapon>();

            foreach (MeleeWeapon weapon in playerWeapons)
            {
                if (weapon.GetCurrentLevel() < 8) // 최대 레벨 8
                {
                    upgradeableWeapons.Add(weapon);
                }
            }

            return upgradeableWeapons;
        }

        /// <summary>
        /// 무기 선택 버튼 생성
        /// </summary>
        private void CreateWeaponChoiceButton(MeleeWeapon weapon)
        {
            if (weaponChoicePrefab == null || weaponChoiceContainer == null) return;

            GameObject choiceButton = Instantiate(weaponChoicePrefab, weaponChoiceContainer);

            // 버튼 텍스트 설정
            Text[] texts = choiceButton.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = $"{weapon.GetWeaponName()} Lv.{weapon.GetCurrentLevel() + 1}";

                // 다음 레벨 설명 가져오기
                var nextLevelStats = UnifiedWeaponDataManager.Instance.GetMeleeWeaponData(
                    weapon.GetWeaponName(),
                    weapon.GetCurrentLevel() + 1
                );

                if (nextLevelStats != null)
                {
                    texts[1].text = nextLevelStats.description;
                }
            }

            // 버튼 클릭 이벤트
            Button button = choiceButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnWeaponSelected(weapon));
            }
        }

        /// <summary>
        /// 새 무기 제공 (모든 무기가 최대 레벨일 때)
        /// </summary>
        private void OfferNewWeapons()
        {
            // 아직 보유하지 않은 무기 목록
            List<string> availableNewWeapons = new List<string>
            {
                "Scythe", "Staff", "Spear", "Greatsword", "Hammer", "Whip"
            };

            // 이미 보유한 무기 제외
            foreach (MeleeWeapon weapon in playerWeapons)
            {
                availableNewWeapons.Remove(weapon.GetWeaponName());
            }

            // 새 무기 선택지 제공
            // 구현은 프로젝트 구조에 따라 추가
        }

        /// <summary>
        /// 무기 선택 완료
        /// </summary>
        private void OnWeaponSelected(MeleeWeapon weapon)
        {
            // 무기 레벨업
            weapon.LevelUp();

            // UI 닫기
            if (levelUpPanel != null)
            {
                levelUpPanel.SetActive(false);
            }

            // 게임 재개
            Time.timeScale = 1f;
        }

        /// <summary>
        /// 주변 경험치 오브 수집
        /// </summary>
        private void CollectNearbyExperience()
        {
            if (player == null) return;

            Collider[] experienceOrbs = Physics.OverlapSphere(player.position, experiencePickupRange, experienceOrbLayer);

            foreach (Collider orb in experienceOrbs)
            {
                ExperienceOrb expOrb = orb.GetComponent<ExperienceOrb>();
                if (expOrb != null)
                {
                    expOrb.Collect(player);
                }
            }
        }

        /// <summary>
        /// 플레이어가 보유한 무기들 찾기
        /// </summary>
        private void FindPlayerWeapons()
        {
            playerWeapons.Clear();

            // 플레이어와 그 자식들에서 무기 컴포넌트 찾기
            if (player != null)
            {
                MeleeWeapon[] weapons = player.GetComponentsInChildren<MeleeWeapon>();
                playerWeapons.AddRange(weapons);
            }
        }

        /// <summary>
        /// 새 무기 추가
        /// </summary>
        public void AddWeapon(MeleeWeapon weapon)
        {
            if (!playerWeapons.Contains(weapon))
            {
                playerWeapons.Add(weapon);
            }
        }

        /// <summary>
        /// 레벨업 효과 표시
        /// </summary>
        private void ShowLevelUpEffect()
        {
            // 레벨업 파티클 효과
            // ParticleManager.Instance?.PlayLevelUpEffect(player.position);

            // 레벨업 사운드
            // AudioManager.Instance?.PlaySound("LevelUp");

            // 화면 플래시 효과
            // UIEffectManager.Instance?.FlashScreen(Color.yellow, 0.3f);
        }

        /// <summary>
        /// UI 업데이트
        /// </summary>
        private void UpdateUI()
        {
            if (experienceBar != null)
            {
                experienceBar.value = currentExperience / experienceToNextLevel;
            }

            if (levelText != null)
            {
                levelText.text = $"Lv.{currentPlayerLevel}";
            }

            if (experienceText != null)
            {
                experienceText.text = $"{Mathf.Floor(currentExperience)}/{Mathf.Floor(experienceToNextLevel)}";
            }
        }

        /// <summary>
        /// 경험치 정보 가져오기
        /// </summary>
        public float GetCurrentExperience() => currentExperience;
        public float GetExperienceToNextLevel() => experienceToNextLevel;
        public int GetPlayerLevel() => currentPlayerLevel;
        public float GetExperienceProgress() => currentExperience / experienceToNextLevel;

        /// <summary>
        /// 디버그용 - 강제 레벨업
        /// </summary>
        [ContextMenu("Force Level Up")]
        public void ForceLevelUp()
        {
            GainExperience(experienceToNextLevel);
        }

        /// <summary>
        /// 디버그용 - 경험치 추가
        /// </summary>
        public void AddExperience(float amount)
        {
            GainExperience(amount);
        }

        private void OnDrawGizmosSelected()
        {
            if (player == null) return;

            // 경험치 획득 범위 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, experiencePickupRange);
        }
    }
}*/