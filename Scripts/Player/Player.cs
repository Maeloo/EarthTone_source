using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	[SerializeField] 
	private int _ID_PLAYER;
	public	int ID_PLAYER {
		get { return _ID_PLAYER;  }
	}

	[SerializeField]
	private bool _isAnAI = false;
	public bool isAnAI {
		get { return _isAnAI; }
	}

	private int mTurnNumber = 0;
	private int mMaxMana	= 10;
	private int mMana		= 0;
	public	int currentMana {
		get { return mMana; }
	}
	
	public int StartHandCards = 3;

	[SerializeField] private Heros			_heros;
	[SerializeField] private ManaManager	_manaManager;
	[SerializeField] private PlayerDeck		_deckManager;
	[SerializeField] private PlayerDraft	_draftManager;
	[SerializeField] private PlayerBoard	_boardManager;
	[SerializeField] private PlayerCemetery	_cemeteryManager;
	[SerializeField] private TurnButton		_turnButton;
	[SerializeField] private GameObject		_scroll;

	private AIBasic mAI = null;


	// On create
	void Start () {
		PlayerDeck.DeckReady += initCards;

		if ( _isAnAI )
			mAI = GetComponent<AIBasic> ( );

		_heros.initialize ( this, _boardManager );
	}

	// Initialise les cartes
	void initCards (int id ) {
		if ( id != ID_PLAYER )
			return ;

		PlayerDeck.DeckReady -= initCards;
		
		int nextId = 0;
		foreach ( Card card in _deckManager.cards ) {
			card.Init ( );
			card.registerDraftManager   ( _draftManager );
			card.registerBoardManager   ( _boardManager );
			card.registerPlayerCemetery ( _cemeteryManager );
			card.registerPlayerManager ( this );
			nextId++;
		}

		initDraft ( );
		initBoard ( );

		GameManager.getInstance ( ).registerPlayer ( this );
	}

	void initDraft ( ) {
		draftCards ( StartHandCards );
	}

	void initBoard ( ) {
		_boardManager.initSlots ( _ID_PLAYER );
	}

	public void draftCards ( int count ) {
		if ( _deckManager.cards.Count < count ) {
			count = _deckManager.cards.Count;
		}
		
		if ( count != 0 ) {
			_draftManager.addCardsToDraft ( _deckManager.cards.GetRange ( 0, count ) );
			_deckManager.cards.RemoveRange ( 0, count );
		}
		else {
			Debug.Log ( "No more cards in deck" );
		}	
	}

	public void notifyManaUse ( int mana ) {
		_manaManager.removeMana ( mana );

		mMana -= mana;

		_heros.notifyManaChange ( );
	}

	public void EndTurn ( ) {
		if ( !_isAnAI ) {
			_draftManager.activeDrag ( false );
		}
	}

	public void StartTurn ( ) {
		draftCards ( 1 );

		mTurnNumber++;

		mMana = mTurnNumber <= mMaxMana ? mTurnNumber : mMaxMana;
		_manaManager.activeMana ( mMana );

		_boardManager.wakeUpCreatures ( );
		_heros.wakeUp ( );

		if ( !_isAnAI ) {
			Invoke ( "playHuman", 1f );
		}
		else {
			Invoke ( "playAI", 1f );
		}
	}

	void playHuman ( ) {
		TweenScale tween =	_scroll.GetComponent<TweenScale> ( );
		tween.method = UITweener.Method.BounceIn;
		tween.Play ( true );

		Invoke ( "hideScroll", 2f );

		_draftManager.activeDrag ( true );
		_turnButton.setActive ( true );
	}

	void hideScroll ( ) {
		TweenScale tween =	_scroll.GetComponent<TweenScale> ( );
		tween.method = UITweener.Method.BounceOut;
		tween.Play ( false );
	}

	public List<Card> getBoard ( ) {
		return _boardManager.cards;
	}

	public Heros getHeros ( ) {
		return _heros;
	}

	void playAI ( ) {
		Player ennemy = GameManager.getInstance ( ).getEnnemy ( this );

		mAI.playWith ( _boardManager.Slots, _draftManager.Cards, mMana, this.getBoard ( ), this.getHeros ( ), ennemy.getBoard ( ), ennemy.getHeros ( ) );
	}
}
