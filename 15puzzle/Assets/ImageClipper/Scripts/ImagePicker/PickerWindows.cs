#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using System.Windows.Forms;
using UnityEngine;

namespace Kakera
{
    namespace KZ
    {
        namespace IO
        {
            internal class PickerWindows : IPicker
            {
                // 最近使用したファイルのパス
                [UnityEngine.SerializeField]
                private string RU_path;

                public PickerWindows()
                {
                    this.RU_path = string.Empty;
                }
                // 最近使用したパスを用いて初期化
                public PickerWindows(string path)
                {
                    this.RU_path = path;
                }
                public void Show(string title, string outputFileName, int size)
                {
                    OpenFileDialog ofd = new OpenFileDialog();

                    ofd.Filter = "Image Files(*.BMP;*.PNG;*.JPG;*.GIF)|*.BMP;*.PNG;*.JPG;*.GIF|All files (*.*)|*.*";

                    //ファイルが実在しない場合は警告を出す(true)、警告を出さない(false)
                    ofd.CheckFileExists = false;

                    if (!string.IsNullOrEmpty(RU_path))
                        ofd.InitialDirectory = this.RU_path;

                    GameObject receiver = GameObject.Find("Unimgpicker");

                    //ダイアログを開く
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        UnityEngine.MonoBehaviour.print("selected img : " + ofd.FileName);
                        this.RU_path = ofd.FileName;
                        receiver.SendMessage("OnComplete", ofd.FileName);
                    }
                }
            }
        }
    }
}
#endif