using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component //제네릭 싱글톤 클래스 
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //오브젝트를 찾고 없을 경우에 생성
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    SetupInstance();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        RemoveDuplicates();
    }

    private static void SetupInstance()
    {
        //타입이름을 리소스 이름으로 설정 후 사용
        string resourceName = typeof(T).Name;
        instance = Instantiate(Resources.Load<T>(resourceName));
        DontDestroyOnLoad(instance.gameObject); //씬 전환시에도 파괴되지 않도록
    }

    //중복된 오브젝트들 파괴
    private void RemoveDuplicates()
    {
        if (instance == null)
        {
            instance = this as T;  // 현재 객체를 `T` 타입으로 캐스팅
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 오브젝트 파괴
        }
    }
}