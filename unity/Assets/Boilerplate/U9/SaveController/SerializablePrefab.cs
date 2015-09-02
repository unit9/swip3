using UnityEngine;
using System.Collections;

public abstract class SerializablePrefab : MonoBehaviour, ISerializable
{
	

	const string prefabPathKey = "_PrefabPath";
	
	protected abstract string PrefabPath {
		get;
	}
	
	#region ISerializable implementation
	public virtual Hashtable Serialize ()
	{
		Hashtable t = new Hashtable ();
		t.Add (prefabPathKey, PrefabPath);
		return t;
	}

	public abstract void Deserialize (Hashtable data);

	public abstract string ID {
		get;
	}

	public abstract void Unload ();
	#endregion

	public static T Instantiate<T> ( Hashtable t ) where T : SerializablePrefab {

		T prefab = (T)Resources.Load ((string)t [prefabPathKey], typeof(T));
		if (prefab) {
			T instance = (T)Instantiate (prefab);
			return instance;
		} else {
			throw new UnityException (string.Format ("Couldn't load a {0} at {1}", typeof(T).ToString (), (string)t [prefabPathKey]));
		}
	}

	public static T InstantiateAndDeserialize<T> (Hashtable t) where T : SerializablePrefab
	{
		if (t.ContainsKey (prefabPathKey)) {
			T instance = Instantiate<T> (t);
			instance.Deserialize (t);
			return instance;
		} else {
			throw new UnityException ("Couldn't find a prefab path to instantiate from.");
		}
	}
}
