using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnSystem : MonoBehaviour
{
    [SerializeField] private GameObject m_EnemyPrefab;
    [SerializeField] private Transform m_EnemyParent;
    private float m_TimePass = 0;

    private void Update() {
        if( m_TimePass > 10 ){
            var newEnemy = Instantiate(m_EnemyPrefab);
            newEnemy.transform.SetParent(m_EnemyParent);
            newEnemy.transform.localPosition = new Vector3(Random.Range(-5,5),20,0);

            m_TimePass = 0;
        }else{
            m_TimePass += Time.deltaTime;
        }
        
    }

}
