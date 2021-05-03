using UnityEngine;

namespace Script.ObjectEngine
{
    public interface TurnInfluence
    {
        
//        bool TurnElement(Transform obj, int angle, float time, GameObject target)
//        {
//            float deltaAngle;
//            float countAngle = 0;
//
//            deltaAngle = angle * (Time.deltaTime / time);
//            if (angle > 0 && countAngle + deltaAngle > angle || angle < 0 && countAngle + deltaAngle < angle) // если мы уже достаточно повернули и в ту и в другую сторону
//            {
//                deltaAngle = angle - countAngle; // узнаем сколько нам не хватает на самом деле  
//                countAngle = angle;
//            }
//            else
//                countAngle += deltaAngle;
//            
//            obj.Rotate(target.transform.position, deltaAngle);
//            
//            if (angle > 0 && countAngle < angle || angle < 0 && countAngle > angle)
//                return false;
//            return true;
//        }
    }
}