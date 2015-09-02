using System.Collections;

public interface ISerializable
{
	
	string ID { get; }
	Hashtable Serialize();	
	void Deserialize( Hashtable data );
	void Unload();

}

