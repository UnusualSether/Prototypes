using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

class ArchivedTechDemoCode
{

    //HandleInput();
    //TouchCamAtive();

    //Quaternion yamRotation = Quaternion.Euler(_initialPitch, _yam, 0f);

    //RotateCamera(yamRotation);


    public void TouchCamAtive()
    {


        //if (Input.touchCount > 0)
        //{
            //Touch touch = Input.GetTouch(0);
            //inputDelta = touch.deltaPosition * 0.1f;
            //condicional de pressionar
            //if (touch.phase == TouchPhase.Began)
            //{
                //_isTouching = true;
                //_touchTime = 0f;
            //}
            //if (_isTouching)
            //{
                //_touchTime += Time.deltaTime;
                //Debug.Log("Tempo pressionado:" + _touchTime.ToString("F2") + "s");
            //}
            //if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            //{
                //Debug.Log("Toque finalizado. Tempo total:" + _touchTime.ToString("F2") + "s");
                //_isTouching = false;
            //}

        }

        //definição de ativação de camera, controlle o time para aticar a camera.
        //if (_touchTime > 100f)
        //{
            //HandleInput();
        //}

    //}
    //public float GetCurretyTouchTime()
    //{
        //return _touchTime;
    //}

    //public void HandleInput()
   // {

        //_yam += inputDelta.x * sensitivity * Time.deltaTime;
        //_pitch -= inputDelta.y * sensitivity * Time.deltaTime;
        // não precisa desse codigo se for deixar o jogador modificar a pisção da camera
        //_yam = Mathf.Clamp(_yam, -_maxRotation, _maxRotation);
        // _pitch = Mathf.Clamp(_yam, -_maxRotation, _maxRotation);

    //}

    //void RotateCamera(Quaternion rotation)
    //{

        //Vector3 targetPositionWithHeight = _target.position + Vector3.up * _heightOffset;
        //Vector3 positionOffset = rotation * new Vector3(0, 0, -_distanceFromTarget);
        //transform.position = targetPositionWithHeight + positionOffset;
        //transform.rotation = rotation;
        /*
        Vector3 positionOffset = rotation * new Vector3(0, 0, -_distanceFromTarget);
        transform.position = _target.position + positionOffset;
        transform.rotation = rotation;
        */
    //}
}
