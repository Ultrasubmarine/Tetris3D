using UnityEngine;
//using GooglePlayGames;
using UnityEngine.SocialPlatforms;

namespace Script.ObjectEngine
{
    public interface TurnInfluence
    {
        // Social.ReportScore(null, null, bool suc);
        //
        // Social.LoadScores("Leaderboard01", scores => {
        //     if (scores.Length > 0)
        //     {
        //         Debug.Log("Got " + scores.Length + " scores");
        //         string myScores = "Leaderboard:\n";
        //         foreach (IScore score in scores)
        //             myScores += "\t" + score.userID + " " + score.formattedValue + " " + score.date + "\n";
        //         Debug.Log(myScores);
        //     }
        //     else
        //         Debug.Log("No scores loaded");
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