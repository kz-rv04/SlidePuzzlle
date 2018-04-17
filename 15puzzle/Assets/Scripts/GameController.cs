using UnityEngine;
using System.Collections;

// メインゲーム画面でのコントローラーとして使用
public class GameController : MonoBehaviour {

    // パズルの盤面のクラス
    [SerializeField]
    private Board board;
    // 選択中のピース ゲームオブジェクトの名前をIDと一致させて識別
    private GameObject SelectedPanel;

    // Use this for initialization
    void Start() {
        this.board = this.GetComponent<Board>();
    }

    // Update is called once per frame
    void Update() {
        // ボタン押下時の処理
        if (Input.GetMouseButtonDown(0))
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
                board.SwapPanels(currentPanel);
            }
        }
    }
}