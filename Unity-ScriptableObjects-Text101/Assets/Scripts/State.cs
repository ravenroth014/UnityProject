using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    // first parameter is limited line
    // second parameter is number of lines before scrollable
    [TextArea (14,10)] [SerializeField] private string _stroyText;
    [SerializeField] private State[] _nextStates;
    
    public string GetStateStory()
    {
        return _stroyText;
    }

    public State[] GetNextStates()
    {
        return _nextStates;
    }
}
