
[System.Serializable]
public class DialogueNode : DialogueLine
{

    public DialogueNode(string sent, Character person, CharacterEmotion emote, DialogueChoice[] choicesList) {
        sentence = sent;
        character = person;
        emotion = emote;
        choices = choicesList;
    }

    public DialogueNode(DialogueNode node) {
        sentence = node.sentence;
        character = node.character;
        emotion = node.emotion;
        choices = node.choices;
    }

    public DialogueChoice[] choices = null;
}
