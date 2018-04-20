using UnityEngine;
using System;
using System.Collections;

public class Panel : MonoBehaviour{

    // パネルのID
    private int id;
    // パネルの配列内での位置
    private int index;
    
    public int PanelIDProp{
        get { return this.id; }
        set { this.id = value; }
    }
    public int PanelPos
    {
        get { return this.index; }
        set { this.index = value; }
    }

    public IEnumerator Move(Vector3 dest,float duration,Action OnCompleted=null){
        float starttime = Time.time;
        Vector3 startpos = this.transform.position;
        for(;Time.time - starttime < duration;)
        {
            this.transform.position = Vector3.Lerp(startpos, dest, (Time.time - starttime) / duration);
            yield return new WaitForEndOfFrame();
        }
        this.transform.position = dest;

        if (OnCompleted != null) {
            OnCompleted();
        }
    
    }
}
