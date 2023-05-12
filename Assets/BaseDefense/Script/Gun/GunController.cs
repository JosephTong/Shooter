using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtendedButtons;
using TMPro;
using System.Linq;
using EZCameraShake;
using BaseDefenseNameSpace;
using UnityEngine.Rendering.Universal;

public class GunController : MonoBehaviour
{
    [SerializeField] private GunScriptable m_SelectedGun;
    [SerializeField] private SpriteRenderer m_FPSImage;


    [Header("Drag to aim")]
    [SerializeField][Range(10f, 500f)] private float m_CrossHairMaxSize = 300f;
    [SerializeField][Range(10f, 300f)] private float m_CrossHairMinSize = 50f;
    [SerializeField] private Button2D m_AimBtn;
    [SerializeField] private RectTransform m_CrossHair;
    [SerializeField] private Transform m_ShootDotParent;
    private Vector2 m_AimDragMouseStartPos = Vector2.zero;
    private Vector2 m_AimDragMouseEndPos = Vector2.zero;
    private Vector3 m_CrossHairDragStartPos;


    [Header("CrossHair light")]
    [SerializeField]private Transform m_CrossHairLight;
    [SerializeField][Range(1f,10f)]private float m_CrossHairLightMinSize = 4;
    [SerializeField]private Coroutine m_LightGainOnShootCoroutine = null;
    


    [Header("Aim effect for camera")]
    [SerializeField] private Camera m_MainCamera;
    [SerializeField] private Vector3 m_FieldCenter;
    private Vector3 m_MainCameraStartPos;
    private Vector3 m_AimDirection = Vector3.zero;
    private float m_AimDistanceNormalized = 0;
    private float m_ScreenCenterToCornerDistance = 10;

    [Header("Aim effect for gun")]
    [SerializeField] private Transform m_GunModel;


    [Header("Accracy")]
    private float m_CurrentAccruacy = 0;
    private Vector3 m_MousePreviousPos = Vector3.zero;

    [Header("Ammo")]
    [SerializeField] private TextMeshProUGUI m_AmmoText;
    [SerializeField] private List<WeaponToBeSwitch> m_WeaponsToBeSwitch = new List<WeaponToBeSwitch>();
    private float m_CurrentAmmo;
    private Dictionary<string,float> m_GunsClipAmmo = new Dictionary<string, float>(); // how many ammo left on gun when switching

    [Header("Shooting")]
    [SerializeField] private GameObject m_ShotPointPrefab; // indicate where the shot land 
    [SerializeField] private AudioSource m_ShootAudioSource;
    [SerializeField] private Button2D m_ShootBtn;
    private float m_CurrentShootCoolDown = 0; // must be 0 or less to shoot 
    private Coroutine m_SemiAutoShootCoroutine = null;

    [Header("Reload")]
    [SerializeField] private Button m_ReloadBtn;




    private void Start()
    {
        BaseDefenseManager.GetInstance().m_ShootUpdatreAction += UpdateCrossHair;
        BaseDefenseManager.GetInstance().m_ChangeToShootAction += ShowWeaponModel;
        BaseDefenseManager.GetInstance().m_ChangeFromShootAction += HideWeaponModel;
        BaseDefenseManager.GetInstance().m_UpdateAction += ShootCoolDown;
        MainGameManager.GetInstance().AddNewAudioSource(m_ShootAudioSource);

        // center crossHair 
        m_CrossHair.position = new Vector3(Screen.width/2,Screen.height/2, 0);

        m_SemiAutoShootCoroutine = null;
        this.transform.position = new Vector3(8, -4.5f, 0) + m_FieldCenter;
        m_MainCamera.transform.position = new Vector3(m_FieldCenter.x, m_FieldCenter.y, -10);
        m_ScreenCenterToCornerDistance = Mathf.Sqrt(Screen.height / 2 * Screen.height / 2 + Screen.width / 2 * Screen.width / 2 );
        m_MainCameraStartPos = m_MainCamera.transform.position;
        foreach (var item in m_WeaponsToBeSwitch)
        {
            m_GunsClipAmmo.Add(item.m_Gun.DisplayName,item.m_Gun.ClipSize);
        }

        m_ReloadBtn.onClick.AddListener(()=>{
            if(IsFullClipAmmo())
                return;
                
            GunReloadControllerConfig gunReloadConfig = new GunReloadControllerConfig{
                GunScriptable = m_SelectedGun,
                GainAmmo = GainAmmo,
                SetAmmoToFull = SetClipAmmoToFull,
                SetAmmoToZero = SetClipAmmoToZero,
                IsFullClipAmmo = IsFullClipAmmo,
            };
            BaseDefenseManager.GetInstance().StartReload(gunReloadConfig);
        });





        m_AimBtn.onDown.AddListener(() =>
        {
            m_MousePreviousPos = Input.mousePosition;
            m_AimDragMouseStartPos = Input.mousePosition;
            m_CrossHairDragStartPos = m_CrossHair.position;
        });
        m_AimBtn.onUp.AddListener(() =>
        {
            m_AimDragMouseStartPos = Vector2.zero;
            m_MousePreviousPos = Vector3.zero;
        });

        m_AimBtn.onExit.AddListener(() =>
        {
            m_AimDragMouseStartPos = Vector2.zero;
            m_MousePreviousPos = Vector3.zero;
        });

        m_ShootBtn.onDown.AddListener(() =>
        {
            
            if(m_CurrentAmmo<=0){
                m_ShootAudioSource.PlayOneShot(m_SelectedGun.OutOfAmmoSound);
            }else{
                m_SemiAutoShootCoroutine = null;
                if( m_SemiAutoShootCoroutine == null && m_SelectedGun.IsSemiAuto){
                    m_SemiAutoShootCoroutine = StartCoroutine( SemiAutoShoot() );
                    return;
                }
                Shoot();
            }
                
        });
        m_ShootBtn.onUp.AddListener(() =>
        {
            if(m_SemiAutoShootCoroutine!= null){
                StopCoroutine(m_SemiAutoShootCoroutine);
                m_SemiAutoShootCoroutine = null;
            }
        });

        m_ShootBtn.onExit.AddListener(() =>
        {
            if(m_SemiAutoShootCoroutine!= null){
                StopCoroutine(m_SemiAutoShootCoroutine);
                m_SemiAutoShootCoroutine = null;
            }
        });

        ChangeAmmoCount(m_SelectedGun.ClipSize, true);
        m_FPSImage.sprite = m_SelectedGun.FPSSprite;
        var crossHairworldPos = Camera.main.ScreenToWorldPoint(m_CrossHair.position);
        m_CrossHairLight.position = crossHairworldPos - Vector3.forward * crossHairworldPos.z;
    }

    private void HideWeaponModel(){
        m_FPSImage.gameObject.SetActive(false);
    }

    private void ShowWeaponModel(){
        m_FPSImage.gameObject.SetActive(true);
    }

    public void UpdateCrossHair(){
        // aim
        if (m_AimDragMouseStartPos != Vector2.zero)
        {
            m_AimDragMouseEndPos = Input.mousePosition;

            Vector3 offset = MainGameManager.GetInstance().GetAimSensitivity() * (m_AimDragMouseEndPos - m_AimDragMouseStartPos) *3 ;
            m_CrossHair.position = m_CrossHairDragStartPos + offset;

            // accrucy lose for moving
            float mouseMoveAmound = Vector3.Distance(m_MousePreviousPos, Input.mousePosition) /
                (Mathf.Sqrt(Screen.height * Screen.height + Screen.width * Screen.width) / 2) * 1000;

            if (mouseMoveAmound == 0)
            {
                // draging but not moving , gain accruacy over time
                AccruacyGainOvertime();
            }
            else
            {
                m_CurrentAccruacy -= Time.deltaTime * mouseMoveAmound * (100 - m_SelectedGun.Stability) *0.75f;
                m_MousePreviousPos = Input.mousePosition;
            }


            CrossHairOutOfBoundPrevention();

            // light follow crossHair
            var crossHairworldPos = Camera.main.ScreenToWorldPoint(m_CrossHair.position);
            m_CrossHairLight.position = crossHairworldPos - Vector3.forward * crossHairworldPos.z;

            // aim to camera effect
            m_AimDirection = (m_CrossHair.position - new Vector3( Screen.width , Screen.height ,0 ) ).normalized;
            m_AimDistanceNormalized = Vector3.Distance(m_CrossHair.position, new Vector3( Screen.width , Screen.height ,0 )) /
                m_ScreenCenterToCornerDistance;
            m_MainCamera.transform.position = m_MainCameraStartPos + m_AimDirection * m_AimDistanceNormalized;

            // aim to weapon effect
            float gunScaleX = 0.1f * ((m_CrossHair.position.x - Screen.width /2 ) / (Screen.width /2));
            m_GunModel.localScale = new Vector3(1 - gunScaleX, 1, 1);

            float gunRotationZ = -5 * (m_CrossHair.position.y - Screen.height /2) / (Screen.height /2);
            m_GunModel.localEulerAngles = new Vector3(0, 0, gunRotationZ);
        }
        else
        {
            AccruacyGainOvertime();
        }
        if (m_CurrentAccruacy < 0)
            m_CurrentAccruacy = 0;

        float targetCrossHairSize = Mathf.InverseLerp(100,0,m_CurrentAccruacy);
        targetCrossHairSize= Mathf.Lerp(m_CrossHairMinSize,m_CrossHairMaxSize,targetCrossHairSize);
        m_CrossHair.sizeDelta = Vector2.one * targetCrossHairSize;
    }


    private void ShootCoolDown()
    {
        // fire rate
        m_CurrentShootCoolDown -= Time.deltaTime;
    }

    public void SetSelectedGun(GunScriptable gun){
        m_GunsClipAmmo[m_SelectedGun.DisplayName] = m_CurrentAmmo;
        m_SelectedGun = gun;

        m_CurrentAccruacy = m_SelectedGun.Accuracy;
        m_SemiAutoShootCoroutine = null;
        ChangeAmmoCount(m_GunsClipAmmo[m_SelectedGun.DisplayName], true);
        m_FPSImage.sprite = m_SelectedGun.FPSSprite;
    }
    private void GainAmmo(int changes){
        ChangeAmmoCount(changes,false);
    }

    private bool IsFullClipAmmo(){
        return m_CurrentAmmo >= m_SelectedGun.ClipSize;
    }

    private void SetClipAmmoToZero(){
        ChangeAmmoCount(0,true);
    }

    private void SetClipAmmoToFull(){
        ChangeAmmoCount(m_SelectedGun.ClipSize,true);
    }
    private IEnumerator SemiAutoShoot(){
        while (m_CurrentAmmo>0)
        {        
            Shoot();
            yield return null;
        }
        
        m_ShootAudioSource.PlayOneShot(m_SelectedGun.OutOfAmmoSound);
    }

    private IEnumerator CrossHairLightGainOnShoot(){
        float timePass = 0; 
        float lightFadeTime = 0.45f;
        while (timePass<=1)
        {
            m_CrossHairLight.localScale = Vector3.one * Mathf.Lerp(m_CrossHairLightMinSize + m_SelectedGun.LightSizeGainOnShoot,m_CrossHairLightMinSize,timePass);
            timePass += Time.deltaTime / lightFadeTime;
            yield return null;
        }
        m_CrossHairLight.localScale = Vector3.one * m_CrossHairLightMinSize;
    }


    private void Shoot()
    {
        if (m_CurrentShootCoolDown > 0)
            return;

        // shake camera
        CameraShaker.Instance.ShakeOnce( m_SelectedGun.CameraShakeStrength , m_SelectedGun.CameraShakeAmount,0.1f,0.1f );

        // Light gain on shoot
        if(m_LightGainOnShootCoroutine != null)
            StopCoroutine(m_LightGainOnShootCoroutine);

        m_LightGainOnShootCoroutine = StartCoroutine(CrossHairLightGainOnShoot());

        // shoot sound
        m_ShootAudioSource.PlayOneShot(m_SelectedGun.ShootSound);

        // move cross hair up 
        m_CrossHair.position += new Vector3(Random.Range(-0.5f,0.5f),Random.Range(0f,0.5f),0) 
            *Mathf.Lerp ( 0, Screen.height / 100, (100 - m_SelectedGun.RecoilControl )/ 100 );

        CrossHairOutOfBoundPrevention();

        // light follow crossHair
        var crossHairworldPos = Camera.main.ScreenToWorldPoint(m_CrossHair.position);
        m_CrossHairLight.position = crossHairworldPos - Vector3.forward * crossHairworldPos.z;

        float targetCrossHairScale = Mathf.InverseLerp(100,0,m_CurrentAccruacy);
        float targetMaxRadius = Mathf.Lerp(0,m_CrossHairMaxSize/2-m_CrossHairMinSize/2,targetCrossHairScale);
        for (int j = 0; j < m_SelectedGun.PelletPerShot; j++)
        {       
            // random center to point distance
            float centerToPointDistance = Mathf.Lerp(0, targetMaxRadius, Random.Range(0,1f));
            float randomAngle = Random.Range(0,360f);
            Vector3 accuracyOffset = new Vector3( 
                Mathf.Sin(randomAngle*Mathf.Deg2Rad) * centerToPointDistance ,
                Mathf.Cos(randomAngle*Mathf.Deg2Rad) * centerToPointDistance ,
                0
            );

            // spawn dot for player to see
            var shotPoint = Instantiate(m_ShotPointPrefab);
            shotPoint.transform.SetParent(m_ShootDotParent);
            var targetPosForPoint = Camera.main.ScreenToWorldPoint( m_CrossHair.position + accuracyOffset );
            shotPoint.GetComponent<RectTransform>().position = m_CrossHair.position + accuracyOffset ; 
            Destroy(shotPoint,1);

            RaycastHit2D[] hits = Physics2D.RaycastAll( targetPosForPoint - Vector3.forward * targetPosForPoint.z, Vector2.zero);
            List<EnemyBodyPart> hitedEnemy = new List<EnemyBodyPart>();
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].collider.TryGetComponent<EnemyBodyPart>(out var enemyBodyPart);         
                if(enemyBodyPart != null){
                    hitedEnemy.Add(enemyBodyPart);
                }
            }
            if(hitedEnemy.Count>0){
                shotPoint.GetComponent<ShootDotController>().OnHit();
                var sortedEnemies = hitedEnemy.OrderBy(x => x.GetDistance()).ToList();
                sortedEnemies[0].OnHit(m_SelectedGun.Damage);
                if(sortedEnemies[0].IsCrit()){
                    shotPoint.GetComponent<ShootDotController>().OnCrit();
                }
            }else{
                shotPoint.GetComponent<ShootDotController>().OnMiss();
            }
        }
        


        m_CurrentAccruacy -= (100 - m_SelectedGun.RecoilControl );

        m_CurrentShootCoolDown = 1 / m_SelectedGun.FireRate;
        ChangeAmmoCount(-1, false);
    }

    private void ChangeAmmoCount(float num, bool isSetAmmoCount = false)
    {
        if (isSetAmmoCount)
        {
            m_CurrentAmmo = num;
        }
        else
        {
            m_CurrentAmmo += num;
        }
        if(m_CurrentAmmo >m_SelectedGun.ClipSize){
            m_CurrentAmmo =m_SelectedGun.ClipSize;
        }

        m_AmmoText.text = $"{m_CurrentAmmo} / {m_SelectedGun.ClipSize}";
    }


    private void AccruacyGainOvertime()
    {
        if (m_CurrentAccruacy < m_SelectedGun.Accuracy)
        {
            m_CurrentAccruacy += Time.deltaTime * m_SelectedGun.Handling;
        }
        else
        {
            m_CurrentAccruacy = m_SelectedGun.Accuracy;
        }
    }


    private void CrossHairOutOfBoundPrevention()
    {
        if (m_CrossHair.position.x < 0)
        {
            // Left out of bound
            m_CrossHair.position = new Vector3(0, m_CrossHair.position.y, 0);
        }

        if (m_CrossHair.position.x > Screen.width)
        {
            // Right out of bound
            m_CrossHair.position = new Vector3(Screen.width, m_CrossHair.position.y, 0);
        }


        if (m_CrossHair.position.y > Screen.height)
        {
            // Top out of bound
            m_CrossHair.position = new Vector3(m_CrossHair.position.x, Screen.height, 0);
        }


        if (m_CrossHair.position.y < 0)
        {
            // Down out of bound
            m_CrossHair.position = new Vector3(m_CrossHair.position.x, 0, 0);
        }
    }


    private void OnDestroy() {
        
    }
}
