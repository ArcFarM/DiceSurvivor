using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

namespace DiceSurvivor.WeaponDataSystem
{
    /// <summary>
    /// Weapon의 속성을 Json파일에서 가져와 읽는 script
    /// 
    /// </summary>
    public class WeaponDataManager : MonoBehaviour // MonoBehaviour를 상속받아 Unity 게임오브젝트에 부착 가능
    {
        public static WeaponDataManager Instance { get; private set; } // 싱글톤 패턴 구현을 위한 정적 인스턴스

        [Header("설정")] // Inspector에서 헤더로 표시
        public string jsonFileName = "area_weapons.json"; // JSON 파일 이름

        private WeaponDatabase weaponDatabase; // 모든 무기 데이터를 저장하는 데이터베이스

        void Awake() // Unity 생명주기 함수, Start보다 먼저 실행됨
        {
            if (Instance == null) // 인스턴스가 없으면
            {
                Instance = this; // 현재 객체를 인스턴스로 설정
                DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 이 객체를 파괴하지 않음
                LoadWeaponData(); // 무기 데이터 로드
            }
            else // 이미 인스턴스가 존재하면
            {
                Destroy(gameObject); // 중복 객체 파괴 (싱글톤 패턴 유지)
            }
        }

        void LoadWeaponData() // JSON 파일에서 무기 데이터를 로드하는 함수
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName); // 파일 경로 생성

            if (File.Exists(filePath)) // 파일이 존재하는 경우
            {
                string jsonString = File.ReadAllText(filePath); // 파일 내용을 문자열로 읽기
                weaponDatabase = JsonUtility.FromJson<WeaponDatabase>(jsonString); // JSON을 객체로 변환

                Debug.Log("무기 데이터 로드 완료"); // 로드 완료 로그 출력
            }
            else // 파일이 존재하지 않는 경우
            {
                Debug.LogError($"무기 데이터 파일을 찾을 수 없습니다: {filePath}"); // 에러 로그 출력
                CreateSampleWeaponData(); // 샘플 데이터 생성
            }
        }

        /// <summary>
        /// 특정 무기의 특정 레벨 데이터를 가져옵니다
        /// </summary>
        /// <param name="weaponName">무기 이름 (예: "Asteroid")</param>
        /// <param name="level">레벨 (1부터 시작)</param>
        /// <returns>해당 레벨의 무기 데이터</returns>
        public WeaponData GetWeaponData(string weaponName, int level) // 무기 데이터를 가져오는 공개 함수
        {
            if (weaponDatabase == null) // 데이터베이스가 로드되지 않았으면
            {
                Debug.LogError("무기 데이터베이스가 로드되지 않았습니다."); // 에러 로그 출력
                return null; // null 반환
            }

            List<WeaponData> weaponLevels = GetWeaponLevels(weaponName); // 해당 무기의 레벨 데이터 가져오기
            if (weaponLevels == null) // 데이터가 없으면
            {
                Debug.LogWarning($"무기 '{weaponName}'을 찾을 수 없습니다."); // 경고 로그 출력
                return null; // null 반환
            }

            int index = level - 1; // 레벨 1 = 인덱스 0으로 변환
            if (index < 0 || index >= weaponLevels.Count) // 인덱스가 범위를 벗어나면
            {
                Debug.LogWarning($"무기 '{weaponName}'의 레벨 {level}이 존재하지 않습니다. (최대 레벨: {weaponLevels.Count})"); // 경고 로그 출력
                return null; // null 반환
            }

            return weaponLevels[index]; // 해당 레벨의 데이터 반환
        }

        /// <summary>
        /// 특정 무기의 최대 레벨을 가져옵니다
        /// </summary>
        public int GetMaxLevel(string weaponName) // 무기의 최대 레벨을 반환하는 함수
        {
            List<WeaponData> weaponLevels = GetWeaponLevels(weaponName); // 해당 무기의 레벨 데이터 가져오기
            return weaponLevels?.Count ?? 0; // 레벨 개수 반환 (null이면 0)
        }

        /// <summary>
        /// 특정 무기의 모든 레벨 데이터를 가져옵니다
        /// </summary>
        public List<WeaponData> GetAllLevelsData(string weaponName) // 무기의 모든 레벨 데이터를 반환하는 함수
        {
            List<WeaponData> weaponLevels = GetWeaponLevels(weaponName); // 해당 무기의 레벨 데이터 가져오기
            return weaponLevels ?? new List<WeaponData>(); // 레벨 리스트 반환 (null이면 빈 리스트)
        }

        /// <summary>
        /// 사용 가능한 모든 무기 이름 목록을 가져옵니다
        /// </summary>
        public List<string> GetAllWeaponNames() // 모든 무기 이름을 반환하는 함수
        {
            List<string> weaponNames = new List<string>(); // 무기 이름을 저장할 리스트 생성

            if (weaponDatabase == null) return weaponNames; // 데이터베이스가 없으면 빈 리스트 반환

            // 리플렉션을 사용해서 모든 무기 필드 이름을 가져옵니다
            var fields = typeof(WeaponDatabase).GetFields(); // WeaponDatabase 클래스의 모든 필드 가져오기
            foreach (var field in fields) // 각 필드에 대해 반복
            {
                if (field.FieldType == typeof(List<WeaponData>)) // 필드 타입이 List<WeaponData>인 경우
                {
                    List<WeaponData> weaponData = (List<WeaponData>)field.GetValue(weaponDatabase); // 필드 값 가져오기
                    if (weaponData != null && weaponData.Count > 0) // 유효한 데이터가 있으면
                    {
                        weaponNames.Add(field.Name); // 필드 이름을 리스트에 추가
                    }
                }
            }

            return weaponNames; // 무기 이름 리스트 반환
        }

        /// <summary>
        /// 무기 이름으로 List<WeaponData>를 가져오는 내부 함수
        /// </summary>
        private List<WeaponData> GetWeaponLevels(string weaponName) // 무기 이름으로 레벨 데이터를 가져오는 private 함수
        {
            if (weaponDatabase == null) return null; // 데이터베이스가 없으면 null 반환

            switch (weaponName.ToLower()) // 무기 이름을 소문자로 변환해서 비교
            {
                case "killingaura": return weaponDatabase.KillingAura; // KillingAura 무기 데이터 반환
                case "icicle": return weaponDatabase.Icicle; // Icicle 무기 데이터 반환
                case "lightningstaff": return weaponDatabase.LightningStaff; // LightningStaff 무기 데이터 반환
                case "asteroid": return weaponDatabase.Asteroid; // Asteroid 무기 데이터 반환
                                                                 // 새로운 무기를 추가할 때 여기에 case를 추가하세요
                default: return null; // 일치하는 무기가 없으면 null 반환
            }
        }

        /// <summary>
        /// 레벨업 가능 여부를 확인합니다
        /// </summary>
        public bool CanLevelUp(string weaponName, int currentLevel) // 레벨업 가능 여부를 확인하는 함수
        {
            int maxLevel = GetMaxLevel(weaponName); // 무기의 최대 레벨 가져오기
            return currentLevel < maxLevel; // 현재 레벨이 최대 레벨보다 작으면 true
        }

        // 샘플 데이터 생성
        void CreateSampleWeaponData() // 샘플 데이터를 생성하는 함수
        {
            WeaponDatabase sampleDatabase = new WeaponDatabase(); // 새로운 데이터베이스 객체 생성

            // Asteroid 샘플 데이터
            sampleDatabase.Asteroid = new List<WeaponData>(); // Asteroid 레벨 리스트 초기화

            for (int i = 0; i < 8; i++) // 8개 레벨에 대해 반복
            {
                WeaponData levelData = new WeaponData() // 새로운 무기 데이터 생성
                {
                    weaponName = "Asteroid", // 무기 이름 설정
                    description = "운석을 소환하여 적을 공격합니다", // 무기 설명 설정
                    weaponType = WeaponType.Projectile, // 무기 타입을 투사체로 설정
                    damage = 10 + (i * 2), // 레벨에 따라 데미지 증가
                    radius = 0.5f, // 반경 고정
                    cooldown = 8f - (i * 0.1f), // 레벨에 따라 쿨다운 감소
                    projectileSpeed = 1f + (i > 6 ? 0.5f : 0f), // 레벨 7 이상에서 속도 증가
                    projectileCount = 2 + (i / 2), // 2레벨마다 투사체 개수 증가
                    duration = 3f + (i * 0.25f) // 레벨에 따라 지속시간 증가
                };

                sampleDatabase.Asteroid.Add(levelData); // 레벨 데이터를 리스트에 추가
            }

            // KillingAura 샘플 데이터
            sampleDatabase.KillingAura = new List<WeaponData>(); // KillingAura 레벨 리스트 초기화

            WeaponData killingAuraLevel1 = new WeaponData() // KillingAura 레벨 1 데이터 생성
            {
                weaponName = "KillingAura", // 무기 이름 설정
                description = "주변에 살상의 오라를 방출합니다", // 무기 설명 설정
                weaponType = WeaponType.DonutArea, // 무기 타입을 도넛형 범위로 설정
                damage = 0f, // 기본 데미지 (DoT 데미지 사용)
                radius = 1f, // 반경 설정
                cooldown = 2f, // 쿨다운 설정
                projectileSpeed = 0f, // 투사체 속도 (사용하지 않음)
                projectileCount = 0, // 투사체 개수 (사용하지 않음)
                duration = 0.5f // 지속시간 설정
            };

            sampleDatabase.KillingAura.Add(killingAuraLevel1); // KillingAura 데이터를 리스트에 추가

            // JSON 파일로 저장
            string jsonString = JsonUtility.ToJson(sampleDatabase, true); // 객체를 JSON 문자열로 변환 (들여쓰기 포함)
            string savePath = Path.Combine(Application.streamingAssetsPath, jsonFileName); // 저장 경로 생성

            if (!Directory.Exists(Application.streamingAssetsPath)) // StreamingAssets 폴더가 없으면
            {
                Directory.CreateDirectory(Application.streamingAssetsPath); // 폴더 생성
            }

            File.WriteAllText(savePath, jsonString); // JSON 문자열을 파일로 저장
            Debug.Log($"샘플 무기 데이터가 생성되었습니다: {savePath}"); // 생성 완료 로그 출력

            weaponDatabase = sampleDatabase; // 생성한 샘플 데이터를 현재 데이터베이스로 설정
        }
    }
}
