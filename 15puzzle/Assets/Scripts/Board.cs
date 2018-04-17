using UnityEngine;
using System.Collections;

public class Board :MonoBehaviour {

    // パズルの盤面を表す配列
    public Panel[] Panels;
    [SerializeField, Range(3, 6)]
    private int puzzleSize;
    [SerializeField]
    private Spawner spawner;
    
    void Start()
    {
        this.Panels = new Panel[puzzleSize*puzzleSize];
        this.spawner = this.GetComponent<Spawner>();
        this.spawner.Init(puzzleSize);
        CreatePuzzle(ref this.Panels);
    }
    // 盤面の配列を初期化(要素をランダムに並び替える)
    private void CreatePuzzle(ref Panel[] panels)
    {
        // 作成するパズルの問題
        int[] array = new int[panels.Length];
        // ランダム数列生成用の一時的な配列
        for (int i = 0; i < array.Length;i++) {
            array[i] = i;
        }
        Shuffle(ref array);
        // パズルが解けない場合もう一度実行


        // 生成した問題からパネルを生成する
        spawner.SpawnPanels(ref panels,ref array, this.puzzleSize);
    }
    private void Shuffle(ref int[] array)
    {
        System.Random rand = new System.Random();
        for (int n = array.Length - 1; n > 1; --n)
        {
            int k = rand.Next(n + 1);
            int tmp = array[k];
            array[k] = array[n];
            array[n] = tmp;
        }
    }

    /// <summary>
    /// 選択したパネルを移動できる位置に移動させる
    /// </summary>
    /// <param name="selected">選択したパネル</param>
    public void SwapPanels(Panel selected)
    {
        //print("Search  selected : " + selected);
        // 上下左右に移動するパネルがあるか判定、移動できる場合は移動
        int dest = SearchDest(selected.PanelPos);
        // 移動できない場合はreturn
        if (dest == -1)
        {
            print("not found");
            return;
        }
        Panel destPanel = Panels[dest];
        // 配列での位置を入れ替える
        Panels[dest] = selected;
        Panels[selected.PanelPos] = destPanel;

        // パネルのIDを入れ替える
        destPanel.PanelPos = selected.PanelPos;
        selected.PanelPos = dest;
        
        Vector3 tmp = selected.transform.position;
        selected.transform.position = destPanel.transform.position;
        destPanel.transform.position = tmp;

        ShowBoard();
        if (IsMatched())
        {
            print("Clear");
        }
    }

    // 空白のマス目の位置を探して返す
    public int SearchDest(int selected)
    {
        int blank = puzzleSize * puzzleSize - 1;

        int row = selected / puzzleSize;
        int col = selected % puzzleSize;

        // 左への探索
        if (col > 0 && Panels[selected - 1].PanelIDProp == blank)
        {
            return selected - 1;
        }
        // 右への探索
        if (col < puzzleSize - 1 && Panels[selected + 1].PanelIDProp == blank)
        {
            print("selected " + selected.ToString() + " dest " + (selected + 1).ToString());
            return selected + 1;
        }
        // 上への探索
        if (row > 0 && Panels[selected - puzzleSize].PanelIDProp == blank)
        {
            print("selected " + selected.ToString() + " dest " + (selected - puzzleSize).ToString());
            return selected - puzzleSize;
        }
        // 下への探索
        if (row < puzzleSize - 1 && Panels[selected + puzzleSize].PanelIDProp == blank)
        {
            print("selected " + selected.ToString() + " dest " + (selected + puzzleSize).ToString());
            return selected + puzzleSize;
        }

        return -1;
    }

    // for debug : Panelの要素を表示
    public void ShowBoard()
    {
        string info = "";
        for (int i = 0; i < Panels.Length; i++)
        {
            info += Panels[i].PanelIDProp + " ";
        }
        Debug.Log(info);
    }

    private bool IsMatched()
    {
        for (int i = 0; i < Panels.Length; i++)
        {
            if (Panels[i].PanelIDProp != i)
                return false;
        }
        return true;
    }
}
