using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	#region Singleton Stuff
	private static GameManager _instance = null;
	private static readonly object singletonLock = new object ( );
	#endregion

	public static GameManager getInstance ( ) {
		lock ( singletonLock ) {
			if ( _instance == null ) {
				_instance = ( GameManager ) GameObject.Find ( "GameManager" ).GetComponent<GameManager> ( );
				
				DontDestroyOnLoad ( _instance );
			}
			return _instance;
		}
	}

	private Player	mPlayerOne;
	private Player	mPlayerTwo;

	private float	mPlayerPlaying;
	public	float	currentPlayerID {
		get { return mPlayerPlaying; }
	}

	[SerializeField] private GameObject _restatButton;
	[SerializeField] private GameObject _youWin;
	[SerializeField] private GameObject _youLose;

	// On create
	void StartGame ( ) {
		int rand = ( int ) Mathf.Floor ( Random.value * 2 );

		mPlayerPlaying = rand == 0 ? mPlayerOne.ID_PLAYER : mPlayerTwo.ID_PLAYER;

		if ( mPlayerOne.ID_PLAYER == mPlayerPlaying ) {
			mPlayerOne.draftCards ( 1 );
		}
		else {
			mPlayerTwo.draftCards ( 1 );
		}
		
		switchTurn ( );
	}

	public Player getEnnemy ( Player self ) {
		return self == mPlayerOne ? mPlayerTwo : mPlayerOne;
	}

	public void registerPlayer ( Player player ) {
		if ( mPlayerOne == null ) {
			mPlayerOne = player;
		}
		else if ( mPlayerTwo == null ) {
			mPlayerTwo = player;
		}

		if ( mPlayerOne && mPlayerTwo ) {
			StartGame ( );
		}
	}

	public void switchTurn ( ) {

		if ( mPlayerOne.ID_PLAYER == mPlayerPlaying ) {
			mPlayerPlaying = mPlayerTwo.ID_PLAYER;

			mPlayerOne.EndTurn ( );
			mPlayerTwo.StartTurn ( );
		}
		else {
			mPlayerPlaying = mPlayerOne.ID_PLAYER;

			mPlayerTwo.EndTurn ( );
			mPlayerOne.StartTurn ( );		
		}
	}

	public void playerDead ( Player looser ) {
		mPlayerOne.gameObject.SetActive ( false );
		mPlayerTwo.gameObject.SetActive ( false );

		TweenScale tween;
		if ( looser.isAnAI ) {
			tween = _youWin.GetComponent<TweenScale> ( );
		}
		else {
			tween = _youLose.GetComponent<TweenScale> ( );
		}

		tween.method = UITweener.Method.BounceIn;
		tween.Play ( true );

		_restatButton.GetComponent<TweenScale> ( ).Play ( true );
	}

}
