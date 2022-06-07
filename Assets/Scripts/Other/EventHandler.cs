using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TransformDelegate(Transform parameters);
public delegate void IntDelegate(int parameters);

public class EventHandler
{

#region ScorePlus
	private static List<TransformDelegate> ScorePlus_List;
	public static event TransformDelegate ScorePlus_Listener
	{
		add
		{
			if(value != null)
			{
				if(ScorePlus_List == null)
				{
					ScorePlus_List = new List<TransformDelegate>(1);
				}
				ScorePlus_List.Add(value);
			}
		}
		remove
		{
			if(value != null)
			{
				for(int i = 0;i<ScorePlus_List.Count;i++)
				{
					if(ScorePlus_List[i] != null && ScorePlus_List[i].Equals(value))
					{
						ScorePlus_List.RemoveAt(i);
						break;
					}
				}
			}
		}
	}
	public static void ScorePlus_Dispatch(params Transform[] parameters)
	{
		if(ScorePlus_List == null || ScorePlus_List.Count <= 0)
		{
			return;
		}
		for(int i = 0;i < ScorePlus_List.Count;i++)
		{
			ScorePlus_List[i]?.Invoke(parameters[i]);
		}
	}
#endregion

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
}