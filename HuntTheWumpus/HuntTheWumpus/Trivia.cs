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

        public static string[] TriviaGenerator(int numberOfQuestions)
        {
            // Generate numberOfQuestions amount of questions
            String[] questions = new string[numberOfQuestions];
            for (int i=0; i<numberOfQuestions;i++)
            {
                questions[i] = "question number: " + i;
            }
            return questions;
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
