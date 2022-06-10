using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void IntDelegate(int parameters);
public delegate void VoidDelegate();

public class EventHandler
{

#region ScoreTween
	private static List<IntDelegate> ScoreTween_List;
	public static event IntDelegate ScoreTween_Listener
	{
		add
		{
			if(value != null)
			{
				if(ScoreTween_List == null)
				{
					ScoreTween_List = new List<IntDelegate>(1);
				}
				ScoreTween_List.Add(value);
			}
		}
		remove
		{
			if(value != null)
			{
				for(int i = 0;i<ScoreTween_List.Count;i++)
				{
					if(ScoreTween_List[i] != null && ScoreTween_List[i].Equals(value))
					{
						ScoreTween_List.RemoveAt(i);
						break;
					}
				}
			}
		}
	}
	public static void ScoreTween_Dispatch(params int[] parameters)
	{
		if(ScoreTween_List == null || ScoreTween_List.Count <= 0)
		{
			return;
		}
		for(int i = 0;i < ScoreTween_List.Count;i++)
		{
			ScoreTween_List[i]?.Invoke(parameters[i]);
		}
	}
#endregion

#region GameStart
	private static List<VoidDelegate> GameStart_List;
	public static event VoidDelegate GameStart_Listener
	{
		add
		{
			if(value != null)
			{
				if(GameStart_List == null)
				{
					GameStart_List = new List<VoidDelegate>(1);
				}
				GameStart_List.Add(value);
			}
		}
		remove
		{
			if(value != null)
			{
				for(int i = 0;i<GameStart_List.Count;i++)
				{
					if(GameStart_List[i] != null && GameStart_List[i].Equals(value))
					{
						GameStart_List.RemoveAt(i);
						break;
					}
				}
			}
		}
	}
	public static void GameStart_Dispatch()
	{
		if(GameStart_List == null || GameStart_List.Count <= 0)
		{
			return;
		}
		for(int i = 0;i < GameStart_List.Count;i++)
		{
			GameStart_List[i]?.Invoke();
		}
	}
#endregion

#region GameExit
	private static List<VoidDelegate> GameExit_List;
	public static event VoidDelegate GameExit_Listener
	{
		add
		{
			if(value != null)
			{
				if(GameExit_List == null)
				{
					GameExit_List = new List<VoidDelegate>(1);
				}
				GameExit_List.Add(value);
			}
		}
		remove
		{
			if(value != null)
			{
				for(int i = 0;i<GameExit_List.Count;i++)
				{
					if(GameExit_List[i] != null && GameExit_List[i].Equals(value))
					{
						GameExit_List.RemoveAt(i);
						break;
					}
				}
			}
		}
	}
	public static void GameExit_Dispatch()
	{
		if(GameExit_List == null || GameExit_List.Count <= 0)
		{
			return;
		}
		for(int i = 0;i < GameExit_List.Count;i++)
		{
			GameExit_List[i]?.Invoke();
		}
	}
#endregion
}