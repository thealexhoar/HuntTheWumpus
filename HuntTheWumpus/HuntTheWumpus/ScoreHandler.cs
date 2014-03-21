using System;
using System.Collections.Generic;

namespace HuntTheWumpus
{
	class ScoreHandler
	{
		public uint Turns { get; private set; }
		public uint Score { get; private set; }

		public ScoreHandler()
		{
			Turns = 0;
			Score = 0;
		}

		public void OnTurn()
		{
			Turns++;
		}

		public void OnGameEnd()
		{
		}

		public void OnGameStart()
		{
		}

		public void OnTriviaAnswer(bool isRight)
		{
		}

		void OnSave(string filename)
		{
		}

		void OnLoad(string filename)	
		{
		}
	}
}
