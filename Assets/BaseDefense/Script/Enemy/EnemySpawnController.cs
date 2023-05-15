using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private List<EnemyScriptable> m_AllEnemyTypes = new List<EnemyScriptable>();
    [SerializeField] private Transform m_EnemyParent;

    private float m_WaveHeat = 0;
    private int m_WaveCount = 0;
    private float m_TimePassed = 0;
    private const int TotalWave = 3;
    private List<EnemyScriptable> m_VaildEnemies = new List<EnemyScriptable>();
    private List<GameObject> m_AllSpawnedEnemy = new List<GameObject>();

    private void Start() {
        m_WaveHeat = MainGameManager.GetInstance().GetHeat() / TotalWave ;
        // filter out enemy type that need too much heat
        m_VaildEnemies = new List<EnemyScriptable>(m_AllEnemyTypes.Where(x=>x.TargetTotalHeatForSpawn<= MainGameManager.GetInstance().GetHeat()).ToList());
        BaseDefenseManager.GetInstance().m_UpdateAction += EnemySpawnUpdate;
    }

    public void EnemySpawnUpdate() {
        if(m_WaveCount>TotalWave ){
            for (int i = 0; i < m_AllSpawnedEnemy.Count; i++)
            {
                if(m_AllSpawnedEnemy[i] == null){
                    m_AllSpawnedEnemy.RemoveAt(i);
                    i--;
                }
            }
            if(m_AllSpawnedEnemy.Count<=0){
                // player win
                
            }
            return;
        }

        if(m_VaildEnemies.Count<=0){
            // no vaild enemy to be spawn
            return;
        }

        m_TimePassed -= Time.deltaTime;
        if( m_TimePassed <=0 ){
            Debug.Log($"Wave {m_WaveCount}");
            WaveHandler();
        }
    }
    private void WaveHandler(){
        float totalHeatOfThisWave = 0;
        while (totalHeatOfThisWave<m_WaveHeat)
        {
            // select random enemy
            int randomIndex = Random.Range(0,m_VaildEnemies.Count);
            var targetEnemyScriptable = m_VaildEnemies[randomIndex];  

            // select random spawn time 
            float timeDelay = Random.Range(2f,20f);
            StartCoroutine( SpawnEnemy(timeDelay , targetEnemyScriptable) );

            // gain heat 
            totalHeatOfThisWave += targetEnemyScriptable.HeatGainForSpawn;

        }

        m_TimePassed = 30;
        m_WaveCount ++;
    }

    public IEnumerator SpawnEnemy(float timeDelay, EnemyScriptable enemyScriptable){
        yield return new WaitForSeconds(timeDelay);
        var newEnemy = Instantiate(enemyScriptable.Prefab);
        m_AllSpawnedEnemy.Add(newEnemy);
        newEnemy.transform.SetParent(m_EnemyParent);
        newEnemy.transform.localPosition = new Vector3(Random.Range(-8,5),20,0);
        newEnemy.GetComponent<EnemyController>().SetScriptable(enemyScriptable);
    }

}
