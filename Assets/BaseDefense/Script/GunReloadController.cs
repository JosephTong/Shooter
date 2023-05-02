using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using GunReloadControllerNameSpase;
using GunReloadScriptableNameSpace;
using UnityEngine;
using UnityEngine.UI;

namespace GunReloadControllerNameSpase
{
    public class SpawnUIObjectForReloadPhaseConfig
    {
        public GameObject Prefab;
        public Vector2 Position;
    }
}

public class GunReloadController : MonoBehaviour
{
    [SerializeField] private GunReloadScriptable m_ReloadScriptable;
    [SerializeField] private bool m_IsPreview = false;


    [SerializeField] private Image m_MainGunImage;
    [SerializeField] private GameObject m_Gray;
    [SerializeField] private GameObject m_ArrowPrefab;
    [SerializeField] private Transform m_GrayWhileDragPanel; // gray out while draging
    [SerializeField] private Transform m_NotGrayWhileDragPanel; // NOT gray out while draging , for EndDragPrefab in ReloadScriptable
    private RectTransform m_DragImage; // mouse while dragging
    private GameObject m_EndDragImage;
    private GameObject m_DragArrow;
    private Sprite m_MainGunOldSprite; // incase canel drag and need to reset main gun image

    private List<GameObject> m_AllSpawnedImage = new List<GameObject>();
    private int m_CurReloadPhase = 0;

    private void Start() {
        InIt();
    }

    private void Update() {
        if(m_DragImage != null){
            m_DragImage.position = Input.mousePosition;
        }
    }

    public void InIt(){
        m_CurReloadPhase = 0;
        m_MainGunImage.sprite = m_ReloadScriptable.StartMainGunImage;
        m_MainGunImage.rectTransform.sizeDelta = m_ReloadScriptable.MainGunSize;

        SetPhase();
    }

    private void SetPhase(){
        if(m_ReloadScriptable.ReloadPhases.Count<=m_CurReloadPhase){
            // out of phase , cancel reload 
            Debug.Log("Cancel Reload by out of phase");
            ResultAction(GunReloadActionResult.CancelReload);
            return;
        }

        // clear all spawn reload UI
        for (int i = 0; i < m_AllSpawnedImage.Count; i++)
        {
            if(m_AllSpawnedImage[i] != null){
                Destroy(m_AllSpawnedImage[i]);
            }
        }
        m_AllSpawnedImage.Clear();

        // remove arrow
        if(m_DragArrow != null){
            Destroy(m_DragArrow);
            m_DragArrow = null;
        }
        
        // remove end drag
        if(m_EndDragImage != null){
            Destroy(m_EndDragImage);
            m_EndDragImage = null;
        }

        GunReloadPhase currentGunReloadPhase = m_ReloadScriptable.ReloadPhases[m_CurReloadPhase];
        for (int i = 0; i < currentGunReloadPhase.DragFunction.Count; i++)
        {
            SpawnDragItems(currentGunReloadPhase.DragFunction[i]);  
        }

        for (int i = 0; i < currentGunReloadPhase.TapFunction.Count ; i++)
        {
            SpawnTapItem(currentGunReloadPhase.TapFunction[i]);
        }

        for (int i = 0; i < currentGunReloadPhase.ExtraImages.Count; i++)
        {
            SpawnUIObjectForReloadPhaseConfig extraItemConfig = new SpawnUIObjectForReloadPhaseConfig{
                Prefab = currentGunReloadPhase.ExtraImages[i].ImagePrefab,
                Position = currentGunReloadPhase.ExtraImages[i].Position
            };
            SpawnUIObjectForReloadPhase( extraItemConfig );
        }
        m_MainGunOldSprite = null;
    }

    
    private GameObject SpawnUIObjectForReloadPhase(SpawnUIObjectForReloadPhaseConfig config){
        var spawnedUIObject = Instantiate(config.Prefab);
        spawnedUIObject.transform.SetParent(m_GrayWhileDragPanel);
        spawnedUIObject.GetComponent<RectTransform>().anchoredPosition = config.Position;
        m_AllSpawnedImage.Add(spawnedUIObject);
        return spawnedUIObject;
    }

    private void SpawnTapItem(GunReloadTapFunction tapFunction){
        SpawnUIObjectForReloadPhaseConfig config = new SpawnUIObjectForReloadPhaseConfig{
            Prefab = tapFunction.Prefab,
            Position = tapFunction.Position
        };
        Button tapBtn = SpawnUIObjectForReloadPhase( config ).GetComponent<Button>();
        tapBtn.onClick.AddListener(()=>{
            OnClickTap(tapFunction);
        });
    }

    private void OnClickTap(GunReloadTapFunction tapFunction){
        m_MainGunImage.sprite = tapFunction.MainGunSpriteOnEnd;
        ResultAction(tapFunction.ResultAction);
    }

    #region Drag
    private void SpawnDragItems(GunReloadDragFunction dragFunction){
        
        SpawnUIObjectForReloadPhaseConfig config = new SpawnUIObjectForReloadPhaseConfig{
            Prefab = dragFunction.StartDragPrefab,
            Position = dragFunction.StartDragPosition
        };
        Button2D startDragIcon = SpawnUIObjectForReloadPhase( config ).GetComponent<Button2D>();

        startDragIcon.onDown.AddListener(()=>{
            OnDownStartDrag(dragFunction);
        });

        startDragIcon.onUp.AddListener(()=>{
            OnUpStartDrag(dragFunction);
        });
    }

    private void OnEnterEndDrag(GunReloadDragFunction dragFunction){
        m_MainGunImage.sprite = dragFunction.MainGunSpriteOnEnd;
        ResultAction( dragFunction.ResultAction );
    }

    private void OnUpStartDrag(GunReloadDragFunction dragFunction){
        
        // remove arrow
        if(m_DragArrow != null){
            Destroy(m_DragArrow);
            m_DragArrow = null;
        }
        
        // remove end drag
        if(m_EndDragImage != null){
            Destroy(m_EndDragImage);
            m_EndDragImage = null;
        }

        // remove cursor image
        if(m_DragImage != null){
            Destroy(m_DragImage.gameObject);
            m_DragImage = null;
        }

        // reset main gun image
        m_MainGunImage.sprite = m_MainGunOldSprite;
        m_MainGunOldSprite = null;
    }

    private void OnDownStartDrag(GunReloadDragFunction dragFunction){

        // do not do anything if already draging
        if(m_DragImage != null)
            return;

        if(dragFunction.DragCursorPrefab == null){
            Debug.Log("Missing drag image for mouse");
            return;
        }

        // record old main gun image incase cancel drag
        m_MainGunOldSprite = m_MainGunImage.sprite;

        // remove arrow
        if(m_DragArrow != null){
            Destroy(m_DragArrow);
            m_DragArrow = null;
        }
        
        // remove end drag
        if(m_EndDragImage != null){
            Destroy(m_EndDragImage);
            m_EndDragImage = null;
        }

        // change main gun sprite
        m_MainGunImage.sprite = dragFunction.MainGunSpriteOnStart;


        // Spawn arrow
        SpawnUIObjectForReloadPhaseConfig arrowConfig = new SpawnUIObjectForReloadPhaseConfig{
            Prefab = m_ArrowPrefab,
            Position = dragFunction.StartDragPosition

        };
        m_DragArrow = SpawnUIObjectForReloadPhase( arrowConfig );
        m_DragArrow.transform.SetParent(m_NotGrayWhileDragPanel);
        m_DragArrow.GetComponent<RectTransform>().sizeDelta = new Vector2(
            m_DragArrow.GetComponent<RectTransform>().sizeDelta.x,
            Vector2.Distance(dragFunction.EndDragPosition , dragFunction.StartDragPosition)
        );
        Vector2 lookAngle = dragFunction.EndDragPosition - dragFunction.StartDragPosition;
        m_DragArrow.GetComponent<RectTransform>().localEulerAngles = new Vector3(0,0, Mathf.Rad2Deg * Mathf.Atan2(lookAngle.y,lookAngle.x) -90 );

        // spawn cursor imager
        if(dragFunction.DragCursorPrefab != null){
            SpawnUIObjectForReloadPhaseConfig cursorConfig = new SpawnUIObjectForReloadPhaseConfig{
                Prefab = dragFunction.DragCursorPrefab,
                Position = Input.mousePosition

            };
            GameObject cursor = SpawnUIObjectForReloadPhase( cursorConfig );
            cursor.transform.SetParent(m_NotGrayWhileDragPanel);
            m_DragImage = cursor.GetComponent<RectTransform>();
        }

        // Spawn end darg 
        SpawnUIObjectForReloadPhaseConfig dragEndConfig = new SpawnUIObjectForReloadPhaseConfig{
            Prefab = dragFunction.EndDragPrefab,
            Position = dragFunction.EndDragPosition

        };
        m_EndDragImage = SpawnUIObjectForReloadPhase( dragEndConfig );
        m_EndDragImage.GetComponent<Button2D>().onEnter.AddListener(()=>{
            OnEnterEndDrag(dragFunction);
        });
    }
    #endregion

    #region Result Action

    private void ResultAction(GunReloadActionResult actionEnum){
        if(actionEnum == (actionEnum | GunReloadActionResult.CancelReload) ){
            if(m_IsPreview){
                Debug.Log("Cancel Reload");
            }else{
                // cancel reload 

            }
        }

        if(actionEnum == ( actionEnum | GunReloadActionResult.GainOneAmmo ) ){
            if(m_IsPreview){
                Debug.Log("Gain one ammo");
            }else{
                // Gain one ammo 
 
            }
        }


        if(actionEnum == ( actionEnum | GunReloadActionResult.FullAmmoReload ) ){
            if(m_IsPreview){
                Debug.Log("Full Ammo Reload");
            }else{
                // Full Ammo Reload
 
            }
        }

        if(actionEnum == ( actionEnum | GunReloadActionResult.RefreshThisPhase ) ){
            // Refresh This Phase
            Debug.Log("Refresh This Phase");
            SetPhase();   
        }

        if(actionEnum == ( actionEnum | GunReloadActionResult.SetClipAmmoToZero ) ){
            if(m_IsPreview){
                Debug.Log("Set Clip Ammo To Zero");
            }else{
                // Set Clip Ammo To Zero
 
            }

        }

        if(actionEnum == ( actionEnum | GunReloadActionResult.ToNextPhase ) ){
            // To Next Phase
            Debug.Log("To Next Phase");
            ++m_CurReloadPhase;
            SetPhase();
        }

    }
        
    #endregion
}
