
[System.Serializable]
public class DialogueNode : DialogueLine
{

    public DialogueNode(string sent, Character person, CharacterEmotion emote, DialogueChoice[] choicesList, bool firstName) {
        sentence = sent;
        character = person;
        emotion = emote;
        choices = choicesList;
        firstNameUsage = firstName;
    }

    public DialogueNode(DialogueNode node) {
        sentence = node.sentence;
        character = node.character;
        emotion = node.emotion;
        choices = node.choices;
        firstNameUsage = node.firstNameUsage;
    }

    public DialogueChoice[] choices = null;
}
