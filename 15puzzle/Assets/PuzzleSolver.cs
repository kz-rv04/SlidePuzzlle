using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PuzzleSolver : MonoBehaviour {

    private Board board;

    private List<int> canSwap = new List<int>();

    private UnityEngine.UI.Text moveCountText;
    private int moves;
    public int Moves
    {
        get { return moves; }
        set
        {
            moves = value;
            moveCountText.text = moves.ToString();
        }
    }

    // 以下パズル解答に用いる変数など
    public static int N, NN;

    static int[,] MDTable;

    static int getManhattanDistance(int x1, int y1, int x2, int y2) {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }
    readonly int[] dx = { 0, 1, 0, -1 };
    readonly int[] dy = { -1, 0, 1, 0 };
    readonly string[] dirs = { "l", "u", "r", "d" };


	// Use this for initialization
	void Start () {
        board = GameObject.Find("GameController").GetComponent<Board>();
        GameController.BoardStateProp = BoardState.START;

        moveCountText = GameObject.Find("MoveCounter").GetComponent<UnityEngine.UI.Text>();

        // Solverの変数などを初期化
        N = board.GetPuzzleSize;
        NN = N * N;
        print(string.Format("N : {0},NN : {1}",N,NN));

        initMDTable(); // テーブル初期化
        // kokomade
    }

    // Update is called once per frame
    void Update () {
        if(GameManager.state == GameState.MAIN && 
            GameController.BoardStateProp == BoardState.START && Input.GetKeyDown(KeyCode.Space))
        {
            string ans = astar(board);
            print(string.Format("Count : {0},Path :  {1}", ans.Length, ans));
            GameController.BoardStateProp = BoardState.CLEAR;
        }
    }

    string astar(Board board) {
        HashSet<string> done = new HashSet<string>();
        Queue<Puzzle> q = new Queue<Puzzle>();

        Puzzle p = new Puzzle(board.Panels);
        p.estimate = p.md;
        q.Enqueue(p);
        done.Add(p.ToString());

        for (; q.Count > 0;) {
            print(q.Count);
            p = q.Dequeue();
            if(p.md == 0) {
                return p.path;
            }

            done.Add(p.ToString());

            int sx = p.space % N;
            int sy = p.space / N;
            for (int i = 0; i < dx.Length; i++) {
                int nx = sx + dx[i];
                int ny = sy + dy[i];
                if (nx < 0 || N <= nx || ny < 0 || N <= ny) {
                    continue;
                }

                Puzzle p2 = (Puzzle)p.Clone();
                p2.changeSpace(nx, ny);
                if (!done.Contains(p2.ToString())) {
                    p2.cost++;
                    p2.estimate = p2.md + p2.cost;
                    //p2.estimate = p2.md;
                    p2.path += dirs[i];
                    q.Enqueue(p2);
                }
            }
        }
        return "unsolvable";
    }


    // マンハッタン距離のテーブルを初期化する関数
    void initMDTable()
    {
        MDTable = new int[NN, NN];
        for (int i = 0; i < NN; i++)
        {
            for (int j = 0; j < NN; j++)
            {
                MDTable[i, j] = getManhattanDistance(i % N, i / N, j % N, j / N);
            }
        }
    }

    public class Puzzle : IComparable,ICloneable
    {
        public int[] panel = new int[NN]; // 0 ~ (NN-1) の正方形の盤面を想定 
        public int space;
        public int md;
        public int cost;
        public int estimate;
        public string path = "";

        public Puzzle() { 
        }
        public Puzzle(Panel[] p) {
            InitParams(p);
        }

        void InitParams(Panel[] panels)
        {
            for(int i = 0;i < NN; i++)
            {
                // 空白の場合
                if (panels[i].PanelIDProp == NN - 1)
                {
                    space = i;
                }
                panel[i] = panels[i].PanelIDProp;
            }
            calcMD(); // 全パネルのマンハッタン距離の合計を求める
        }


        void calcMD() {
            md = 0;
            for (int i = 0; i < NN; i++) {
                if (panel[i] != NN) {
                    md += MDTable[i, panel[i]];
                }
            }
        }

        public void changeSpace(int x, int y) {
            int index = x + y * N;
            md += MDTable[space, panel[index]];
            md -= MDTable[index, panel[index]];
            panel[space] = panel[index];
            panel[index] = NN - 1;
            space = index;
        }


        public override string ToString() {
            string s = "";
            for (int i = 0; i < NN; i++) {
                if (i > 0) {
                    if (i % N == 0)
                    {
                        s += "\n";
                    }
                    else
                    {
                        s += " ";
                    }
                }
                s += panel[i];
            }
            return s.ToString();
        }

        public object Clone()
        {
            try {
                Puzzle obj = new Puzzle();
                obj.panel = (int[])this.panel.Clone();
                obj.md = md;
                obj.space = space;
                obj.cost = cost;
                obj.estimate = estimate;
                obj.path = path;
                return obj;
            }catch(NotSupportedException e)
            {
                throw e;
            }
        }

        public int CompareTo(object obj)
        {
            return this.estimate - ((Puzzle)obj).estimate;
        }
    }
}


public static class DictionaryExtensions
{
    public static KeyValuePair<TKey, TValue> GetRandomItem<TKey,TValue>(this Dictionary<TKey,TValue> dict)
    {
        return dict.ElementAt(UnityEngine.Random.Range(0, dict.Count));
    }
}