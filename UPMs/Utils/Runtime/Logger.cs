using UnityEngine;

namespace nfynt.utils
{
	public class Logger
	{
		public static void Log(object message)
		{
			Debug.Log(message);
		}
		public static void Error(object message)
		{
			Debug.Log("<color=Red>" + message + "</color>");
		}
		public static void Warn(object message)
		{
			Debug.Log("<color=Yellow>" + message + "</color>");
		}
	}
}


/*
 __  _ _____   ____  _ _____  
|  \| | __\ `v' /  \| |_   _| 
| | ' | _| `. .'| | ' | | |   
|_|\__|_|   !_! |_|\__| |_|
 

*/