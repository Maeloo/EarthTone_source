using UnityEngine;
using System.Collections;

public class Heros : ACreature {

	[SerializeField] private GameObject _damage;
	
	[SerializeField] int _minAttack = 1;
	[SerializeField] int _maxAttack = 3;

	private bool mAlreadyAttack = false;
	public	bool alreadAttacked {
		get { return mAlreadyAttack; }
	}

	private PlayerBoard mBoardManager;
	private Player		mPlayerManager;
	
	[SerializeField] private int _life = 20;

	[SerializeField] private int _cost = 2;
	public int cost {
		get { return _cost; }
	}

	public int attack {
		get { return ( int ) Mathf.Floor ( _minAttack + Random.value * _maxAttack ); }
	}

	public void initialize ( Player playerManager, PlayerBoard boardManager ) {
		mBoardManager	= boardManager;
		mPlayerManager	= playerManager;
	}

	public void wakeUp ( ) {
		mAlreadyAttack = false;

		notifyManaChange ( );
	}

	public void notifyManaChange ( ) {
		GetComponent<AAttackComponent> ( ).displayAttackFx ( _cost <= mPlayerManager.currentMana );
	}

	public override bool isOnBoard ( ) {
		return true;
	}

	// On press
	void OnPress ( bool isPressed ) {
		if ( mAlreadyAttack ) {
			Debug.Log ( "Can't Attack : Already Attack" );
			return;
		}

		if ( isPressed ) {
			if ( _cost <= mPlayerManager.currentMana ) {
				if ( GameManager.getInstance ( ).currentPlayerID == this.getOwnerID ( ) )
					mBoardManager.useArrowAt ( transform.position );
			}
			else {
				Debug.Log ( "Not enough mana" );
			}			
		}
		else {
			ACreature target = mBoardManager.getTarget ( );
			if ( target ) {
				if ( target.getOwnerID ( ) != this.getOwnerID ( ) && target.isOnBoard ( ) ) {
					attackTarget ( target, 0f );
				}
				else {
					Debug.Log ( "Invalid target " + target.getOwnerID ( ) + " " + this.getOwnerID ( ) );
				}
			}
			mBoardManager.deactiveArrow ( );
		}
	}

	public override int getOwnerID ( ) {
		return mPlayerManager.ID_PLAYER;
	}

	public override void attackTarget ( ACreature target, float delay = 0f ) {
		StartCoroutine ( _attackTarget ( target, delay ) );
	}
	private IEnumerator _attackTarget ( ACreature target, float delay ) {
		yield return new WaitForSeconds ( delay );

		GetComponent<AAttackComponent> ( ).attack ( target.gameObject );

		target.takeAttackDamage ( this, attack );

		mAlreadyAttack = true;
	}

	public override void counterAttackerTarget ( ACreature target ) {
		target.takeCounterAttackDamage ( attack );
	}

	public override void takeCounterAttackDamage ( int damage ) {
		mDamageTaken = damage;
	}

	private int mDamageTaken = 0;
	public override void takeAttackDamage ( ACreature attacker, int damage ) {
		mDamageTaken = damage;

		/*if ( attacker.getAttackType ( ) != RangedAttack.TYPE_ATTACK )
			counterAttackerTarget ( attacker );*/
	}

	public override void applyDamage ( ) {
		Debug.Log ( getOwnerID ( ) + " " + _life + "hp take" + mDamageTaken + " damages" );
		_life -= mDamageTaken;

		transform.Search ( "life_text" ).GetComponent<UILabel> ( ).text = _life.ToString ( );

		_damage.GetComponentInChildren<UILabel> ( ).text = mDamageTaken.ToString ( );

		TweenScale tween = null;
		if ( mDamageTaken != 0 ) {
			tween = _damage.AddComponent<TweenScale> ( );
			tween.duration = .5f;
			tween.from = Vector3.zero;
			tween.to = new Vector3 ( 1f, 1f, 1f );
			tween.method = UITweener.Method.BounceIn;
			tween.Play ( true );
		}

		StartCoroutine ( callbackDamageApplied ( tween ) );

		mBoardManager.ClearTarget ( );
	}

	IEnumerator callbackDamageApplied ( TweenScale tween ) {
		yield return new WaitForSeconds ( 2f );

		if ( mDamageTaken != 0 )
			tween.Play ( false );

		if ( _life <= 0 ) {
			GameManager.getInstance ( ).playerDead ( mPlayerManager );
		}

		mDamageTaken = 0;
	}

	public void updateMana ( ) {
		mPlayerManager.notifyManaUse ( _cost );
	}

}
