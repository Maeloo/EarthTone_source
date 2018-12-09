using UnityEngine;
using System.Collections;

public class TurnButton : MonoBehaviour {

	private bool		isActive	= true;
	private UITweener	selfTween	= null;

	void Start ( ) {
		selfTween = GetComponent<UITweener> ( );
	}

	public void setActive ( bool value ) {
		isActive = value;
		GetComponent<CapsuleCollider> ( ).enabled = value;

		switchState ( );
	}

	private void switchState ( ) {
		selfTween.method = isActive ? UITweener.Method.BounceIn : UITweener.Method.BounceOut;
		selfTween.Play ( isActive );
	}

	void OnClick ( ) {
		GameManager.getInstance ( ).switchTurn ( );
		setActive ( false );
		switchState ( );
	}
}
