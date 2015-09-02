#pragma strict

public var InputUpdate : int = 0;
public var CurrInput : int;
public var PrefView : int = 1;
public var CurrView : int = 1;

function Start () {

}

function Update () {

}

function ReceiveInput(i: int)
{
	InputUpdate++;
	CurrInput = i;
}

function SwitchToView(i: int)
{
	PrefView = CurrView;
	CurrView = i;
}