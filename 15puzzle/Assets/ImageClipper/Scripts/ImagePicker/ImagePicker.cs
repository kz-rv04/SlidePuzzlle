using UnityEngine;
using Kakera;

public class ImagePicker : MonoBehaviour {

    [SerializeField]
    private Unimgpicker picker;
    [SerializeField]
    private ImageClipper clipper;
    void Awake()
    {
        this.picker = this.GetComponent<Unimgpicker>();
        this.clipper = GameObject.Find("ImageClipper").GetComponent<ImageClipper>();

        // 画像選択終了時の処理を追加
        picker.Completed += (string path) =>
        {
            // 画像選択時に返ってきたpathの画像を表示する
            StartCoroutine(this.clipper.ShowTex(path));
        };

    }

    public void OnShowPickerPressed()
    {
        picker.Show("Select Image", "unimgpicker", 1024);
    }
}
