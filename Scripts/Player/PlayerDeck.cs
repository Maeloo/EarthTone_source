using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDeck : MonoBehaviour {

	public int			MaxCards = 15;
	public GameObject	CardPrefab;
	public GameObject	Anchor;

	List<Card> mDeck;
	public List<Card> cards {
		get { return mDeck; }
	}

	public delegate void	OnDeckReadyEvent ( int id );
	public static	event	OnDeckReadyEvent DeckReady;

	private int ID_PLAYER;

	private int _idx = 0;

	void Start ( ) {
		XmlDataParser.getInstance ( ).loadCardDescriptorXML ( "CardsDescriptor" );
		XmlDataParser.getInstance ( ).loadDeckXML ( "Decks" );

		init ( );
	}

	private void init ( ) {
		mDeck					= new List<Card> ( MaxCards );
		List<string> newDeck	= XmlDataParser.getInstance ( ).getDeck ( 1 );

		ETTools.shuffleList ( newDeck );

		ID_PLAYER = GetComponentInParent<Player> ( ).ID_PLAYER;

		for ( int i = 1; i <= MaxCards; i++ ) {
			createCard ( newDeck[i] );
		}

		InvokeRepeating ( "ready", 0f, .1f );		
	}

	private void ready ( ) {
		if ( DeckReady != null ) {
			DeckReady ( ID_PLAYER );
			CancelInvoke ( "ready" );
		}
	}

	private void createCard ( string id ) {
		string nextId = ID_PLAYER + "_" + _idx + "x" + id;

		GameObject card = NGUITools.AddChild ( gameObject, CardPrefab );
		card.name = "card_" + nextId;
		card.transform.localPosition = Anchor.transform.localPosition;

		Card ccard = card.GetComponent<Card> ( );
		ccard.Create ( nextId, XmlDataParser.getInstance ( ).getCardData ( id ) );

		mDeck.Add ( ccard );

		_idx++;
	}
	
}
