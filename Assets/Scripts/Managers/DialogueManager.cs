using UnityEngine;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] private DialogueBox dialogueBox;
    [SerializeField] private DialogueBox.Talk[] talks;

    public void ActivateDialogue()
    {
        dialogueBox.Setup(talks);
    }
    

}
