using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_GameState
{
    None,
    Start,
    Lobby,
    Battle,
}

public class GameStateManager : Singletone<GameStartManager>
{
    private E_GameState gameState = E_GameState.None;
    public E_GameState GameState
    {
        get { return gameState; }
        set
        {
            if (value == gameState)
                return;

            switch (gameState)
            {
                case E_GameState.None:
                    break;

                case E_GameState.Start:
                    OnChangedStart.Execute();
                    break;

                case E_GameState.Lobby:
                    OnChangedLobby.Execute();
                    break;

                case E_GameState.Battle:
                    OnChangedBattle.Execute();
                    break;
            }
        }
    }

    public Action OnChangedStart { get; set; }
    public Action OnChangedLobby { get; set; }
    public Action OnChangedBattle { get; set; }

	void Start () {
		
	}
}
