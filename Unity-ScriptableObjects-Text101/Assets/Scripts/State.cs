using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{
    // first parameter is limited line
    // second parameter is number of lines before scrollable
    [TextArea (14,10)] [SerializeField] private string _stroyText;

    public string GetStateStory()
    {
        return _stroyText;
    }
}
