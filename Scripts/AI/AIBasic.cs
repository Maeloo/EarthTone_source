using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Player))]
public class AIBasic : MonoBehaviour {

	private int mMana;

	public void playWith ( 
		List<Slot> pslots, List<Card> pdraft, int pmana,
		List<Card> pSelfBoard, Heros pSelfHeros, List<Card> pEnnemyBoard, Heros pEnnemyHeros ) 
	{
		mMana = pmana;
		while ( !playCards ( pslots, pdraft ) ) { }
		StartCoroutine ( playAttack ( pSelfBoard, pSelfHeros, pEnnemyBoard, pEnnemyHeros ) );
	}

	public IEnumerator playAttack ( List<Card> pSelfBoard, Heros pSelfHeros, List<Card> pEnnemyBoard, Heros pEnnemyHeros ) {
		yield return new WaitForSeconds ( 1f );

		List<Card>	selfBoard	= pSelfBoard.Clone ( );
		List<Card>	ennemyBoard	= pEnnemyBoard.Clone ( );

		float delay = 0f;

		foreach ( Card creature in selfBoard ) {
			Card toRemove = null;
			foreach ( Card target in ennemyBoard ) {
				if ( !creature.isSleeping && creature.attack >= target.life ) {
					creature.attackTarget ( target, delay );
					toRemove = target;
					delay += .8f;
					break;
				}	
			}

			if ( !creature.isSleeping && !creature.alreadyAttack ) {
				creature.attackTarget ( pEnnemyHeros, delay );
				delay += .8f;
			}

			if ( toRemove )
				ennemyBoard.Remove ( toRemove );
		}

		if ( pSelfHeros.cost <= mMana ) {
			foreach ( Card target in ennemyBoard ) {
				if ( pSelfHeros.attack >= target.life ) {
					pSelfHeros.attackTarget ( target, delay );
					delay += .8f;
					break;
				}
			}

			if ( !pSelfHeros.alreadAttacked ) {
				pSelfHeros.attackTarget ( pEnnemyHeros, delay );
				delay += .8f;
			}
		}

		end ( delay );
	}

	private bool playCards ( List<Slot> pslots, List<Card> pdraft ) {
		List<Slot>	slots	= pslots.Clone ( );
		List<Card>	draft	= pdraft.Clone ( );

		draft = updateCards ( draft, mMana );

		if ( draft.Count == 0 ) {
			//end ( );
			return true;
		}

		slots = updateSlots ( slots );

		if ( slots.Count == 0 ) {
			//end ( );
			return true;
		}

		sortCards ( ref draft );

		int rand = ( int ) Mathf.Floor ( Random.value * slots.Count );

		Slot slot = slots[rand];
		Card card = draft[draft.Count - 1];

		card.setDisplayFace ( true );
		card.forcePlaceOnBoard ( slot );

		draft.Remove ( card );

		mMana -= card.cost;

		return playCards ( slots, draft );
	}


	List<Slot> updateSlots ( List<Slot> slots ) {
		List<Slot> forRemove = new List<Slot> ( );

		foreach ( Slot slot in slots ) {
			if ( slot.isOccupied )
				forRemove.Add ( slot );
		}

		foreach ( Slot slot in forRemove ) {
			slots.Remove ( slot );
		}

		return slots;
	}

	List<Card> updateCards ( List<Card> cards, int mana ) {
		List<Card> forRemove = new List<Card> ( );

		foreach ( Card card in cards ) {
			if ( card.cost > mana )
				forRemove.Add ( card );
		}

		foreach ( Card card in forRemove ) {
			cards.Remove ( card );
		}

		return cards;
	}

	void sortCards ( ref List<Card> cards ) {
		if ( cards.Count == 0 )
			return;

		Card temp = cards[0];

		for ( int i = 0; i < cards.Count; i++ ) {
			for ( int j = 0; j < cards.Count - 1; j++ ) {
				if ( cards[j].cost > cards[j + 1].cost ) {
					temp			= cards[j + 1];
					cards[j + 1]	= cards[j];
					cards[j]		= temp;
				}
			}
		}	
	}

	void end ( float delay ) {
		Invoke ( "turnOver", delay );	
	}

	void turnOver ( ) {
		GameManager.getInstance ( ).switchTurn ( );
	}

}

