using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ETTools {

	public static void shuffleList ( List<string> list ) {
		if ( list.Count > 1 )
		{
			int i = list.Count - 1;
			while ( i > 0 ) {
				int s		= ( int ) Mathf.Floor ( Random.value * ( list.Count ) );
				var temp	= list[s];

				list[s] = list[i];
				list[i] = temp;
				i--;
			}
		}
	}

}
