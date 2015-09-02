using UnityEngine;
using System.Collections.Generic;

public class UIPanelManager : MonoSingleton<UIPanelManager> {

	[SerializeField]
	UIPanel[] panelsToManage;
	
	Dictionary<string,UIPanel> panelDictionary;
	
	void Awake() {
		Instance = this;
		panelDictionary = new Dictionary<string, UIPanel>();
		foreach( UIPanel p in panelsToManage ) {
			panelDictionary.Add(p.name,p);
		}
	}
	
	public UIPanel GetPanel( string name ) {
		return panelDictionary[name];
	}
	
	public void PlaceInPanel( string panelName, Transform t ) {
		UIPanel p = GetPanel( panelName );
		Vector3 pos = t.position;
		pos.z = p.transform.position.z;		
		TransformUtils.SwitchParentAndMaintainScale( t, p.transform );
		t.position = pos;
	}
}
