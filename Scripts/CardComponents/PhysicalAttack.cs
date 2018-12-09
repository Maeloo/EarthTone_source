using UnityEngine;
using System.Collections;

class PhysicalAttack : AAttackComponent {

	public const string TYPE_ATTACK = "physical";

	private float	mAttackDuration = .3f;

	TweenPosition	mTweenAttack	= null;
	GameObject		mCurrentTarget	= null;

	public override void attack ( GameObject target ) {
		mTweenAttack			= gameObject.AddComponent<TweenPosition> ( );
		mTweenAttack.from		= transform.localPosition;
		mTweenAttack.to			= target.transform.localPosition;
		mTweenAttack.duration	= mAttackDuration;
		mTweenAttack.Play ( true );

		mCurrentTarget = target;

		Invoke ( "reverseTween", mAttackDuration );
	}

	void reverseTween ( ) {
		mTweenAttack.Play ( false );
		Invoke ( "applyDamage", mAttackDuration );
	}

	public override void applyDamage ( ) {
		Destroy ( mTweenAttack );

		mCurrentTarget.GetComponent<ACreature> ( ).applyDamage ( );
		gameObject.GetComponent<ACreature> ( ).applyDamage ( );
	}

	public override string getAttackType ( ) {
		return TYPE_ATTACK;
	}

	public override void displayAttackFx ( bool value ) {
	}
}
