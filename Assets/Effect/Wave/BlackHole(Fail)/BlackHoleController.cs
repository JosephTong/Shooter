using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    private Material m_Mat = null;

    void Start()
    {
        m_Mat = m_SpriteRenderer.material;
        transform.localScale = new Vector3(1,1,1);
        
        float width = m_SpriteRenderer.sprite.bounds.size.x;
        float height = m_SpriteRenderer.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        
        transform.localScale = new Vector2( worldScreenWidth / width, worldScreenHeight / height);
        m_Mat.SetFloat("_ScreenRatio", (float)Screen.width/(float)Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
