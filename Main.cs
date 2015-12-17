using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Main : MonoBehaviour {
	private GameObject[,] allCubes;
	private GameObject topCube;
	public GameObject blankCube;
	public GameObject deadCube;
	public GameObject blueCube;
	public GameObject greenCube;
	public GameObject yellowCube;
	public GameObject redCube;
	public GameObject magentaCube;
	public GameObject placingCube;
	public Material blankMaterial;
	public Material deadMaterial;
	public Material blueMaterial;
	public Material greenMaterial;
	public Material yellowMaterial;
	public Material redMaterial;
	public Material magentaMaterial;
	public Material blueActiveMaterial;
	public Material greenActiveMaterial;
	public Material yellowActiveMaterial;
	public Material redActiveMaterial;
	public Material magentaActiveMaterial;
	public Text scoreText;
	public Text gameText;
	List<int> cubePlaceXVal = new List<int> ();
	List<int> deadPlaceXVal = new List<int> ();
	List<int> deadPlaceYVal = new List<int> ();
	//need lists below??
	List<int> scoreSameXVal = new List<int> ();
	List<int> scoreSameYVal = new List<int> ();
	List<int> scoreDiffXVal = new List<int> ();
	List<int> scoreDiffYVal = new List<int> ();
	int score = 0;
	int gridWidth = 8;
	int gridHeight = 5;
	float timeToAct = 0.0f;
	float turnLength = 2.0f;
	float gameTime = 60.0f;
	int nextCubeRow = 0;
	bool cubePlaced = false;
	bool rowSelected = false;
	int randNum;
	int randLen;
	int randomX;
	Color nextCubeColor;
	int xAdd, yAdd;
	int deadX, deadY;
	int selectedCubeX, selectedCubeY;
	int activeCubeX, activeCubeY;
	bool foundActive, foundSelected;
	int diffX, diffY;
	bool theGameOver = false;

	void Start () {
		allCubes = new GameObject[gridWidth, gridHeight];
		for (int width = 0; width < gridWidth; width++){
			for (int height = 0; height < gridHeight; height++) {
				allCubes[width, height] = (GameObject) Instantiate(blankCube, new Vector3(width*1.5f - 5,height*1.5f - 2,2.5f), Quaternion.identity);
				allCubes[width, height].GetComponent<CubeBehaviour>().x = width;
				allCubes[width, height].GetComponent<CubeBehaviour>().y = height;
			}
		}
		timeToAct += turnLength;
		topCube = (GameObject) Instantiate(placingCube, new Vector3(-5f, 5.5f, 2.5f), Quaternion.identity);
		SelectPlacingColor ();
	}

	void Update () {
		if (theGameOver == false) {
			RowSelection ();
			if (cubePlaced == false && rowSelected == true) {
				PlaceCube ();
				rowSelected = false;
			}
			if (Time.time >= timeToAct) {
				if (cubePlaced == false) {
					RndmDeadCube ();
					score--;
				}
				cubePlaced = false;
				timeToAct += turnLength;
				//placingCube recreated with a new color
				topCube = (GameObject)Instantiate (placingCube, new Vector3 (-5f, 5.5f, 2.5f), Quaternion.identity);
				SelectPlacingColor ();
			}
			if (score < 0) {
				score = 0;
			}
			MoveCube ();
			ScoringSame ();
			ScoringDifferent ();
			scoreText.text = "Score: " + score.ToString ();
		}
	}

	void PlaceCube() {
		cubePlaceXVal.Clear ();
		for (int xVal = 0; xVal < gridWidth; xVal++) {
			 if (allCubes[xVal, nextCubeRow].GetComponent<CubeBehaviour>().cubeColor == Color.White){
				cubePlaceXVal.Add(xVal);
			}
		}
		if (cubePlaceXVal.Count == 0) {
			GameOver ();
		} else {
			//select a random number from array
			randLen = Random.Range (0, cubePlaceXVal.Count);
			randomX = cubePlaceXVal[randLen];
			//place cube in nextCubeRow, randomY
			Destroy (allCubes[randomX, nextCubeRow]); 
			Destroy (topCube);
			if (nextCubeColor == Color.Blue) {
				allCubes[randomX, nextCubeRow] = (GameObject) Instantiate(blueCube, new Vector3(randomX*1.5f - 5,nextCubeRow*1.5f - 2,2.5f), Quaternion.identity);
			} else if (nextCubeColor == Color.Green) {
				allCubes[randomX, nextCubeRow] = (GameObject) Instantiate(greenCube, new Vector3(randomX*1.5f - 5,nextCubeRow*1.5f - 2,2.5f), Quaternion.identity);
			} else if (nextCubeColor == Color.Yellow) {
				allCubes[randomX, nextCubeRow] = (GameObject) Instantiate(yellowCube, new Vector3(randomX*1.5f - 5,nextCubeRow*1.5f - 2,2.5f), Quaternion.identity);
			} else if (nextCubeColor == Color.Red) {
				allCubes[randomX, nextCubeRow] = (GameObject) Instantiate(redCube, new Vector3(randomX*1.5f - 5,nextCubeRow*1.5f - 2,2.5f), Quaternion.identity);
			} else if (nextCubeColor == Color.Magenta) {
				allCubes[randomX, nextCubeRow] = (GameObject) Instantiate(magentaCube, new Vector3(randomX*1.5f - 5,nextCubeRow*1.5f - 2,2.5f), Quaternion.identity);
			}
			allCubes[randomX, nextCubeRow].GetComponent<CubeBehaviour>().x = randomX;
			allCubes[randomX, nextCubeRow].GetComponent<CubeBehaviour>().y = nextCubeRow;
			cubePlaced = true;
		}
	}

	void MoveCube() { //I'm not very happy with this because it changes coordinates and aspects of cubes
					//instead of moving cubes, if I had more time to think about it I would try to fix it
		foundActive = false;
		foundSelected = false;
		//find active cube
		for (int width = 0; width < gridWidth; width++){
			for (int height = 0; height < gridHeight; height++) {
				if (allCubes[width, height].GetComponent<CubeBehaviour>().active == true) {
					activeCubeX = allCubes[width, height].GetComponent<CubeBehaviour>().x;
					activeCubeY = allCubes[width, height].GetComponent<CubeBehaviour>().y;
					//they are coming out as zero?
					foundActive = true;
				}
			}
		}
		//find selected cube
		for (int width = 0; width < gridWidth; width++){
			for (int height = 0; height < gridHeight; height++) {
				if (allCubes[width, height].GetComponent<CubeBehaviour>().selected == true) {
					selectedCubeX = allCubes[width, height].GetComponent<CubeBehaviour>().x;
					selectedCubeY = allCubes[width, height].GetComponent<CubeBehaviour>().y;
					foundSelected = true;
					allCubes[width, height].GetComponent<CubeBehaviour>().selected = false;
				}
			}
		}
		diffX = activeCubeX - selectedCubeX;
		diffY = activeCubeY - selectedCubeY;
		if (foundActive == false || foundSelected == false) {
			//do nothing
		} else if (Mathf.Abs (diffX) <= 1 && Mathf.Abs (diffY) <= 1) {
			//x and y of cubeSelected are either the same or off by 1 of activeCube
			//move active cube to this space, move white cube to prev active space
			allCubes[selectedCubeX, selectedCubeY].GetComponent<CubeBehaviour>().cubeColor = allCubes[activeCubeX, activeCubeY].GetComponent<CubeBehaviour>().cubeColor;
			allCubes[selectedCubeX, selectedCubeY].GetComponent<Renderer>().material = allCubes[activeCubeX, activeCubeY].GetComponent<Renderer>().material;
			allCubes[activeCubeX, activeCubeY].GetComponent<CubeBehaviour>().cubeColor = Color.White;
			allCubes[activeCubeX, activeCubeY].GetComponent<Renderer>().material = blankMaterial;
			//deactivate cube
			allCubes[activeCubeX, activeCubeY].GetComponent<CubeBehaviour>().active = false;
			allCubes[selectedCubeX, selectedCubeY].GetComponent<CubeBehaviour>().active = false;
			if (allCubes[selectedCubeX, selectedCubeY].GetComponent<CubeBehaviour>().cubeColor == Color.Blue) {
				allCubes[selectedCubeX, selectedCubeY].GetComponent<Renderer>().material = blueMaterial;
			} else if (allCubes[selectedCubeX, selectedCubeY].GetComponent<CubeBehaviour>().cubeColor == Color.Green) {
				allCubes[selectedCubeX, selectedCubeY].GetComponent<Renderer>().material = greenMaterial;
			} else if (allCubes[selectedCubeX, selectedCubeY].GetComponent<CubeBehaviour>().cubeColor == Color.Yellow) {
				allCubes[selectedCubeX, selectedCubeY].GetComponent<Renderer>().material = yellowMaterial;
			} else if (allCubes[selectedCubeX, selectedCubeY].GetComponent<CubeBehaviour>().cubeColor == Color.Red) {
				allCubes[selectedCubeX, selectedCubeY].GetComponent<Renderer>().material = redMaterial;
			} else if (allCubes[selectedCubeX, selectedCubeY].GetComponent<CubeBehaviour>().cubeColor == Color.Magenta) {
				allCubes[selectedCubeX, selectedCubeY].GetComponent<Renderer>().material = magentaMaterial;
			}
		} else {
			//do nothing
		}

	}

	void RowSelection () {
		for (int num = 1; num < 6; num++) {
			if (Input.GetKeyDown (num.ToString())) {
				nextCubeRow = num - 1;
				rowSelected = true;
			}
		}
	}

	void RndmDeadCube(){
		deadPlaceXVal.Clear ();
		deadPlaceYVal.Clear ();
		for (int width = 0; width < gridWidth; width++){
			for (int height = 0; height < gridHeight; height++) {
				if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor == Color.White) {
					xAdd = allCubes[width, height].GetComponent<CubeBehaviour>().x;
					yAdd = allCubes[width, height].GetComponent<CubeBehaviour>().y;
					deadPlaceXVal.Add (xAdd);
					deadPlaceYVal.Add (yAdd);
				}
			}
		}
		if (deadPlaceXVal.Count == 0) {
			GameOver ();
		} else {
			randLen = Random.Range (0, deadPlaceXVal.Count);
			deadX = deadPlaceXVal[randLen];
			deadY = deadPlaceYVal[randLen];

			Destroy (allCubes[deadX, deadY]);
			Destroy (topCube);
			allCubes[deadX, deadY] = (GameObject) Instantiate(deadCube, new Vector3(deadX*1.5f - 5,deadY*1.5f - 2,2.5f), Quaternion.identity);
		}
	}

	void SelectPlacingColor(){
		randNum = Random.Range(1, 6);
		if (randNum == 1) {
			topCube.GetComponent<CubeBehaviour>().cubeColor = Color.Blue;
			topCube.GetComponent<Renderer> ().material = blueMaterial;
			nextCubeColor = Color.Blue;
		} else if (randNum == 2) {
			topCube.GetComponent<CubeBehaviour>().cubeColor = Color.Green;
			topCube.GetComponent<Renderer> ().material = greenMaterial;
			nextCubeColor = Color.Green;
		} else if (randNum == 3) {
			topCube.GetComponent<CubeBehaviour>().cubeColor = Color.Yellow;
			topCube.GetComponent<Renderer> ().material = yellowMaterial;
			nextCubeColor = Color.Yellow;
		} else if (randNum == 4) {
			topCube.GetComponent<CubeBehaviour>().cubeColor = Color.Red;
			topCube.GetComponent<Renderer> ().material = redMaterial;
			nextCubeColor = Color.Red;
		} else if (randNum == 5) {
			topCube.GetComponent<CubeBehaviour>().cubeColor = Color.Magenta;
			topCube.GetComponent<Renderer> ().material = magentaMaterial;
			nextCubeColor = Color.Magenta;
		}

	}

	public void ClickCubes(GameObject clickedCube){
		if (clickedCube.GetComponent<CubeBehaviour>().isTopCube == true) {
			//do nothing
		} else if (clickedCube.GetComponent<CubeBehaviour>().active == true) {
			clickedCube.GetComponent<CubeBehaviour>().active = false;
			//change back
			if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.Blue) {
				clickedCube.GetComponent<Renderer>().material = blueMaterial;
			} else if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.Green) {
				clickedCube.GetComponent<Renderer>().material = greenMaterial;
			} else if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.Yellow) {
				clickedCube.GetComponent<Renderer>().material = yellowMaterial;
			} else if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.Red) {
				clickedCube.GetComponent<Renderer>().material = redMaterial;
			} else if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.Magenta) {
				clickedCube.GetComponent<Renderer>().material = magentaMaterial;
			}
		} else if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.White) {
				clickedCube.GetComponent<CubeBehaviour>().selected = true;
		} else if (clickedCube.GetComponent<CubeBehaviour>().cubeColor != Color.White && clickedCube.GetComponent<CubeBehaviour>().cubeColor != Color.Black) { 
			for (int width = 0; width < gridWidth; width++){
				for (int height = 0; height < gridHeight; height++) {
					if (allCubes[width, height].GetComponent<CubeBehaviour>().active == true) {
						allCubes[width, height].GetComponent<CubeBehaviour>().active = false;
						//change back
						if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor == Color.Blue) {
							allCubes[width, height].GetComponent<Renderer>().material = blueMaterial;
						} else if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor == Color.Green) {
							allCubes[width, height].GetComponent<Renderer>().material = greenMaterial;
						} else if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor == Color.Yellow) {
							allCubes[width, height].GetComponent<Renderer>().material = yellowMaterial;
						} else if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor == Color.Red) {
							allCubes[width, height].GetComponent<Renderer>().material = redMaterial;
						} else if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor == Color.Magenta) {
							allCubes[width, height].GetComponent<Renderer>().material = magentaMaterial;
						}
					}
				}
			}
			clickedCube.GetComponent<CubeBehaviour>().active = true;
			//change
			if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.Blue) {
				clickedCube.GetComponent<Renderer>().material = blueActiveMaterial;
			} else if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.Green) {
				clickedCube.GetComponent<Renderer>().material = greenActiveMaterial;
			} else if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.Yellow) {
				clickedCube.GetComponent<Renderer>().material = yellowActiveMaterial;
			} else if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.Red) {
				clickedCube.GetComponent<Renderer>().material = redActiveMaterial;
			} else if (clickedCube.GetComponent<CubeBehaviour>().cubeColor == Color.Magenta) {
				clickedCube.GetComponent<Renderer>().material = magentaActiveMaterial;
			}
		}
	}

	void ScoringSame() {
		//check all cubes for plus sign of same color
		for (int width = 1; width < (gridWidth - 1); width++){
			for (int height = 1; height < (gridHeight - 1); height++) {
				if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor != Color.Black && allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor != Color.White){
					if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor == allCubes[width, height - 1].GetComponent<CubeBehaviour>().cubeColor){
						if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor == allCubes[width, height + 1].GetComponent<CubeBehaviour>().cubeColor){
							if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor == allCubes[width - 1, height].GetComponent<CubeBehaviour>().cubeColor){
								if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor == allCubes[width + 1, height].GetComponent<CubeBehaviour>().cubeColor){
									//change cubes to black
									Destroy (allCubes[width, height]);
									Destroy (allCubes[width, height - 1]);
									Destroy (allCubes[width, height + 1]);
									Destroy (allCubes[width - 1, height]);
									Destroy (allCubes[width + 1, height]);
									allCubes[width, height] = (GameObject) Instantiate(deadCube, new Vector3(width*1.5f - 5,height*1.5f - 2,2.5f), Quaternion.identity);
									allCubes[width, height - 1] = (GameObject) Instantiate(deadCube, new Vector3(width*1.5f - 5,height*1.5f - 3,2.5f), Quaternion.identity);
									allCubes[width, height + 1] = (GameObject) Instantiate(deadCube, new Vector3(width*1.5f - 5,height*1.5f - 1,2.5f), Quaternion.identity);
									allCubes[width - 1, height] = (GameObject) Instantiate(deadCube, new Vector3(width*1.5f - 6,height*1.5f - 2,2.5f), Quaternion.identity);
									allCubes[width + 1, height] = (GameObject) Instantiate(deadCube, new Vector3(width*1.5f - 4,height*1.5f - 2,2.5f), Quaternion.identity);
									//add 10 to score
									score += 10;
								}
							}
						}
					}
				}
			}
		}
		
	}

	void ScoringDifferent() {
		//check all cubes for plus sign of all different colors
		for (int width = 1; width < (gridWidth -1); width++){
			for (int height = 1; height < (gridHeight -1); height++) {
				if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor != Color.Black && allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor != Color.White && allCubes[width, height - 1].GetComponent<CubeBehaviour>().cubeColor != Color.Black && allCubes[width, height - 1].GetComponent<CubeBehaviour>().cubeColor != Color.White && allCubes[width, height + 1].GetComponent<CubeBehaviour>().cubeColor != Color.Black && allCubes[width, height + 1].GetComponent<CubeBehaviour>().cubeColor != Color.White && allCubes[width - 1, height].GetComponent<CubeBehaviour>().cubeColor != Color.Black && allCubes[width - 1, height].GetComponent<CubeBehaviour>().cubeColor != Color.White && allCubes[width + 1, height].GetComponent<CubeBehaviour>().cubeColor != Color.Black && allCubes[width + 1, height].GetComponent<CubeBehaviour>().cubeColor != Color.White){
					if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor != allCubes[width, height - 1].GetComponent<CubeBehaviour>().cubeColor){
						if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor != allCubes[width, height + 1].GetComponent<CubeBehaviour>().cubeColor && allCubes[width, height - 1].GetComponent<CubeBehaviour>().cubeColor != allCubes[width, height + 1].GetComponent<CubeBehaviour>().cubeColor){
							if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor != allCubes[width - 1, height].GetComponent<CubeBehaviour>().cubeColor && allCubes[width, height - 1].GetComponent<CubeBehaviour>().cubeColor != allCubes[width - 1, height].GetComponent<CubeBehaviour>().cubeColor && allCubes[width, height + 1].GetComponent<CubeBehaviour>().cubeColor != allCubes[width - 1, height].GetComponent<CubeBehaviour>().cubeColor){
								if (allCubes[width, height].GetComponent<CubeBehaviour>().cubeColor != allCubes[width + 1, height].GetComponent<CubeBehaviour>().cubeColor && allCubes[width, height - 1].GetComponent<CubeBehaviour>().cubeColor != allCubes[width + 1, height].GetComponent<CubeBehaviour>().cubeColor && allCubes[width, height + 1].GetComponent<CubeBehaviour>().cubeColor != allCubes[width + 1, height].GetComponent<CubeBehaviour>().cubeColor && allCubes[width - 1, height].GetComponent<CubeBehaviour>().cubeColor != allCubes[width + 1, height].GetComponent<CubeBehaviour>().cubeColor){
									//change cubes to black
									Destroy (allCubes[width, height]);
									Destroy (allCubes[width, height - 1]);
									Destroy (allCubes[width, height + 1]);
									Destroy (allCubes[width - 1, height]);
									Destroy (allCubes[width + 1, height]);
									allCubes[width, height] = (GameObject) Instantiate(deadCube, new Vector3(width*1.5f - 5,height*1.5f - 2,2.5f), Quaternion.identity);
									allCubes[width, height - 1] = (GameObject) Instantiate(deadCube, new Vector3(width*1.5f - 5,height*1.5f - 3,2.5f), Quaternion.identity);
									allCubes[width, height + 1] = (GameObject) Instantiate(deadCube, new Vector3(width*1.5f - 5,height*1.5f - 1,2.5f), Quaternion.identity);
									allCubes[width - 1, height] = (GameObject) Instantiate(deadCube, new Vector3(width*1.5f - 6,height*1.5f - 2,2.5f), Quaternion.identity);
									allCubes[width + 1, height] = (GameObject) Instantiate(deadCube, new Vector3(width*1.5f - 4,height*1.5f - 2,2.5f), Quaternion.identity);
									//add 5 to score
									score += 5;
								}
							}
						}
					}
				}
			}
		}

	}

	void GameOver(){
		gameText.text = "Game Over!";
		scoreText.text = "Final score: " + score.ToString ();
		theGameOver = true;
	}

}
