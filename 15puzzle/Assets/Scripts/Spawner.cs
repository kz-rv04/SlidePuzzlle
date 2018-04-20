using UnityEngine;
using UnityEngine.UI;

public class Spawner :MonoBehaviour{

    [SerializeField]
    private GameObject panelObj;
    [SerializeField]
    private Transform panelField;
    // パズルのピースに使う画像
    public Sprite[] sprites;
    // 切り取る元画像
    public Texture2D raw;

    private Vector2 offset;

    private float scaleFactor;

    private Color SKELETON_COLOR = new Color(255, 255, 255, 0);

    public bool Init(int puzzlesize) {
        this.sprites = Slice(puzzlesize);
        if (this.sprites != null) return true;
        return false;
    }

    public Sprite[] Slice(int puzzlleSize) {

        Sprite[] sliced = null;

        if (this.raw != null) {

            Texture2D tex = this.raw;

            // 完成図の画像を表示しておく
            GameObject.Find("Preview").GetComponent<RawImage>().texture = tex;

            Vector2 size = new Vector2(tex.width,tex.height);

            if (size.x != size.y) return null;

            sliced = new Sprite[puzzlleSize * puzzlleSize];

            Vector2 pivot = Vector2.one / 2;
            Vector2 unit = size / puzzlleSize;
            this.offset = unit / 100; // pixelsize / pixelperunitの値
            this.scaleFactor = (1024f / puzzlleSize) / (tex.width / puzzlleSize);
            //print(scaleFactor);

            Rect rect = new Rect(0, tex.height-unit.y, unit.x, unit.y);

            for (int i = 0; i < puzzlleSize; i++) {
                for (int j = 0; j < puzzlleSize; j++) {
                    //print(rect);

                    var s = Sprite.Create(tex, rect, pivot);
                    sliced[i * puzzlleSize + j] = s;
                    rect.x += unit.x;
                }
                rect.x = 0;
                rect.y -= unit.y;
            }
        }
        return sliced;
    }

    // パズルのピースを生成、必要な情報(パネル画像,UI座標)
    public void SpawnPanels(ref Panel[] panels,ref int[] board,int puzzleSize)
    {
        float scaledSize = this.offset.x * scaleFactor;
        float scale = (((float)(puzzleSize - 1) / 2));

        Vector2 initPos = new Vector2(-scaledSize * scale, scaledSize* scale);
        Vector3 def = new Vector3(1.0f*scaleFactor, 1.0f*scaleFactor, 1.0f);
        int x = 0;
        for (int i = 0; i < puzzleSize; i++)
        {
            for (int j = 0; j < puzzleSize; j++)
            {
                GameObject obj = Instantiate(panelObj,panelField) as GameObject;
                obj.tag = "Panel";
                // boardに格納されているIDに対応するSpriteを貼る
                obj.GetComponent<SpriteRenderer>().sprite = this.sprites[board[x]];
                // 右下のパネルのみ透明にしておく
                if (board[x] == board.Length - 1)
                {
                    obj.GetComponent<SpriteRenderer>().color = SKELETON_COLOR;
                }

                obj.transform.SetParent(panelField.transform);
                obj.GetComponent<BoxCollider2D>().size = new Vector2(this.offset.x, this.offset.y);
                obj.name = board[x].ToString();
                Transform transform = obj.GetComponent<Transform>();

                transform.localPosition = initPos + new Vector2(scaledSize*j,-scaledSize*i);

                transform.localScale = def;

                Panel panel = obj.GetComponent<Panel>();
                panel.PanelPos = x;
                panel.PanelIDProp = board[x];
                panels[x] = panel;
                x++;
            }
        }
    }
}
