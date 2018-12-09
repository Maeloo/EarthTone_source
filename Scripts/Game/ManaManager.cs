using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManaManager : MonoBehaviour {

	[SerializeField] private UILabel _manaInfo;

	private List<GameObject> mManas;

	private int mActiveManas = 0;


	// On create
	void Start () {
		mManas = new List<GameObject> ( );

		foreach ( Transform child in transform ) {
			mManas.Add ( child.gameObject );
		}
	}

	// Active un nombre total de mana
	public void activeMana ( int count ) {
		mActiveManas = count;
		for ( int i = 0; i < count; i++ ) {
			TweenColor tween = mManas[i].GetComponent<TweenColor> ( );
			tween.delay = i * .2f;
			tween.Play ( true );
		}

		updateManaInfos ( );
	}

	// Enleve un nombre de mana
	public void removeMana ( int count ) {
		for ( int i = mActiveManas - 1; i > mActiveManas - count - 1; i-- ) {
			TweenColor tween = mManas[i].GetComponent<TweenColor> ( );
			tween.delay = ( mActiveManas - 1 - i ) * tween.duration;
			tween.Play ( false );
		}
		mActiveManas -= count;

		updateManaInfos ( );
	}

	private void updateManaInfos ( ) {
		if ( _manaInfo != null ) {
			_manaInfo.text = mActiveManas.ToString ( ) + " / 10";
		}
	}
}
