using UnityEngine;

public class GameEvent
{
}

public class GameEvent_SetPause : GameEvent
{
	public bool status;
	public GameEvent_SetPause(bool status)
	{
		this.status = status;
	}
}

public class GameEvent_GameStart : GameEvent
{
	public GameEvent_GameStart()
	{
	}
}

public class GameEvent_GameOver : GameEvent
{
	public GameEvent_GameOver()
	{
	}
}
