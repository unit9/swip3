using UnityEngine;
using System.Collections.Generic;

public class ListDictionary<K,V> : Dictionary<K,List<V>>
{

	public ListDictionary() : base() {
		
	}
	
	public void Add( K key, V value ) {
		if( this.ContainsKey(key) ) {
			this[key].Add(value);
		} else {
			List<V> list = new List<V>();
			Add( key, list );
			list.Add( value );
		}
	}
	
	public void Remove( K key, V value ) {
		if( this.ContainsKey(key) ) {
			this[key].Remove(value);
			if( this[key].Count == 0 ) {
				this.Remove (key);
			}
		} else {
			Debug.LogWarning("Key : " + key + " was not in ListDictionary");
		}
	}
	
}

