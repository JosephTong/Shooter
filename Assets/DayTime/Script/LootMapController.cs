using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LootLocationControllerNameSpace;

public class LootMapController : MonoBehaviour
{
    [SerializeField] private List<LootLocationScriptable> m_AllLocation = new List<LootLocationScriptable>();
    [SerializeField] private Transform m_LocationParent;
    [SerializeField] private GameObject m_LocaionPrefab;
    [SerializeField][Range(500f,3000f)] private float m_HeatAreaSize = 600;
    [SerializeField] private RectTransform m_HeatCutOut;
    [SerializeField] private LootDetailsPanel m_LootDetailsPanel;




    private void Start() {
        for (int i = 0; i < m_AllLocation.Count; i++)
        {
            var locationIcon = Instantiate(m_LocaionPrefab);
            locationIcon.name = m_AllLocation[i].DisplayName;

            // position
            locationIcon.transform.SetParent(m_LocationParent);

            if(locationIcon.TryGetComponent<LootLocationController>(out var lootLocationController)){

                // random id
                int id = Random.Range(0,999999);
                while (DayTimeManager.GetInstance().IsLocationIdUsed(id))
                {
                    id = Random.Range(0,999999);
                }
                DayTimeManager.GetInstance().StoreLootLocationController(id,lootLocationController);

                var scriptable = m_AllLocation[i];
                var tmp = new LootLocationControllerConfig{
                    Scriptable = scriptable,
                    OnClick = ()=>{m_LootDetailsPanel.SetLootDeail(scriptable,id,
                        DayTimeManager.GetInstance().GetLootLocationController(id).GetLootBotCount());},
                    LootBotCount = 0
                };
                lootLocationController.Init(tmp);

            }
            
        }
    }

    private void Update() {
        m_HeatCutOut.sizeDelta = Vector2.one * m_HeatAreaSize;
    }

}
