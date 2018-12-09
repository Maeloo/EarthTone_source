using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBoard : MonoBehaviour {

	[SerializeField]
	private Arrow _arrow;

	private List<Slot> mSlots;
	public	List<Slot> Slots {
		get { return mSlots; }
	}

	private List<Card> mCards;
	public List<Card> cards {
		get { return mCards; }
	}

	void Start ( ) {
		mCards = new List<Card> ( );
		mSlots = new List<Slot> ( );
		foreach ( Transform child in transform ) {
			Slot slot =  child.GetComponent<Slot> ( );

			if ( slot ) {
				mSlots.Add ( slot );
			}
		}
	}

	public void initSlots ( int id ) {
		foreach ( Slot slot in mSlots ) {
			slot.setID ( id );
		}
	}

	public void wakeUpCreatures ( ) {
		foreach ( Card card in mCards ) {
			card.isSleeping		= false;
			card.alreadyAttack	= false;
		}
	}

	public void addCardToBoard ( Card card ) {
		mCards.Add ( card );

		card.isSleeping = true;
	}

	public void removeFromBoard ( Card card ) {
		mCards.Remove ( card );
	}

	public void useArrowAt ( Vector3 position ) {
		_arrow.transform.parent.position = position;
		_arrow.transform.parent.gameObject.SetActive ( true );
	}

	public void deactiveArrow ( ) {
		_arrow.transform.parent.gameObject.SetActive ( false );
	}

	public ACreature getTarget ( ) {
		return _arrow.targetedCard;
	}

	public void ClearTarget ( ) {
		_arrow.clearTarget ( );
	}
}
