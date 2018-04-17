using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageClipper :MonoBehaviour{
    
    [SerializeField]
    private RawImage rawImg;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Text debugText;

    [SerializeField]
    private RectTransform wire;
    [SerializeField]
    private Slider Scaler;
    private bool canControll;
    private bool canMove;

    // dont use
    #region
    [SerializeField, Range(0f, 1.0f),HideInInspector]
    private float distance;

    private Color FLESH_COLOR = new Color(252f / 255f, 226f / 226f, 196f / 255f);
    private Color BROWN_COLOR = new Color(138f / 255f, 59f / 226f, 1f / 255f);
    #endregion

    void Update()
    {
        if (canControll)
        {
            SetClipAreaSize();
            if (Input.GetMouseButtonDown(0))
            {
                canMove = TouchClipArea();
            }
            if (Input.GetMouseButton(0) && canMove)
            {
                this.MoveClipArea();
            }
            if (Input.GetMouseButtonUp(0))
            {
                canMove = false;
            }
        }
    }
    
    /*
    // ImageComponentに取得した画像を貼り付ける関数
    public IEnumerator ShowImg(string path)
    {
        WWW www = new WWW("file:///" + path);
        yield return www;
        Texture2D tex = www.texture;
        if (tex == null)
        {
            Debug.LogError("texture not found");
        }
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        
        this.target.GetComponent<Image>().sprite = sprite;
        this.target.GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width, tex.height);
    }
    */

    // pathから画像を探して表示する
    public IEnumerator ShowTex(string path)
    {
        this.debugText.text = path;
        WWW www = new WWW("file:///" + path);
        yield return www;
        Texture2D tex = www.texture;
        if (tex == null)
        {
            Debug.LogError("texture not found");
        }

        DisplayTex(tex);
    }

    public Texture2D ClipTex(Texture2D tex)
    {

        Color[] pixels;
        Texture2D out_tex;
        int BottomLeftX, BottomLeftY;
        BottomLeftX = Mathf.Clamp((int)this.wire.anchoredPosition.x + tex.width / 2 - (int)(this.wire.sizeDelta.x / 2), 0,tex.width) ;
        BottomLeftY = Mathf.Clamp((int)this.wire.anchoredPosition.y + tex.height / 2 - (int)(this.wire.sizeDelta.y / 2), 0, tex.height);
        print("X : " + BottomLeftX + "Y : " + BottomLeftY);
        
        pixels = tex.GetPixels(BottomLeftX, BottomLeftY, (int)this.wire.sizeDelta.x, (int)this.wire.sizeDelta.y);
        out_tex = new Texture2D((int)this.wire.sizeDelta.x,(int)this.wire.sizeDelta.y);
        out_tex.SetPixels(pixels);
        out_tex.Apply();
        return out_tex;
    }

    public Texture2D ConvertTex(Texture2D tex)
    {
        // Textureからが措置を取得
        Color[] pixels = tex.GetPixels();
        // 書き換えテクスチャ用配列
        Color[] out_pixels = new Color[pixels.Length];
        for (int i = 0; i < pixels.Length; i++) {
            /*
            Color p = Color.white - pixels[i];
            p.a = 1.0f;
            */
            if (((pixels[i].a - FLESH_COLOR.a) * (pixels[i].a - FLESH_COLOR.a)
                + (pixels[i].g - FLESH_COLOR.g) * (pixels[i].g - FLESH_COLOR.g)
                + (pixels[i].b - FLESH_COLOR.b) * (pixels[i].b - FLESH_COLOR.b)) < distance)
            {
                out_pixels[i] = BROWN_COLOR;
            }
            else
            {
                out_pixels[i] = pixels[i];
            }

        }

        Texture2D out_tex = new Texture2D(tex.width, tex.height);
        out_tex.filterMode = FilterMode.Bilinear;
        out_tex.SetPixels(out_pixels);
        out_tex.Apply();
        return out_tex;
    }

    public void Clip()
    {
        DisplayTex(ClipTex((Texture2D)this.rawImg.texture));
    }

    // テクスチャをCanvasに表示
    public void DisplayTex(Texture2D tex)
    {
        float shorter;
        CanvasScaler cs = this.canvas.GetComponent<CanvasScaler>();
        if (tex.width <= tex.height)
        {
            shorter = tex.width;
            cs.matchWidthOrHeight = 1.0f;
        }
        else
        {
            shorter = tex.height;
            cs.matchWidthOrHeight = 1.0f;
        }

        cs.referenceResolution = new Vector2(tex.width,tex.height);
        this.rawImg.GetComponent<RawImage>().texture = tex;
        RectTransform rect = this.rawImg.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(tex.width, tex.height);

        DisplayControll(shorter);
    }

    /// <summary>
    /// 切り取り範囲を表示する
    /// </summary>
    /// <param name="pos">中心座標</param>
    /// <param name="maxSize">正方形の辺の最大の長さ</param>
    public void DisplayControll(float maxSize)
    {
        this.Scaler.enabled = true;
        this.wire.GetComponent<RawImage>().enabled = true;
        Scaler.maxValue = maxSize;
        Scaler.value = 0;
        this.wire.anchoredPosition = Vector2.zero;
        canControll = true;
    }

    private bool TouchClipArea()
    {
        Vector2 pos = GetCanvasPos(Input.mousePosition, this.canvas);
        RaycastHit hit;
        if (Physics.Raycast(Input.mousePosition + new Vector3(0, 0, -1.0f), Vector3.forward, out hit))
        {
            if (hit.collider.tag.Equals("ClipArea"))
            {
                return true;
            }
        }
        return false;
    }

    private void MoveClipArea()
    {
        Texture2D tex = this.rawImg.texture as Texture2D;
        Vector2 pos = GetCanvasPos(Input.mousePosition, this.canvas);
        pos.x = Mathf.Clamp(pos.x, -(tex.width - this.wire.sizeDelta.x) / 2, (tex.width - this.wire.sizeDelta.x) /2 );
        pos.y = Mathf.Clamp(pos.y, - (tex.height - this.wire.sizeDelta.y ) / 2, (tex.height - this.wire.sizeDelta.y) / 2);
        this.wire.anchoredPosition = pos;
    }
    private void SetClipAreaSize()
    {
        Vector2 size = new Vector2(Scaler.value, Scaler.value);
        this.wire.sizeDelta = size;
        this.wire.GetComponent<BoxCollider>().size = size;
    }

    private Vector2 GetCanvasPos(Vector2 mousePos,Canvas canvas)
    {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        Vector2 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        pos -= new Vector2(0.5f, 0.5f);
        pos.x *= rect.sizeDelta.x;
        pos.y *= rect.sizeDelta.y;
        return pos;
    }
}
