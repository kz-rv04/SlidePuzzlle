using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    TITLE,
    MAIN,
    PAUSE,
    RESULT
}
public class GameManager : MonoBehaviour {

    public static GameState state { get; set; }

	// Use this for initialization
	void Start () {
        state = GameState.TITLE;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
