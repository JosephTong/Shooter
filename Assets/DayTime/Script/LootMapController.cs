using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LootMapController : MonoBehaviour
{
    [SerializeField] private List<LootLocationScriptable> m_AllLocation = new List<LootLocationScriptable>();
    [SerializeField] private Transform m_LocationParent;
    [SerializeField] private GameObject m_LocaionPrefab;
    [SerializeField][Range(500f,3000f)] private float m_HeatAreaSize = 600;
    [SerializeField] private RectTransform m_HeatCutOut;
    [SerializeField][Range(1,10)] private int m_BotCount = 3;
    [SerializeField] private TMP_Text m_BotCountText;
    [SerializeField] private LootDetailsPanel m_LootDetailsPanel;



    private void Start() {
        for (int i = 0; i < m_AllLocation.Count; i++)
        {
            var locationIcon = Instantiate(m_LocaionPrefab);
            locationIcon.name = m_AllLocation[i].DisplayName;

            // position
            locationIcon.transform.SetParent(m_LocationParent);
            if(locationIcon.TryGetComponent<RectTransform>(out var rectTransform)){
                rectTransform.sizeDelta = m_AllLocation[i].Size;

                rectTransform.localPosition = new Vector3(
                        m_AllLocation[i].Position.x, 
                        m_AllLocation[i].Position.y,
                        0 
                    );
            }
            
            if(locationIcon.TryGetComponent<Image>(out var image)){
                image.sprite = m_AllLocation[i].Icon;
            }
            var scriptable = m_AllLocation[i];

            if(locationIcon.TryGetComponent<Button>(out var btn)){
                btn.onClick.AddListener(()=>{
                    m_LootDetailsPanel.SetLootDeail(scriptable);
                });
            }
        }
    }

    private void Update() {
        m_HeatCutOut.sizeDelta = Vector2.one * m_HeatAreaSize;
    }
}
