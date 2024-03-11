using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipMethod : FlipMethodInterface
{
		public bool notEnteredYet = true;
		public FlipMethod()
		{
			notEnteredYet = true;
		}
		public void EntryLogic()
		{
			Debug.Log("Entered");
			return;
		}

		public void ExitLogic()
		{
			Debug.Log("Exited");
			return;
		}

		public bool ExitCondition()
		{
			return true;
		}

		public void UpdateLogic()
		{
			if (notEnteredYet)
			{
				EntryLogic();
				notEnteredYet = false;
			}
			Logic();
			notEnteredYet = ExitCondition();
			if (notEnteredYet)
			{
				ExitLogic();
			}
			return;
		}

		public void Logic()
		{
			return;
		}
	
	// a nested namespace

		public class Sclass
		{
			public static void SayHello()
			{
				Debug.Log("Hello");
			}
		}

}

