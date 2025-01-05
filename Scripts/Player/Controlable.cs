using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controlable : MonoBehaviour
{
    //플레이어가 조종할 오브젝트들의 기본 메서드 정의
    public Transform cameraArmSocket;
    public Transform cameraArm;

    public abstract void Move(Vector2 input);

    public abstract void Rotate(Vector2 input);

    public abstract void Interact();

    public abstract void Jump();

    public abstract void Action();
}
