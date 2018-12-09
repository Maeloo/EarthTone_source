using UnityEngine;
using System.Collections;

public class Card : ACreature {

	private string _ID_CARD;
	public string ID_CARD {
		get { return _ID_CARD; }
	}

	private CardData.TYPES mType;
	public CardData.TYPES type {
		get { return mType; }
	}

	private int mLife	= 0;
	public	int life {
		get { return mLife; }
	}

	private int mCost	= 0;
	public	int cost {
		get { return mCost; }
	}

	private int mAttack = 0;
	public	int attack {
		get { return mAttack; }
	}

	public GameObject Template;

	private float mWidth;
	public float width {
		get { return mWidth * Template.transform.localScale.x; }
	}

	private bool mPreviewDisplayed	= false;
	private bool mAuthPreview		= true;

	private bool mIsSleeping		= false;
	public	bool isSleeping {
		get { return mIsSleeping; }
		set { mIsSleeping =  value; }
	}

	private bool mAlreadyAttack		= false;
	public bool alreadyAttack {
		get { return mAlreadyAttack; }
		set { mAlreadyAttack = value; }
	}

	private bool mIsOnBoard			= false;
	public override bool isOnBoard ( ) {
		return mIsOnBoard;
	} 

	private Vector3 mFixPosition;
	private Vector3 mFixRotation;

	[HideInInspector]
	public string _ID_DRAFT;

	[HideInInspector]
	public int DEPTH;

	private Slot mSlot = null;
	public	bool slotSelected {
		get { return mSlot != null; }
	} 

	private PlayerDraft		mDraftManager		= null;
	private PlayerBoard		mBoardManager		= null;
	private PlayerCemetery	mCemeteryManager	= null;
	private Player			mPlayerManager		= null;

	[SerializeField] private GameObject _front;
	[SerializeField] private GameObject _back;
	[SerializeField] private GameObject _damage;
	[SerializeField] private GameObject _deathFX;

	private CardEventsTransmitter	mCET			= null;
	private UIDragObject			mDragComponent	= null;

	// Initialize
	public void Init ( ) {
		UISprite sprite	= Template.GetComponent<UISprite> ( );
		mWidth = sprite.width;

		CardEventsTransmitter.OnDragEnded	+= OnDragEnd;
		CardEventsTransmitter.OnDragStarted += OnDragStart;
		CardEventsTransmitter.OnOverred		+= OnHover;
		CardEventsTransmitter.OnPressed		+= OnPress;
	}

	public void Create ( string id, Hashtable pparams ) {
		_ID_CARD	= id;
		_ID_DRAFT	= "-2";

		mCET			= GetComponentInChildren<CardEventsTransmitter> ( );
		mDragComponent	= GetComponentInChildren<UIDragObject> ( );

		activeDrag ( false );
		mCET.transmitHoverEvents = false;

		switch ( ( string ) pparams["type"] ) {
			case "Monstre":
				mType = CardData.TYPES.MONSTRE;
				break;

			case "Sort":
				mType = CardData.TYPES.SORT;
				break;
		}

		mLife	= int.Parse ( ( string ) pparams["life"] );
		mCost	= int.Parse ( ( string ) pparams["cost"] );
		mAttack = int.Parse ( ( string ) pparams["attack"] );

		transform.Search ( "illustration" ).GetComponent<UISprite> ( ).spriteName	= ( string ) pparams["illustration"];
		transform.Search ( "name" ).GetComponent<UILabel> ( ).text					= ( string ) pparams["name"];
		transform.Search ( "cost" ).GetComponent<UILabel> ( ).text					= ( string ) pparams["cost"];
		transform.Search ( "attack" ).GetComponent<UILabel> ( ).text				= ( string ) pparams["attack"];
		transform.Search ( "life" ).GetComponent<UILabel> ( ).text					= ( string ) pparams["life"];
		transform.Search ( "description" ).GetComponent<UILabel> ( ).text			= ( string ) pparams["description"];

		gameObject.AddComponent<PhysicalAttack> ( );

		setDisplayFace ( false );
	}

	// On press
	void OnPress ( string id, bool isPressed ) {
		if (!mIsOnBoard || id != ID_CARD )
			return;

		if ( mIsSleeping ) {
			Debug.Log ( "Can't Attack : I'm Sleeping" );
			return;
		}

		if ( mAlreadyAttack ) {
			Debug.Log ( "Can't Attack : Already Attack" );
			return;
		}

		if ( isPressed ) {
			if ( GameManager.getInstance ( ).currentPlayerID == this.getOwnerID ( ) )
				mBoardManager.useArrowAt ( transform.position );
		}
		else {
			ACreature target = mBoardManager.getTarget ( );
			if ( target ) {
				if ( target.getOwnerID ( ) != this.getOwnerID ( ) && target.isOnBoard ( ) ) {
					attackTarget ( target, 0f );
				}
				else {
					Debug.Log ( "Invalid target" );
				}
				
			}
			mBoardManager.deactiveArrow ( );
		}
	}

	// On hover
	void OnHover ( string id, bool isOver ) {
		if ( _ID_DRAFT == "-1" || id != _ID_DRAFT )
			return;
		
		displayPreview ( isOver );	
	}

	// On drag end
	void OnDragEnd (string id ) {
		if ( mIsOnBoard || id != _ID_DRAFT ) 
			return;

		if ( mSlot ) {
			if ( mCost <= mPlayerManager.currentMana ) {
				placeCardOnBoard ( );
			}
			else {
				mDraftManager.addCardToDraft ( this );
				Debug.Log ( "Not enough mana" );
			}			
		}
		else {
			Debug.Log ( "Can't add card to board" );
			mDraftManager.addCardToDraft ( this );
		}		
	}

	// On drag start
	void OnDragStart ( string id ) {
		if ( id != _ID_DRAFT )
			return;

		_ID_DRAFT			= "-1";
		mPreviewDisplayed	= false;
		mAuthPreview		= false;

		mDraftManager.removeCardFromDraft ( this );
	}

	public override void attackTarget ( ACreature target, float delay = 0f ) {
		mAlreadyAttack = true;
		StartCoroutine ( _attackTarget ( target, delay ) );
	}
	private IEnumerator _attackTarget ( ACreature target, float delay ) {
		yield return new WaitForSeconds ( delay );
		
		GetComponent<AAttackComponent> ( ).attack ( target.gameObject );

		target.takeAttackDamage ( this, mAttack );
	}

	public override void counterAttackerTarget ( ACreature target ) {
		target.takeCounterAttackDamage ( mAttack );
	}

	public override void takeCounterAttackDamage ( int damage ) {
		if ( mLife - damage <= 0 )
			mSlot.isOccupied = false;

		mDamageTaken = damage;
	}

	private int mDamageTaken = 0;
	public override void takeAttackDamage ( ACreature attacker, int damage ) {
		if ( mLife - damage <= 0 )
			mSlot.isOccupied = false;

		mDamageTaken = damage;

		if ( attacker.getAttackType ( ) != RangedAttack.TYPE_ATTACK )
			counterAttackerTarget ( attacker );
	}

	public override void applyDamage ( ) {
		Debug.Log ( ID_CARD + " " + mLife + "hp take" + mDamageTaken + " damages" );
		mLife -= mDamageTaken;
		
		_damage.GetComponentInChildren<UILabel> ( ).text = mDamageTaken.ToString ( );

		TweenScale tween = null;
		if ( mDamageTaken != 0 ) {
			tween			= _damage.AddComponent<TweenScale> ( );
			tween.duration	= .5f;
			tween.from		= Vector3.zero;
			tween.to		= new Vector3 ( 1f, 1f, 1f );
			tween.method	= UITweener.Method.BounceIn;
			tween.Play ( true );
		}		

		transform.Search ( "life" ).GetComponent<UILabel> ( ).text = mLife.ToString ( );

		StartCoroutine ( callbackDamageApplied ( tween ) );

		mBoardManager.ClearTarget ( );
	}

	IEnumerator callbackDamageApplied ( TweenScale tween ) {
		yield return new WaitForSeconds ( 2f );

		if ( mDamageTaken != 0 )
			tween.Play ( false );

		if ( mLife <= 0 ) {
			//NGUITools.AddChild ( _deathFX );
			GameObject deathFX= ( GameObject ) Instantiate ( _deathFX );
			deathFX.transform.position = transform.position;
			deathFX.SetActive ( true );

			mBoardManager.removeFromBoard ( this );
			mCemeteryManager.sendToCemetery ( this );
		}

		mDamageTaken = 0;
	}

	public void forcePlaceOnBoard ( Slot slot ) {
		_ID_DRAFT			= "-1";
		mPreviewDisplayed	= false;
		mAuthPreview		= false;
		mSlot				= slot;

		mDraftManager.removeCardFromDraft ( this );

		placeCardOnBoard ( );
	}

	private void placeCardOnBoard ( ) {
		mBoardManager.addCardToBoard ( this );

		moveTo ( mSlot.transform.localPosition );
		mDraftManager.replaceCards ( );

		mIsOnBoard = mSlot.isOccupied = true;
		activeDrag ( false );

		mPlayerManager.notifyManaUse ( mCost );
	}

	public void activeOver ( bool value ) {
		mCET.transmitHoverEvents = value;
	}

	public void activeDrag ( bool value ) {
		mDragComponent.enabled = value;
		mCET.transmitDragEndEvents = value;
		mCET.transmitDragStartEvents = value;
	}

	public void notifyOpenSlot ( Slot slot ) {
		if ( !mIsOnBoard )
			mSlot = slot;
	}

	public void registerDraftManager ( PlayerDraft draftManager ) {
		mDraftManager = draftManager;
	}

	public void registerBoardManager ( PlayerBoard boardManager ) {
		mBoardManager = boardManager;
	}

	public void registerPlayerCemetery ( PlayerCemetery playerCemetery ) {
		mCemeteryManager = playerCemetery;
	}

	public void registerPlayerManager ( Player playerManager ) {
		mPlayerManager = playerManager;
	}

	public override int getOwnerID ( ) {
		return mPlayerManager.ID_PLAYER;
	}

	// Move the card to a position
	public void moveTo ( Vector3 position, float speed = -1f ) {
		mFixPosition = position;

		speed = speed == -1f ? CardData.CARD_SPEED : speed;

		float duration = Vector3.Distance ( transform.localPosition, position ) / speed;

		TweenPosition tween		= GetComponent<TweenPosition> ( );
		tween.from				= transform.localPosition;
		tween.to				= position;
		tween.duration			= duration;
		tween.ResetToBeginning ( );
		tween.Play ( true );
	}

	public void rotateTo ( Vector3 rotation ) {
		mFixRotation = rotation;

		TweenRotation tween = new TweenRotation ( );
		tween.from = transform.localRotation.eulerAngles;
		tween.to = rotation;
		tween.duration = .5f;
		tween.ResetToBeginning ( );
		tween.Play ( true );
	}

	public void authorizePreview ( ) {
		mAuthPreview = true;
	}

	public void setDisplayFace ( bool front ) {
		TweenRotation tween = GetComponent<TweenRotation> ( );
		tween.Play ( front );
		Invoke ( "switchFace", tween.duration / 2 );
	}

	private void switchFace ( ) {
		_front.SetActive ( !_front.activeSelf );
		_back.SetActive ( !_back.activeSelf );
	}

	void displayFullScreenInfos ( bool visible ) { 
	}

	// Display preview card
	void displayPreview ( bool visible ) {
		if ( visible ) {
			if ( !mAuthPreview )
				return;

			if ( !mPreviewDisplayed ) {
				mPreviewDisplayed	= true;
				
				Vector3 previewPos = transform.localPosition;
				previewPos.y += CardData.OVER_OFFSET;
				
				TweenPosition.Begin ( gameObject, .2f, previewPos );

				gameObject.GetComponentInChildren<UIPanel> ( ).depth = 8;
			}
		}
		else {
			gameObject.GetComponentInChildren<UIPanel> ( ).depth = DEPTH;
			TweenPosition.Begin ( gameObject, .16f, mFixPosition );
			mPreviewDisplayed = false;
		}
	}
	
}
