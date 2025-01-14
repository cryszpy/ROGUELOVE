using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private Slider slider;

    [SerializeField] private float loadTime = 1f;

    private static int start = -1;
    public static void StartLeaf(int index) {
        start = index;
        GameStateManager.transitionPauseLock = true;
    }

    private static bool end;
    public static void EndLeaf(bool condition) {
        end = condition;
    }

    private static int loading;
    public static void SetLoading(int num) {
        loading = num;
    }

    private static bool loadingBarEnable;
    public static void SetLoadingBar(bool condition) {
        loadingBarEnable = condition;
    }

    public static bool letterboxManualControl;

    void Start() {
        if (loadingBarEnable && GameStateManager.GetStage() == 0) {
            SetLoadingBar(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (start != -1) {
            StartCoroutine(LoadLevel(start));
            start = -1;
        }
        if (end == true) {
            end = false;
            EndLoad();
        }
        if (loadingBarEnable) {
            loadingScreen.SetActive(true);
        } else {
            loadingScreen.SetActive(false);
        }
    }

    public void EndLoad() {
        SetLoadingBar(false);
        animator.SetTrigger("LightLeakEnd");
    }

    public IEnumerator LoadLevel(int index) {
        Time.timeScale = 1f;
        animator.SetTrigger("LightLeakStart");
        if (GameStateManager.dialogueManager.player) {
            GameStateManager.dialogueManager.player.enabled = false;
            GameStateManager.dialogueManager.player.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(loadTime);

        SetLoadingBar(true);

        SceneManager.LoadSceneAsync(index);

        /*while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //Debug.Log(progress);

            //slider.value = progress;

            yield return null;
        }*/

        if (index == 0) {
            animator.SetTrigger("LightLeakEnd");
            SetLoadingBar(false);
            yield return null;
        }
    }
}
