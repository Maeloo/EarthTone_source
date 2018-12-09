using UnityEngine;
using System.Collections;

abstract class AAttackComponent : MonoBehaviour {

	abstract public void	attack ( GameObject target );
	abstract public void	applyDamage ( );
	abstract public string	getAttackType ( );
	abstract public void	displayAttackFx ( bool value );

}
