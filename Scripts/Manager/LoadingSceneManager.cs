using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadingSceneManager : Singleton<LoadingSceneManager>
{
    static string nextScene;
    [SerializeField]
    private Image progressBar; 
    [SerializeField]
    private CanvasGroup canvasGroup;

    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        // 씬 로딩이 끝나는 이벤트 등록
        SceneManager.sceneLoaded += Instance.OnSceneLoaded;
        nextScene=sceneName;
        Instance.StartCoroutine(Instance.LoadSceneProcess());
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == nextScene) 
        {
            StartCoroutine(Fade(false));
            //등록한 것들이 중첩되서 작동하지 않도록
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = 0f; // 초기 값 설정
        }
       
    }

    IEnumerator LoadSceneProcess()
    {
        progressBar.fillAmount = 0f;
        //코루틴 안에서 다른 코루틴을 시작하고 yield 리턴하면 코루틴 시작할때까지 대기함
        yield return StartCoroutine(Fade(true));

        //비동기로 씬을 부르기
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; //씬을 불러오는게 끝나도 잠시 대기

        float timer = 0f;
        while(!op.isDone)
        {
            yield return null;
            if(op.progress<0.9f) //씬 로딩이 진행중일 경우 진행 바를 맞춰 채움
            {
                progressBar.fillAmount = op.progress;
            }
            else
            {
                //씬 로딩이 완료된 경우에는 진행 바를 부드럽게 이동
                timer += Time.unscaledDeltaTime; //게임의 시간 흐름 속도와 상관없이 값을 올린다
                progressBar.fillAmount = Mathf.Lerp(0.9f,1f,timer);
                if(progressBar.fillAmount>=1f){
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }


    }

    //씬 전환 시 로딩 씬 캔버스의 투명도를 조절하며 페이드인,아웃
    private IEnumerator Fade(bool isFadeIn){
        float timer = 0f;
        while(timer<=1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime*3f;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f,1f,timer) : Mathf.Lerp(1f,0f,timer);

        }

        if(!isFadeIn)
        {
            gameObject.SetActive(false);
        }

    }

    


}
