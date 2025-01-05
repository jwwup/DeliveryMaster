using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProduct 
{
    // 상속받는 클래스마다 다르게 구현하도록
    public void Initialize();
}
