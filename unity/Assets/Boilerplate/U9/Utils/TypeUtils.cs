using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TypeUtils
{

	public static List<TTo> CastList<TFrom,TTo> (List<TFrom> list) 
		where TTo : TFrom
		where TFrom : class
	{
		List<TTo> castedList = new List<TTo> ();
		foreach (TFrom obj in list) {
			TTo t = obj as TTo;
			if (t != null) {
				castedList.Add (t);
			}
		}
		return castedList;
	}

	public static TTo[] CastArray<TFrom,TTo> ( TFrom[] from ) 
		where TTo : TFrom
			where TFrom : class
	{
		TTo[] to = new TTo[from.Length];
		for (int i = 0; i < from.Length; i++) {
			TFrom obj = (TTo)from [i];
		}
		return to;
	}


}
