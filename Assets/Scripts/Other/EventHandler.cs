using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void IntDelegate(int parameters);
public delegate void VoidDelegate();

public class EventHandler
{

#region HeightTween
	private static List<IntDelegate> HeightTween_List;
	public static event IntDelegate HeightTween_Listener
    {
		add
		{
			if(value != null)
			{
				if(HeightTween_List == null)
				{
                    HeightTween_List = new List<IntDelegate>(1);
				}
                HeightTween_List.Add(value);
			}
		}
		remove
		{
			if(value != null)
			{
				for(int i = 0;i< HeightTween_List.Count;i++)
				{
					if(HeightTween_List[i] != null && HeightTween_List[i].Equals(value))
					{
                        HeightTween_List.RemoveAt(i);
						break;
					}
				}
			}
		}
	}
	public static void ScoreTween_Dispatch(params int[] parameters)
	{
		if(HeightTween_List == null || HeightTween_List.Count <= 0)
		{
			return;
		}
		for(int i = 0;i < HeightTween_List.Count;i++)
		{
            HeightTween_List[i]?.Invoke(parameters[i]);
		}
	}
#endregion
}