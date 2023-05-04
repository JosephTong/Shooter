using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private GameObject m_EnemyPrefab;
    [SerializeField] private Transform m_EnemyParent;


    public void SpawnEnemy(){
        var newEnemy = Instantiate(m_EnemyPrefab);
        newEnemy.transform.SetParent(m_EnemyParent);
        newEnemy.transform.localPosition = new Vector3(Random.Range(-5,5),20,0);
    }

}
