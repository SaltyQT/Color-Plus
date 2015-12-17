using UnityEngine;
using System.Collections;

public class CubeBehaviour : MonoBehaviour {
	Main theGameMain;
	public int x, y;
	public Color cubeColor;
	public bool active = false;
	public bool selected = false;
	public bool isTopCube = false;
	
	void Start () {
		theGameMain = GameObject.Find ("GameMain").GetComponent<Main> ();
	}
	
	void Update () {
		
	}
	
	void OnMouseDown(){
		theGameMain.ClickCubes (this.gameObject);
	}
	void OnMouseOver(){
	}
	void OnMouseExit(){
	}

}
