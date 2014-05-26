using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace HuntTheWumpus
{
    public class Trivia
    {


        public bool isInitialized = false;
        List<Question> questions = new List<Question>();
        Queue<Question> q = new Queue<Question>();
       
        public struct Question
        {
            public string QuestionText; // Actual question text.
            public string[] Choices;    // Array of answers from which user can choose.
            public int Answer;          // Index of correct answer within Choices.
            public string formattedString;  // Formatted string to pass to game control 

        }

        public Trivia()
        {
            this.isInitialized = true;
            //this.questions = new Question[12];

        }

        public void triviaReader()
        {


            using (StreamReader sr = new StreamReader(@"C:\Computer Programming\Wumpus\Wumpus\Questions.txt"))
            {
                string line;
                Question question;

                // Read and display lines from the file until 
                // the end of the file is reached. 
                while ((line = sr.ReadLine()) != null)
                {
                    //// Skip this loop if the line is empty.
                    // if (line.Length == 0)
                    //    continue;

                    // Create a new question object.
                    question = new Question()
                    {

                        // Set the question text to the line just read.
                        QuestionText = line,
                        // Set the choices to an array containing the next 4 lines read from the file.
                        Choices = new string[]
                        { 
                         sr.ReadLine(), 
                         sr.ReadLine(),
                         sr.ReadLine(),
                         sr.ReadLine()
                        }
                    };

                    question.formattedString = question.QuestionText + "/n" + question.Choices[0] + "/n" + question.Choices[1 ] + "/n" + question.Choices[2] + "/n" + question.Choices[3]; 
                    // Initially set the correct answer to -1, which means that no choice marked as correct has yet been found.
                    question.Answer = -1;

                    // Check each choice to see if it begins with the '!' char (marked as correct).
                    for (int i = 0; i < 4; i++)
                    {
                        if (question.Choices[i].StartsWith("!"))
                        {
                            // Current choice is marked as correct. 
                            question.Choices[i] = question.Choices[i].Substring(1);
                            question.Answer = i;
                            break; // Stop looking through the choices.
                        }
                    }

                    

                    //// Check if none of the choices was marked as correct. If this is the case, throw an exception and then stop processing.
                    if (question.Answer == -1)
                    {
                        throw new InvalidOperationException("No correct answer was specified for the following question.\r\n\r\n" + question.QuestionText);
                    }

                    // Finally, add the question to the complete list of questions.
                    questions.Add(question);
                }
            }


            Console.ReadKey();
        }


        private bool isAllPicked(bool[] isPicked)
        {
            for (int i = 0; i < isPicked.Count(); i++)
            {
                if (isPicked[i] == false)
                {
                    return false;
                }

            }
            return true;
        }
        
       
        private void RandomizeQuestions()
        {
            q.Clear();
            bool[] isPicked = new bool[questions.Count()];
            Random random = new Random();
            while (isAllPicked(isPicked) == false)
            {
                int x = random.Next(0, questions.Count());
                if (isPicked[x] == false)
                {
                    isPicked[x] = true;
                    q.Enqueue(questions[x]);
                }

            }

        }

        public int numberOfQuestions = 1; 
       
        private Question[] CreateQuestionArray(int numberOfQuestions)
        {
           Question[] Questions = new Question[numberOfQuestions]; 

            for (int i = 0; i < numberOfQuestions + 1; i++)
            {
                if (q.Count() == 0)
                {
                    RandomizeQuestions();
                }

               Questions[i] = q.Dequeue();

            }

            return Questions;

        }

        public string[] SendQuestions(int numberOfquestions)
        {
           
            string[] questions = new string[numberOfQuestions]; 
            foreach(Question q in CreateQuestionArray(numberOfQuestions))
            {
                for (int i = 0; i< numberOfQuestions+1; i++)
                {
                    questions[i] = q.formattedString; 
                }
            }            

            return questions; 
        }


        public bool CheckAnswer(int userAnswer)
        {
            foreach (Question q in CreateQuestionArray(numberOfQuestions))
                if (q.Answer == userAnswer)
                    return true;

            return false;

        }

    }
}



