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

    private void Start() {
        m_WaveHeat = MainGameManager.GetInstance().GetHeat() / TotalWave ;
        // filter out enemy type that need too much heat
        m_VaildEnemies = new List<EnemyScriptable>(m_AllEnemyTypes.Where(x=>x.TargetTotalHeatForSpawn<= MainGameManager.GetInstance().GetHeat()).ToList());
        BaseDefenseManager.GetInstance().m_UpdateAction += EnemySpawnUpdate;
    }

    public void EnemySpawnUpdate() {
        if(m_WaveCount>TotalWave ){
            // player win
            //return;
        }

        if(m_VaildEnemies.Count<=0){
            // no vaild enemy
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
            float timeDelay = Random.Range(0f,15f);
            StartCoroutine( SpawnEnemy(timeDelay , targetEnemyScriptable) );

            // gain heat 
            totalHeatOfThisWave += targetEnemyScriptable.HeatGainForSpawn;

        }

        m_TimePassed = 25;
        m_WaveCount ++;
    }

    public IEnumerator SpawnEnemy(float timeDelay, EnemyScriptable enemyScriptable){
        yield return new WaitForSeconds(timeDelay);
        var newEnemy = Instantiate(enemyScriptable.Prefab);
        newEnemy.transform.SetParent(m_EnemyParent);
        newEnemy.transform.localPosition = new Vector3(Random.Range(-5,5),20,0);
        newEnemy.GetComponent<EnemyController>().SetScriptable(enemyScriptable);
    }

}
