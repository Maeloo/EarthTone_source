using UnityEngine;
using System.Collections;

public class Slot : MonoBehaviour {

	private int mOwnerID;

	private bool mIsOccupied = false;
	public	bool isOccupied {
		get { return mIsOccupied;  }
		set { mIsOccupied = value; }
	}

	public void setID ( int id ) {
		mOwnerID = id;
	}

	// On trigger enter
	void OnTriggerEnter ( Collider collider ) {
		if ( !mIsOccupied ) {
			Card card = collider.GetComponentInParent<Card> ( );

			if ( card && card.getOwnerID ( ) == mOwnerID ) {
				card.notifyOpenSlot ( this );
			}
		}		
	}

	// On trigger stay
	void OnTriggerStay ( Collider collider ) {		
		if ( !mIsOccupied ) {
			Card card = collider.GetComponentInParent<Card> ( );

			if ( card && card.getOwnerID ( ) == mOwnerID && !card.slotSelected ) {
				card.notifyOpenSlot ( this );
			}
		}
	}

	// On trigger exit
	void OnTriggerExit ( Collider collider ) {
		if ( !mIsOccupied ) {
			Card card = collider.GetComponentInParent<Card> ( );

			if ( card ) {
				card.notifyOpenSlot ( null );
			}
		}
	}

}
