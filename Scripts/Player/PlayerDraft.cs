using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDraft : MonoBehaviour {

	public int			MaxCards	= 6;
	public float		CardMargin	= 0f;
	public float		CardAngle	= 10f;
	public Transform	Anchor;

	private List<Card>	mDraft = new List<Card> ( );
	public	List<Card>	Cards {
		get { return mDraft; }
	}

	// Update les layers
	void updateLayer ( ) {
		for ( int i = 0; i < mDraft.Count; i++ ) {
			mDraft[i].GetComponentInChildren<UIPanel> ( ).depth = i;
		}
	}

	// Enlève la carte du draft
	public void removeCardFromDraft ( Card card ) {
		mDraft.Remove ( card );
	}

	public void addCardsToDraft ( List<Card> cards ) {
		foreach ( Card card in cards ) {
			mDraft.Add ( card );

			if ( !GetComponentInParent<Player> ( ).isAnAI ) {
				card.activeOver ( true );
				card.setDisplayFace ( true );
			}
		}
		replaceCards ( );
	}

	public void addCardToDraft ( Card card ) {
		mDraft.Add ( card );
		card.activeOver ( true );
		replaceCards ( );
	}

	public void activeDrag ( bool value ) {
		foreach ( Card card in mDraft ) {
			card.activeDrag ( value );
		}
	}

	// Replace les cartes dans la main du joueur
	public void replaceCards ( ) {
		float	widthSum	= getWidthSum ( );
		int		it			= 0;

		foreach ( Card card in mDraft ) {
			Vector3 newPos = Anchor.localPosition;
			newPos.x = ( -widthSum / 2 ) + it * ( card.width + CardMargin );

			Vector3 newAngle = Vector3.zero;
			newAngle.z = -( mDraft.Count * CardAngle ) / 2 + it * CardAngle;			

			card._ID_DRAFT	= GetComponentInParent<Player>().ID_PLAYER + "_" + it;
			card.DEPTH		= it;
			card.moveTo ( newPos );
			//card.rotateTo ( newAngle );
			
			it++;
		}

		updateLayer ( );
	}

	// Retourne la somme des largeur des cartes
	float getWidthSum ( ) {
		float widthSum = 0f;
		foreach ( Card card in mDraft ) {
			widthSum += ( card.width + CardMargin );
		}

		return widthSum;
	}
}
