using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using BaseDefenseNameSpace;

namespace BaseDefenseNameSpace
{
    public class SpawnUIObjectForReloadPhaseConfig
    {
        public GameObject Prefab;
        public Vector2 Position;
    }

    public class GunReloadControllerConfig
    {
        public GunScriptable GunScriptable;
        //public Action CancelReload;
        public Action<int> GainAmmo;
        public Action SetAmmoToFull;
        public Action SetAmmoToZero;
        public Func<bool> IsFullClipAmmo;
    }

    public enum BaseDefenseStage{
        Shoot,
        SwitchWeapon,
        Reload,
        Result
    }
}

public class BaseDefenseManager : MonoBehaviour
{
    public static BaseDefenseManager m_Instance = null;
    [SerializeField] private EnemySpawnController m_EnemySpawnController;
    [SerializeField] private GunController m_GunController;
    [SerializeField] private GunReloadController m_ReloadController;
    [SerializeField] private SwitchWeaponController m_SwitchWeaponController;
    [SerializeField] private BaseDefenseResultPanel m_BaseDefenseResultPanel;
    [SerializeField] private BaseDefenseTimmyPanel m_BaseDefenseTimmyPanel;
    [SerializeField] private Button m_OptionBtn;
    [SerializeField] private GameObject m_OptionPanel;

    
    [SerializeField] private GameObject m_ReloadControllerPanel;


    [Header("Enemy Hp Bars")]
    [SerializeField] private Transform m_EnemyHpBarParent;
    public Transform EnemyHpBarParent { get { return m_EnemyHpBarParent; } }

    [Header("Wall")]
    [SerializeField] private HpBar m_WallHpBar;
    private float m_TotalWallHpBarStayTime = 0;




    
    #region UpdateAction
    public Action m_ShootUpdatreAction = null;
    public Action m_SwitchWeaponUpdateAction = null;
    public Action m_ReloadUpdateAction = null;
    public Action m_UpdateAction = null;
    #endregion

    #region Change Game Stage From
    public Action m_ChangeFromShootAction = null;
    public Action m_ChangeFromSwitchWeaponAction = null;
    public Action m_ChangeFromReloadAction = null;
    #endregion

    #region Change Game Stage To
    public Action m_ChangeToShootAction = null;
    public Action m_ChangeToSwitchWeaponAction = null;
    public Action m_ChangeToReloadAction = null;
    #endregion


    private BaseDefenseStage m_GameStage = BaseDefenseStage.Shoot;
    public BaseDefenseStage GameStage {get { return m_GameStage; }}


    private void Awake() {
        if(m_Instance==null){
            m_Instance = this;
        }else{
            Destroy(this);
        }
    }

    public static BaseDefenseManager GetInstance(){
        if(m_Instance==null){
            m_Instance = new GameObject().AddComponent<BaseDefenseManager>();
        }
        return m_Instance;
    }


    private void Start() {
        

        m_ChangeFromReloadAction += CloseReloadPanel;
        m_OptionBtn.onClick.AddListener(()=>{
            m_OptionPanel.SetActive(true);
        });
        m_WallHpBar.m_HpBarFiller.fillAmount = MainGameManager.GetInstance().GetWallCurHp() / MainGameManager.GetInstance().GetWallMaxHp();
    }

    private void Update() {
        switch (m_GameStage)
        {
            case BaseDefenseStage.Shoot:
                m_ShootUpdatreAction?.Invoke();
            break;
            case BaseDefenseStage.SwitchWeapon:
                m_SwitchWeaponUpdateAction?.Invoke();
            break;
            case BaseDefenseStage.Reload:
                m_ReloadUpdateAction?.Invoke();
            break;
            default:
            break;
        }
        m_UpdateAction?.Invoke();
    }
    
    private void FixedUpdate()
    {
        // wall hp bar stay time
        if(m_TotalWallHpBarStayTime>0){
            m_TotalWallHpBarStayTime -= Time.deltaTime;
        }else{
            m_WallHpBar.m_CanvasGroup.alpha = 0;
        }
    }

    public void SetTimmyAssitancePanel(){
        m_BaseDefenseTimmyPanel.Init();
    }

    public void ChangeGameStage(BaseDefenseStage newStage){
        switch (m_GameStage)
        {
            case BaseDefenseStage.Shoot:
                m_ChangeFromShootAction?.Invoke();
            break;
            case BaseDefenseStage.SwitchWeapon:
                m_ChangeFromSwitchWeaponAction?.Invoke();
            break;
            case BaseDefenseStage.Reload:
                m_ChangeFromReloadAction?.Invoke();
            break;
            default:
            break;
        }

        switch (newStage)
        {
            case BaseDefenseStage.Shoot:
                m_ChangeToShootAction?.Invoke();
            break;
            case BaseDefenseStage.SwitchWeapon:
                m_ChangeToSwitchWeaponAction?.Invoke();
            break;
            case BaseDefenseStage.Reload:
                m_ChangeToReloadAction?.Invoke();
            break;
            default:
            break;
        }

        m_GameStage = newStage;
    }

    public void GameOver(bool isLose = false){
        ChangeGameStage(BaseDefenseStage.Result);
        m_BaseDefenseResultPanel.ShowResult(isLose);
    }

    public void OnWallHit(float damage){
        if(m_GameStage == BaseDefenseStage.Result){
            // game over already
            return;
        }
        m_WallHpBar.m_CanvasGroup.alpha = 1;
        MainGameManager.GetInstance().ChangeWallHp(-damage);
        float wallCurHp = MainGameManager.GetInstance().GetWallCurHp();
        m_WallHpBar.m_HpBarFiller.fillAmount =  wallCurHp / MainGameManager.GetInstance().GetWallMaxHp();
        m_TotalWallHpBarStayTime = 4;
        
        if(wallCurHp<=0)
            GameOver(true);
    }


    public void StartReload(GunReloadControllerConfig gunReloadConfig){
        m_ReloadControllerPanel.SetActive(true);
        m_ReloadController.StartReload( gunReloadConfig );
    }

    public void CloseReloadPanel(){
        m_ReloadControllerPanel.SetActive(false);

    }

    public void SwitchSelectedWeapon(GunScriptable gun){
        m_GunController.SetSelectedGun(gun);
    }
}
