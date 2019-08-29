using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCamera : MonoBehaviour
{
    public float sensitivityHor = 125.0f; // Чувствительность мыши по горизонтали 
    public float sensitivityVert = 50.0f; //Чувствительность мыши по вертикали
    public float distance = 5.0f; //Расстояние камеры от объекта (в начальный момент)

    public float heightCamera = 2.0f; //Высота относительно персонажа

    public float yMinLimit = 0;//Минимальный угол поворота камеры по оси Y
    public float yMaxLimit = 50;//Максимальный угол поворота камеры по оси Y

    public float minDistance = 0.1f;//Минимальная дистанция между камерой и персонажем
    public float maxDistance = 10.0f;//Максимальная дистанция между камерой и персонажем

    public float zoomRote = 90.0f;//Скорость приближения или отдаления камеры

    private float xRot = 0.0f; //Угол поворота относительно оси Y (в плоскости X)
    private float yRot = 0.0f; //Угол поворота относительно оси X (в плоскости Y)

    public Transform target; //Объект за которым следим(Наш персонаж)

    void Start()
    {
        //переворачиваем углы 
        Vector3 angles = transform.eulerAngles;
        xRot = angles.y;
        yRot = angles.x;

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.freezeRotation = true; //Остановка камеры при столкновении с физическим объектом
        }
    }

    void LateUpdate()
    {
        if (target)
        {//Если персонаж выставлен во вкладке Inspector 
         //Меняем углы согласно положению мыши 
            xRot += Input.GetAxis("Mouse X") * sensitivityHor * 0.017f;
            yRot -= Input.GetAxis("Mouse Y") * sensitivityVert * 0.017f;

            //Меняем дистанцию до персонажа с помощью колесика мыши. 
            distance -= (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomRote;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            yRot = Mathf.Clamp(yRot, yMinLimit, yMaxLimit); //Вызов функции для ограничения углов поворота

            //Повернуть камеру согласно подсчитанным данным 
            Quaternion rotation = Quaternion.Euler(yRot, xRot, 0);
            transform.rotation = rotation;

            //Двигаем камеру и следим за персонажем 
            Vector3 position = rotation * new Vector3(0.0f, heightCamera, -distance) + target.position;
            transform.position = position;

            //Следующий код нужен, чтобы камера не заходила за объекты
            RaycastHit hit;
            Vector3 trueTargetPosition = target.transform.position;
            if (Physics.Linecast(trueTargetPosition, transform.position, out hit))
            {
                float tempDistance = Vector3.Distance(trueTargetPosition, hit.point) - 0.4f;
                if (tempDistance > 0.0f)
                {
                    position = target.position - (rotation * Vector3.forward * tempDistance + new Vector3(0, -heightCamera, 0));
                    transform.position = position;
                }
            }
        }
    }
}
