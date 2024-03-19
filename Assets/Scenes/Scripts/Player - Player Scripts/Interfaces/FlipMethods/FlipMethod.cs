using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlipMethod : FlipMethodInterface
{
		public bool notEnteredYet = true;
		
		public FlipMethod()
		{
			notEnteredYet = true;
		}
		public virtual bool EntryCondition()
		{
			return true;
		}
		public virtual void EntryLogic()
		{
			//Debug.Log("Entered");
			return;
		}

		public virtual void ExitLogic()
		{
			//Debug.Log("Exited");
			notEnteredYet = true;
			return;
		}

		public virtual bool ExitCondition()
		{
			return true;
		}
		public virtual void UpdateLogic()
		{
			if (notEnteredYet)
			{
				if (EntryCondition() == false)
				{
					return;
				}
			}

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
		}

		public virtual void Logic()
		{

		}


		/**public class Sclass
		{
			public static void SayHello()
			{
				Debug.Log("Hello");
			}
		}*/

}

