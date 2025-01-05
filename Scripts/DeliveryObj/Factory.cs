using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  abstract class Factory : MonoBehaviour
{
    // 상품을 생성해서 리턴하는 함수
    public abstract IProduct GetProduct();
}
