using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus
{
    public class Trivia
    {
        public bool isInitialized = false;

        public Trivia() {
            this.isInitialized = true;
        }

        public void TriviaGenerator(int numberOfQuestions = 0)
        {
            
        }
        public int ReceiveHazardStatus(bool IsHazard,  string hazardType)
        {
            int numberOfQuestions = 1; 
            return numberOfQuestions; 
        }

        public int CheckAnswer(bool isAnsweredCorrectly)
        {
            
            return 1; 
        }



    }
}
