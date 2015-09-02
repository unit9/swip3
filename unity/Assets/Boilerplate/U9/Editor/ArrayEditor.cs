using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ArrayEditor {
	
	public delegate T ObjectEditDelegate<T>( T o );
	
	public static T[] DisplayArrayEditor<T>( T[] currentArray, ObjectEditDelegate<T> objectEditDelegate ) {
		
		if( currentArray == null ) {
			currentArray = new T[0];
		}
		List<T> tList = new List<T>();
		tList.AddRange( currentArray );
			
		EditorGUILayout.BeginVertical();
			
			
		
			for( int i = 0, ni = tList.Count ; i < ni ; i++ ) {	
				EditorGUILayout.BeginHorizontal();
					tList[i] = objectEditDelegate( tList[i] );
					if( GUILayout.Button("-") ) {
						tList.Remove( tList[i] );
						i--;
						ni--;
					}
				EditorGUILayout.EndHorizontal();
			}
		
			EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button("+") ) {
					tList.Add( default(T) );
				}
			EditorGUILayout.EndHorizontal();
			
		EditorGUILayout.EndVertical();
		
		return tList.ToArray();
	}
	
	public static T SelectObject<T>( T o ) where T : Object {
		return (T)EditorGUILayout.ObjectField( o, typeof(T), false );
	}
	
}
