using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainGameNameSpace;



namespace MainGameNameSpace
{
    [System.Serializable]
    public class FourResources
    {
        public float Raw=100; 
        public float Scrap=100;
        public float Chem=100;
        public float Electronic=100;
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

    [Header(TotalHeatHeader)]
    [SerializeField][Range(15f,1000f)] private float m_TotalHeat = 35;

    private FourResources m_OwnedResource;
    private int m_BotOwned = 10;

    
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

    public float GetHeat(){
        if(m_TotalHeat > 1000){
            m_TotalHeat = 1000;
        }
        if(m_TotalHeat <15){
            m_TotalHeat = 15;
        }
        return m_TotalHeat;
    }

    public void AddNewAudioSource(AudioSource audioSource){
        m_AllAudioSource.Add(audioSource);
        audioSource.volume = m_Volume;
        
        UpdateVolume();
    }

    public int GetOwnedBotCount(){
        return m_BotOwned;
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

}
