using UnityEngine;

[System.Serializable]
public class Weapon
{
	/// <summary>
	/// Weapon에 id와 name을 설정하는 스크립트
	/// </summary>
	#region Variables
	public int id;				//무기 아이디
	public string name;			//무기 이름
	public float damage;		//무기 데미지
	public float range;			//무기 사거리/범위
	public float coolTime;		//무기 공격 쿨타임
	#endregion
}
