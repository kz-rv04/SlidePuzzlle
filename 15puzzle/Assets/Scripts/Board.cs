using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{

    // パズルの盤面を表す配列
    public Panel[] Panels;
    [SerializeField, Range(3, 6)]
    private int puzzleSize;
    [SerializeField]
    private Spawner spawner;

    public void InitBoard()
    {
        this.Panels = new Panel[puzzleSize * puzzleSize];
        this.spawner = this.GetComponent<Spawner>();

        // パズル画像の生成、初期化に成功したら
        if (this.spawner.Init(puzzleSize))
            CreatePuzzle(ref this.Panels);
    }

    // 盤面の配列を初期化(要素をランダムに並び替える)
    private void CreatePuzzle(ref Panel[] panels)
    {
        // 作成するパズルの問題
        int[] array = new int[panels.Length];
        // ランダム数列生成用の一時的な配列
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = i;
        }
        Shuffle(ref array);
        // パズルが解けない場合もう一度実行
        for (; !IsSolvable(array, puzzleSize);)
        {
            Shuffle(ref array);
        }

        // 生成した問題からパネルを生成する
        spawner.SpawnPanels(ref panels, ref array, this.puzzleSize);

        ShowBoard();
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
    public void SwapPanels(Panel selected, int dest)
    {
        Panel destPanel = Panels[dest];
        // 配列での位置を入れ替える
        Panels[dest] = selected;
        Panels[selected.PanelPos] = destPanel;

        // パネルのIDを入れ替える
        destPanel.PanelPos = selected.PanelPos;
        selected.PanelPos = dest;

        Vector3 destPos = destPanel.transform.position;

        destPanel.transform.position = selected.transform.position;

        selected.StartCoroutine(selected.Move(destPos, 0f,()=> {
            JudgeBoard();
        }));
        //selected.transform.position = destPos;

        ShowBoard();
    }

    public void JudgeBoard()
    {
        GameController.BoardStateProp = BoardState.FREE;
        if (IsMatched())
        {
            print("Clear");
            GameController.BoardStateProp = BoardState.CLEAR;
            Panels[puzzleSize * puzzleSize - 1].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
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
            //print("selected " + selected.ToString() + " dest " + (selected + 1).ToString());
            return selected + 1;
        }
        // 上への探索
        if (row > 0 && Panels[selected - puzzleSize].PanelIDProp == blank)
        {
            //print("selected " + selected.ToString() + " dest " + (selected - puzzleSize).ToString());
            return selected - puzzleSize;
        }
        // 下への探索
        if (row < puzzleSize - 1 && Panels[selected + puzzleSize].PanelIDProp == blank)
        {
            //print("selected " + selected.ToString() + " dest " + (selected + puzzleSize).ToString());
            return selected + puzzleSize;
        }

        return -1;
    }

    // for debug : Panelの要素を表示
    public void ShowBoard()
    {
        string info = "";
        for (int i = 0; i < puzzleSize; i++)
        {
            for (int j = 0; j < puzzleSize; j++) {
                info += Panels[i*puzzleSize + j].PanelIDProp;
                if (j != puzzleSize - 1)
                    info += ",";
            }
            info += '\n';
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

    /// <summary>
    /// パズルが解けるかどうか判定する
    /// </summary>
    /// <param name="array">判定する配列</param>
    /// <param name="size">パズルの縦横の長さ</param>
    /// <returns>パズルが解けるかどうか</returns>
    private bool IsSolvable(int[] array, int size)
    {
        List<int> puzzle = new List<int>();
        List<int> correct = new List<int>();
        // 奇数ならtrue,左上からスタート
        bool trigger = (size % 2 == 1) ? true : false;
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++)
            {
                int index;
                // trueなら列の左から
                if (trigger)
                {
                    index = i * size + j;
                }
                // falseなら列の右から
                else
                {
                    index = i * size + (size - j - 1);
                }
                puzzle.Add(array[index]);
                correct.Add(index);
            }
            trigger = !trigger;
        }

        puzzle.Remove(size * size - 1);
        correct.Remove(size * size - 1);
        /*
        string str = "[";
        puzzle.ForEach((n) =>
        {
            str += n.ToString() + ",";
        });
        str += "]";
        print(string.Format("Puzzle : {0} , {1}",puzzle.Count, str));
        */

        int distance = 0;

        for (int n = 0; n < size * size - 1; n++) {
            int moves = 0;
            for (int m = n; m < size * size - 1; m++) {
                if (puzzle[m] == correct[n]) {
                    break;
                }
                moves++;
            }

            if (moves > 0)
            {
                puzzle.MoveTo(n, moves);
                distance += moves;
            }
        }
        print("distance : " + distance);

        return (distance % 2 == 0) ? true : false;
    }


}

public static class ListExtensions
{
    /// <summary>
    /// サイズを設定します
    /// </summary>
    public static void SetSize<T>(this List<T> self, int size)
    {
        if (self.Count <= size) return;
        self.RemoveRange(size, self.Count - size);
    }

    public static T Pickup<T>(this List<T> list, int index)
    {
        if (index < 0 || list.Count <= index)
            throw new System.IndexOutOfRangeException();
        T item = list[index];
        list.RemoveAt(index);
        return item;
    }

    /// <summary>
    /// index + moveCountの位置にある要素をindexの位置に移動させる
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <param name="moveCount"></param>
    public static void MoveTo<T>(this List<T> list, int index, int moveCount)
    {
        if (moveCount == 0)
        {
            return;
        }
        T num = list.Pickup(index + moveCount);
        list.Insert(index, num);
    }
}