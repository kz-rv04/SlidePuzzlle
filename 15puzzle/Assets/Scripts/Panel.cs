using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Panel : MonoBehaviour{

    private Image image;
    // パネルのID
    [SerializeField]
    private int id;
    // パネルの配列内での位置
    [SerializeField]
    private int index;

    public Panel(Image panel,int n)
    {
        this.image = panel;
        this.id    = n;
    }

    public Image GetPanelImage {
        get { return this.image; }
    }
    public int PanelIDProp{
        get { return this.id; }
        set { this.id = value; }
    }
    public int PanelPos
    {
        get { return this.index; }
        set { this.index = value; }
    }
}
