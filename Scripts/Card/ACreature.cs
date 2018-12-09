using UnityEngine;
using System.Collections;

abstract public class ACreature : MonoBehaviour {

	abstract public void	attackTarget ( ACreature target, float delay );
	abstract public void	takeAttackDamage ( ACreature attacker, int damage );
	abstract public void	counterAttackerTarget ( ACreature target );
	abstract public void	takeCounterAttackDamage ( int damage );
	abstract public void	applyDamage ( );
	abstract public int		getOwnerID ( );
	abstract public bool	isOnBoard ( );

	virtual public string getAttackType ( ) {
		return GetComponent<AAttackComponent> ( ).getAttackType ( );
	}


}
