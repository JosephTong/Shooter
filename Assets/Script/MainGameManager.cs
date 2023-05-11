using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainGameNameSpace;



namespace MainGameNameSpace
{
    [System.Serializable]
    public class ResourcesRecord
    {
        public float Raw = 0;
        public float Scrap = 0;
        public float Chem = 0;
        public float Electronic = 0;
        public int Bot = 0;
        public float Heat = 0;

        public void Change(ResourcesRecord record){
            Raw += record.Raw;
            Scrap += record.Scrap;
            Chem += record.Chem;
            Electronic += record.Electronic;
            Bot += record.Bot;
            Heat += record.Heat;
        }
    }
}

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager m_Instance = null;
    private List<AudioSource> m_AllAudioSource = new List<AudioSource>();

    public float m_Volume = 0.75f;
    public float m_AimSensitivity = 0.5f;    
    
    private const string TotalHeatHeader = " - Strong enemy will not spawn if Total heat is lower than the requirement\n"+
        " - High cap at 1000 \n"+
        " - Low cap at 15 \n"+
        " - 25 seconds per wave \n"+ // EnemySpawnController >> m_TimePassed 
        " - Random select vaild enemy(s) until it reach Wave heat \n" +
        " - All selected enemy will spawn at the first 15 seconds \n"+
        " - Wave heat = TotalHeat / 3 \n" + // EnemySpawnController >> TotalWave
        " - To next day it player clear all 3 waves"; 

    //[Header(TotalHeatHeader)]
    //[SerializeField][Range(15f,1000f)] private float m_TotalHeat = 35;

    private ResourcesRecord m_OwnedResource = new ResourcesRecord();
    //private int m_BotOwned = 10;

    
    public static MainGameManager GetInstance(){
        if(m_Instance==null){
            m_Instance = new GameObject().AddComponent<MainGameManager>();
        }
        return m_Instance;
    }


    private void Awake() {
        if(m_Instance==null){
            m_Instance = this;
        }else{
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        m_OwnedResource = new ResourcesRecord{
            Raw = 5000,
            Scrap = 5000,
            Chem = 5000,
            Electronic = 5000,
            Bot = 10,
            Heat = 35
        };
    }

    public float GetHeat(){
        if(m_OwnedResource.Heat > 1000){
            m_OwnedResource.Heat = 1000;
        }
        if(m_OwnedResource.Heat <15){
            m_OwnedResource.Heat = 15;
        }
        return m_OwnedResource.Heat;
    }

    public void AddNewAudioSource(AudioSource audioSource){
        m_AllAudioSource.Add(audioSource);
        audioSource.volume = m_Volume;
        
        UpdateVolume();
    }

    public int GetOwnedBotCount(){
        return m_OwnedResource.Bot;
    }

    public void UpdateVolume(){
        List<int> toBeRemove = new List<int>();
        for (int i = 0; i < m_AllAudioSource.Count; i++)
        {
            if(m_AllAudioSource[i] != null){
                m_AllAudioSource[i].volume = m_Volume;
            }else{
                toBeRemove.Add(i);
            }
        }
        for (int i = 0; i < toBeRemove.Count; i++)
        {
            m_AllAudioSource.RemoveAt(toBeRemove[i]-i);
        }
    }

    public void GainResources(ResourcesRecord gain){
        m_OwnedResource.Change(gain);
    }

    public ResourcesRecord GetOwnedResources(){
        return m_OwnedResource;
    }

}
