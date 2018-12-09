using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {

	public static Transform Search ( this Transform target, string name ) {
		if ( target.name == name )
			return target;

		int c =  target.childCount;
		for ( int i = 0; i < c; ++i ) {
			var result =  Search ( target.GetChild ( i ), name );
			if ( result != null )
				return result;
		}

		return null;
	}

	public static void UniformScale ( this Transform target, float scale ) {
		target.localScale = new Vector3 ( scale, scale, scale );
	}


	public static List<T> Clone<T> ( this List<T> listToClone ) {
		List<T> list = new List<T> ( );
		foreach ( T item in listToClone ) {
			list.Add (item );
		}
		return list;
	}
}
