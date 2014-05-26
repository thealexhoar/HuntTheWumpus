//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO; 

//namespace HuntTheWumpus
//{
//    class TriviaHints
//    {
//        public bool isInitialized = false; 
//        string[] hints;
//        Queue<string> h = new Queue<string>();

//        public TriviaHints()
//        {
//            this.isInitialized = true; 
//        }

//        public void triviaReader()
//        {


//            using (StreamReader sr = new StreamReader(@"C:\Computer Programming\Wumpus\Wumpus\Questions.txt"))
//            {
//                string line;
//                string hint;

//                // Read and display lines from the file until 
//                // the end of the file is reached. 
//                while ((line = sr.ReadLine()) != null)
//                {
//                    //// Skip this loop if the line is empty.
//                    if (line.Length == 0)
//                        continue;
////                    {
////                        // Set the question text to the line just read.
//                        hint = line;
//                    }
//                }
//            }
//        //}


        //    private bool isAllPicked(bool[] isPicked)
        //{
        //    for (int i = 0; i < isPicked.Count(); i++)
        //    {
        //        if (isPicked[i] == false)
        //        {
        //            return false;
        //        }

        //    }
        //    return true;
        //}
        
       
        //private void RandomizeHints()
        //{
        //    h.Clear();
        //    bool[] isPicked = new bool[hints.Count()];
        //    Random random = new Random();
        //    while (isAllPicked(isPicked) == false)
        //    {
        //        int x = random.Next(0, hints.Count());
        //        if (isPicked[x] == false)
        //        {
        //            isPicked[x] = true;
        //            q.Enqueue(questions[x]);
        //        }

        //    }

        //}

        //public int numberOfQuestions = 1; 
       
        //private string CreateQuestionArray(int numberOfQuestions)
        //{
        //   string[] hints = new string[1] ; 

        //    for (int i = 0; i < numberOfQuestions + 1; i++)
        //    {
        //        if (q.Count() == 0)
        //        {
        //            RandomizeQuestions();
        //        }

        //       Questions[i] = q.Dequeue();

        //    }

//        //    return Questions;

//        //}

//        //public string[] SendQuestions(int numberOfquestions)
//        //{
           
//        //    string[] questions = new string[numberOfQuestions]; 
//        //    foreach(Question q in CreateQuestionArray(numberOfQuestions))
//        //    {
//        //        for (int i = 0; i< numberOfQuestions+1; i++)
//        //        {
//        //            questions[i] = q.formattedString; 
//        //        }
//        //    }            

//            return questions; 
//        }
//        }

                    
//    }
//}
