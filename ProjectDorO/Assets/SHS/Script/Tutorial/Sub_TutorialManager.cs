using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sub_TutorialManager : MonoBehaviour
{
    [SerializeField] Sub_EntityManager entityManager;
    [SerializeField] private Sub_MiniMapInputReceiver miniMapInputReceiver;
    [SerializeField] private Sub_MiniMapManager miniMapManager;
    [SerializeField] private string nextSceneName;

    [Serializable]
    private class TutorialStep
    {
        public enum ActionType
        {
            Wait, cencer,
            LeftClick, RightClick,
            W, A, S, D, Q, E, X, M, Space,
            One, Two, Three, Four, Five,
            F, Enter
        }
        public ActionType actionType;
        public GameObject stepObject;

        public float endDelay;
    }

    [SerializeField] private TutorialStep[] tutorialSteps;
    private bool cencerCheck = false;

    void Start()
    {
        StartCoroutine(TutorialCoroutine());
    }
    private IEnumerator TutorialCoroutine()
    {
        Debug.Log("Æ©Åä¸®¾ó ½ÃÀÛ");
        miniMapInputReceiver.miniMapLock = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (TutorialStep step in tutorialSteps)
        {
            Debug.Log($"Æ©Åä¸®¾ó ½ºÅÜ ½ÃÀÛ: {step.actionType}");

            if (step.stepObject != null)
                step.stepObject.SetActive(true);

            switch (step.actionType)
            {
                case TutorialStep.ActionType.LeftClick:
                    yield return new WaitUntil(() => Mouse.current.leftButton.isPressed);
                    break;
                case TutorialStep.ActionType.RightClick:
                    yield return new WaitUntil(() => Mouse.current.rightButton.isPressed);
                    break;
                case TutorialStep.ActionType.W:
                    yield return new WaitUntil(() => Keyboard.current.wKey.isPressed);
                    break;
                case TutorialStep.ActionType.A:
                    yield return new WaitUntil(() => Keyboard.current.aKey.isPressed);
                    break;
                case TutorialStep.ActionType.S:
                    yield return new WaitUntil(() => Keyboard.current.sKey.isPressed);
                    break;
                case TutorialStep.ActionType.D:
                    yield return new WaitUntil(() => Keyboard.current.dKey.isPressed);
                    break;
                case TutorialStep.ActionType.Q:
                    yield return new WaitUntil(() => Keyboard.current.qKey.isPressed);
                    break;
                case TutorialStep.ActionType.E:
                    yield return new WaitUntil(() => Keyboard.current.eKey.isPressed);
                    break;
                case TutorialStep.ActionType.X:
                    yield return new WaitUntil(() => Keyboard.current.xKey.isPressed);
                    break;
                case TutorialStep.ActionType.M:
                    yield return new WaitUntil(() => Keyboard.current.mKey.isPressed);
                    miniMapManager.ToggleMap();
                    break;
                case TutorialStep.ActionType.Space:
                    yield return new WaitUntil(() => Keyboard.current.spaceKey.isPressed);
                    break;
                case TutorialStep.ActionType.One:
                    yield return new WaitUntil(() => Keyboard.current.digit1Key.isPressed && entityManager.PlayerGet(0) != null);
                    entityManager.PlayerSeclect(0);
                    break;
                case TutorialStep.ActionType.Two:
                    yield return new WaitUntil(() => Keyboard.current.digit2Key.isPressed && entityManager.PlayerGet(1) != null);
                    entityManager.PlayerSeclect(1);
                    break;
                case TutorialStep.ActionType.Three:
                    yield return new WaitUntil(() => Keyboard.current.digit3Key.isPressed && entityManager.PlayerGet(2) != null);
                    entityManager.PlayerSeclect(2);
                    break;
                case TutorialStep.ActionType.Four:
                    yield return new WaitUntil(() => Keyboard.current.digit4Key.isPressed && entityManager.PlayerGet(3) != null);
                    entityManager.PlayerSeclect(3);
                    break;
                case TutorialStep.ActionType.Five:
                    yield return new WaitUntil(() => Keyboard.current.digit5Key.isPressed && entityManager.PlayerGet(4) != null);
                    entityManager.PlayerSeclect(4);
                    break;
                case TutorialStep.ActionType.cencer:
                    yield return new WaitUntil(() => cencerCheck == true);
                    cencerCheck = false;
                    break;
                case TutorialStep.ActionType.F:
                    yield return new WaitUntil(() => Keyboard.current.fKey.isPressed);
                    break;
                case TutorialStep.ActionType.Enter:
                    yield return new WaitUntil(() => Keyboard.current.enterKey.isPressed);
                    break;
            }

            yield return new WaitForSeconds(step.endDelay);
            step.stepObject.SetActive(false);
        }

        miniMapInputReceiver.miniMapLock = false;
        Debug.Log("Æ©Åä¸®¾ó Á¾·á");

        Sub_LoadingManager.LoadScene(nextSceneName);
        yield return null;
    }

    public void CencerPressed()
    {
        cencerCheck = true;
    }
}
