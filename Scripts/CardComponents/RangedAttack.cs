using UnityEngine;
using System.Collections;

class RangedAttack : AAttackComponent {

	public const string TYPE_ATTACK = "ranged";

	public GameObject AttackFx;

	private float	mAttackDuration = .5f;
	private Vector3 mBasPos;

	TweenPosition	mTweenAttack	= null;
	GameObject		mCurrentTarget	= null;

	void Start ( ) {
		mBasPos = AttackFx.transform.position;
	}

	public override void attack ( GameObject target ) {
		mTweenAttack			= AttackFx.AddComponent<TweenPosition> ( );
		mTweenAttack.from		= AttackFx.transform.position;
		mTweenAttack.to			= target.transform.position;
		mTweenAttack.duration	= mAttackDuration;
		mTweenAttack.method		= UITweener.Method.EaseOut;
		mTweenAttack.Play ( true );

		mCurrentTarget = target;

		Invoke ( "applyDamage", mAttackDuration );
	}

	public override void applyDamage ( ) {
		mCurrentTarget.GetComponent<ACreature> ( ).applyDamage ( );
		gameObject.GetComponent<ACreature> ( ).applyDamage ( );

		AttackFx.transform.localPosition = mBasPos;

		displayAttackFx ( false );

		// Change for creature card
		gameObject.GetComponent<Heros> ( ).updateMana ( );
	}

	public override string getAttackType ( ) {
		return TYPE_ATTACK;
	}

	public override void displayAttackFx ( bool value ) {
		AttackFx.SetActive ( value );
	}

}
