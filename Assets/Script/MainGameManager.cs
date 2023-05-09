using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager m_Instance = null;
    private List<AudioSource> m_AllAudioSource = new List<AudioSource>();

    public float m_Volume = 0.75f;
    public float m_AimSensitivity = 0.5f;

    
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

    public void AddNewAudioSource(AudioSource audioSource){
        m_AllAudioSource.Add(audioSource);
        audioSource.volume = m_Volume;
        
        UpdateVolume();
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
