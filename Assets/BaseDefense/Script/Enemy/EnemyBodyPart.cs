using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyPart : MonoBehaviour
{
    [SerializeField] private EnemyController m_EnemyController;
    [SerializeField][Range(0.1f,100)] private float m_DamageMod = 1;

    public void OnHit(float damage){
        m_EnemyController.OnDamage(damage*m_DamageMod);
    }
    public float GetDistance(){
        return m_EnemyController.GetDistance();
    }
}
