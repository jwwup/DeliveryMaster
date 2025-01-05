using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Controlable controlTarget;

    //케릭터or탈것으로 컨트롤 할 오브젝트 변경
    public void ChangeControlTarget(Controlable origin, Controlable target)
    {
        StartCoroutine(MoveCameraArm(origin,target));
        controlTarget = target;
    }

    //카메라 암(카메라를 잡고 있는 오브젝트)의 위치와 부모 오브젝트를 변경
    public IEnumerator MoveCameraArm(Controlable origin, Controlable target)
    {
        if(origin.cameraArm != null)
        {
            var cameraArm = origin.cameraArm;
            Vector3 startPos = cameraArm.position;
            Quaternion startRot = cameraArm.rotation;
            float timer = 0f;
            while(timer<=1f)
            {
                yield return null;
                timer += Time.deltaTime*3f;
                cameraArm.position=Vector3.Lerp(startPos,target.cameraArmSocket.position,timer);
                cameraArm.rotation=Quaternion.Slerp(startRot,target.cameraArmSocket.rotation,timer);

            }
            
            cameraArm.SetParent(target.cameraArmSocket);
            target.cameraArm = cameraArm;
        }
    }

}
