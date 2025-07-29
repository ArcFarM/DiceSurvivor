using UnityEngine;

public abstract class EffectSoBase : ScriptableObject
{
	/// <summary>
	/// 무기 각각의 이펙트를 따로 따로 설정하기 위한 Base 스크립트
	/// </summary>
	#region Custom Method
	public abstract void ApplyEffect();
	#endregion
}
