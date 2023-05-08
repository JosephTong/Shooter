using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager m_Instance = null;

    public float m_Volume = 1;
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

}
