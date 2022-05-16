using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.GameLogic
{

    public class LineBanManager: MonoBehaviour
    {
        public bool isBan;
        
        public int amountForBan; // if line generated so many times 
        public int currentAmountForBan;
    
        //step after ban
        public int freezeSteps;
        public int currentFreeze;

        public LineBanManager(bool isBan, int amountForBan, int freezeSteps)
        {
            this.isBan = isBan;
            
            this.amountForBan = amountForBan;
            this.freezeSteps = freezeSteps;
            
            this.currentFreeze = 0;
            currentAmountForBan = 0;
        }

        /// <summary><para> Checking are we can create this element by banLine rules</para></summary>
        public bool CanCreateElement(List<Vector3Int> blocksPositions)
        {
            if (isLineElement(blocksPositions))
            {
                if (isBan)
                {
                    return false;
                }
                else
                {
                    IncrementLine();
                }
            }
            else
            {
                NotLine();
            }
            return true;
        }
        
        private bool isLineElement(List<Vector3Int> blocksPositions)
        {
            if (blocksPositions.Count != 3)
                return false;
      
            for (int i = 0; i < blocksPositions.Count-1; i++)
            {
                if (blocksPositions[i].x != blocksPositions[i + 1].x || blocksPositions[i].z != blocksPositions[i + 1].z)
                    return false;
            }
            return true;
        }
        
        private void IncrementFreezeStep()
        {
            if (currentFreeze + 1 == freezeSteps)
            {
                isBan = false;
                currentFreeze = 0;
            }
            else
            {
                currentFreeze++; 
            }
        }

        private void IncrementLine()
        {
            if (currentAmountForBan + 1 == amountForBan)
            {
                isBan = true;
                currentAmountForBan = 0;
            }
            else
            {
                currentAmountForBan++;
            }
        }

        private void ClearLine()
        {
            this.currentAmountForBan = 0;
        }

        private void NotLine()
        {
            if (isBan)
                IncrementFreezeStep();
            else
                ClearLine();
        }
    }
}