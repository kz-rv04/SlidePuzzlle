using UnityEngine;
using System.Collections;

public enum BoardState
{
    START,
    FREE,
    MOVE,
    PAUSE,
    CLEAR
}
// メインゲーム画面でのコントローラーとして使用
public class GameController : MonoBehaviour {

    private static BoardState boardState;

    // パズルの設定画面のオブジェクト
    [SerializeField]
    private GameObject Config;


    // パズルの盤面のクラス
    private Board board;
    // 選択中のピース ゲームオブジェクトの名前をIDと一致させて識別
    private GameObject SelectedPanel;

    float timer;
    public float Timer
    {
        get { return timer; }
        set
        {
            timer = value;
            timerText.text = timer.ToString();
        }
    }
    UnityEngine.UI.Text timerText;

    int moves;

    // Use this for initialization
    void Start() {
        this.board = this.GetComponent<Board>();
        this.Config = GameObject.Find("Config");
        this.Config.SetActive(false);
        this.timerText = GameObject.Find("Timer").GetComponent<UnityEngine.UI.Text>();
        Timer = 0f;
        moves = 0;

        BoardStateProp = BoardState.START;
    }


    // Update is called once per frame
    void Update() {
        // ボタン押下時の処理
        if ((BoardStateProp == BoardState.FREE || BoardStateProp == BoardState.START) && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10.0f);
            if(hit.collider == null)
            {
                SelectedPanel = null;
                return;
            }
            if (hit.collider.tag == "Panel")
            {
                Panel currentPanel = hit.collider.gameObject.GetComponent<Panel>();
                // パネルの入れ替え先を探す
                int dest = board.SearchDest(currentPanel.PanelPos);
                //print(dest);
                // 移動できない場合
                if (dest == -1)
                {
                    return;
                }
                BoardStateProp = BoardState.MOVE;
                board.SwapPanels(currentPanel,dest);
                moves++;
            }
        }

        if (BoardStateProp == BoardState.FREE || BoardStateProp == BoardState.MOVE)
        {
            Timer += Time.deltaTime;
        }
    }

    public void StartGame() {
        this.board.InitBoard();
        print("パズル盤面初期化 GameStart");

        GameManager.state = GameState.MAIN;

        this.Config.SetActive(false);
    }

    public void OpenConfig() {
        this.Config.SetActive(true);
    }

    public static BoardState BoardStateProp
    {
        get { return boardState; }
        set
        {
            boardState = value;
        }
    }
}