using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public PlayerInputReader inputReader;

    private CharacterControlRoot current;

    private void Update()
    {
        if (inputReader.Select1) SelectCharacter("Player1");
        if (inputReader.Select2) SelectCharacter("Player2");
        if (inputReader.Select3) SelectCharacter("Player3");
        if (inputReader.Select4) SelectCharacter("Player4");
        if (inputReader.Select5) SelectCharacter("Player5");
    }

    private void SelectCharacter(string tag)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        if (obj == null) return;

        CharacterControlRoot root = obj.GetComponent<CharacterControlRoot>();

        if (current != null)
            current.DisableControl();

        current = root;
        current.EnableControl(inputReader);

        Debug.Log($"{tag} 선택 완료");
    }
}
