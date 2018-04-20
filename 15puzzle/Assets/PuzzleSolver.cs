using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

	// Use this for initialization
	void Start () {
        board = GameObject.Find("GameController").GetComponent<Board>();
        GameController.BoardStateProp = BoardState.START;

        moveCountText = GameObject.Find("MoveCounter").GetComponent<UnityEngine.UI.Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if ((GameController.BoardStateProp == BoardState.FREE || GameController.BoardStateProp == BoardState.START
            && GameManager.state == GameState.MAIN)) {
            GameController.BoardStateProp = BoardState.MOVE;
            var canSwapPanels = SearchNextPanel();
            var item = canSwapPanels.GetRandomItem();
            board.SwapPanels(item.Key, item.Value);
            Moves++;
        }

    }

    // 次に動かせるパネルを探す
    private Dictionary<Panel,int> SearchNextPanel() {
        // パネルと動かせる方向の組の辞書
        Dictionary<Panel,int> dic = new Dictionary<Panel,int>();
        foreach(Panel p in board.Panels)
        {
            int dest = board.SearchDest(p.PanelPos);
            if (dest != -1)
            {
                dic.Add(p, dest);
            }
        }
        return dic;
    }
}


public static class DictionaryExtensions
{
    public static KeyValuePair<TKey, TValue> GetRandomItem<TKey,TValue>(this Dictionary<TKey,TValue> dict)
    {
        return dict.ElementAt(Random.Range(0, dict.Count));
    }
}