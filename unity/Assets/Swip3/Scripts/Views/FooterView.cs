using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FooterView : MonoBehaviour {

	[SerializeField]
	GameObject _Scores, _CreatedBy;


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GameController.Inst.ECurrentView == GameController.View.Game
		   ||(GameController.Inst.EPrevView == GameController.View.Game && GameController.Inst.ECurrentView == GameController.View.Quit))
		{
			_Scores.SetActive(true);
			_CreatedBy.SetActive(false);
		}
		else
		{
			_Scores.SetActive(false);
			_CreatedBy.SetActive(true);
		}
	}
}

