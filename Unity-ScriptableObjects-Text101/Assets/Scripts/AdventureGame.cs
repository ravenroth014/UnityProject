using UnityEngine;
using UnityEngine.UI;

public class AdventureGame : MonoBehaviour
{
	[SerializeField] private Text _textComponent;
	[SerializeField] private State _startingState;

	private State _state;
	
	// Use this for initialization
	void Start ()
	{
		_state = _startingState;
		_textComponent.text = _state.GetStateStory();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
