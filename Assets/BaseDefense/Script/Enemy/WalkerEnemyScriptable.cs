using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable/Enemy/Walker", order = 3)]
public class WalkerEnemyScriptable : ScriptableObject
{
    [Range(0.1f,10000)] public float MaxHp = 100;
    [Range(0.1f,10000)] public float Damage = 33;
    [Range(1, 50)] public float MoveSpeed = 5;
    public Animator AnimatorForImage;
}
