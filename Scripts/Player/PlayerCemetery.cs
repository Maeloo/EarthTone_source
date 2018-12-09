using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCemetery : MonoBehaviour {

	private List<Card> mCards;
	public	List<Card> cards {
		get { return mCards; }
	}

	void Start ( ) {
		mCards = new List<Card> ( );
	}

	public void sendToCemetery ( Card card ) {
		//card.moveTo ( transform.localPosition, CardData.CARD_SPEED * 5 );
		mCards.Add ( card );

		StartCoroutine ( addToCemetery ( card ) );
	}

	// TODO: régler probleme depth explosion et mettre delay
	IEnumerator addToCemetery ( Card card ) {
		yield return new WaitForSeconds ( 0f );
		card.transform.localPosition = transform.localPosition;
	}
	
}
