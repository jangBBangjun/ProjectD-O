using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Sub_LoadingManager : MonoBehaviour
{
    [Header("로딩 % 텍스트 참조")]
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("로딩 속도 조절")]
    [Range(0.1f, 5f)]
    [SerializeField] private float progressSpeed = 1.0f;

    private static string nextScene;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string Scene_Name)
    {
        nextScene = Scene_Name;
        SceneManager.LoadScene("Sub_Loding_Scene");
    }

    IEnumerator LoadScene()
    {
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("Next scene name is not set. Cannot proceed.");
            yield break;
        }

        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float displayProgress = 0f;

        while (!op.isDone)
        {
            yield return null;

            // op.progress는 0 ~ 0.9까지 진행됨
            float targetProgress = (op.progress < 0.9f) ? op.progress : 1f;

            // MoveTowards로 일정 속도로 증가 (progressSpeed로 조절 가능)
            displayProgress = Mathf.MoveTowards(displayProgress, targetProgress, Time.deltaTime * progressSpeed);

            int percent = Mathf.RoundToInt(displayProgress * 100f);
            progressText.text = ".. " + percent + "<size=50%>%</size>";

            // 100% 도달 시 씬 전환
            if (displayProgress >= 1f)
            {
                op.allowSceneActivation = true;
                yield break;
            }
        }
    }
}