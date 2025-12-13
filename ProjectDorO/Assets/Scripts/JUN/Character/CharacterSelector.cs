using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    [Header("Characters (1 → 5 순서)")]
    [SerializeField] private List<CharacterControlRoot> characters;

    [SerializeField] private PlayerInputReader inputReader;

    [SerializeField] private ThirdPersonCamera cameraController;

    private CharacterControlRoot current;

    private void Start()
    {
        // 처음 캐릭터 자동 선택 (0번)
        if (characters.Count > 0)
            SelectCharacter(0);
    }

    private void Update()
    {
        if (inputReader.Select1) SelectCharacter(0);
        if (inputReader.Select2) SelectCharacter(1);
        if (inputReader.Select3) SelectCharacter(2);
        if (inputReader.Select4) SelectCharacter(3);
        if (inputReader.Select5) SelectCharacter(4);
    }

    private void SelectCharacter(int index)
    {
        if (index < 0 || index >= characters.Count)
            return;

        CharacterControlRoot next = characters[index];
        if (next == null)
            return;

        if (current == next)
            return;

        if (current != null)
            current.DisableControl();

        current = next;
        current.EnableControl(inputReader);

        // 카메라 타겟 변경
        cameraController.SetTarget(current.transform);
    }

}
