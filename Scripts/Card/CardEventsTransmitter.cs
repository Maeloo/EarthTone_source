using UnityEngine;
using System.Collections;

public class CardEventsTransmitter : MonoBehaviour {

	public bool transmitPressEvents		= true;
	public bool transmitHoverEvents		= true;
	public bool transmitDragEndEvents	= true;
	public bool transmitDragStartEvents = true;

	public delegate void	OnDragEndEvent ( string id );
	public static	event	OnDragEndEvent OnDragEnded;

	public delegate void	OnDragStartEvent ( string id );
	public static	event	OnDragStartEvent OnDragStarted;

	public delegate void	OnHoverEvent ( string id, bool isOver );
	public static	event	OnHoverEvent OnOverred;

	public delegate void	OnPressEvent ( string id, bool isPressed );
	public static	event	OnPressEvent OnPressed;

	private Card self = null;

	void Start ( ) {
		self = GetComponentInParent<Card> ( );
	}

	// On Press
	void OnPress ( bool isPressed ) {
		if ( transmitPressEvents ) {
			OnPressed ( self.ID_CARD, isPressed );
		}		
	}

	// On hover
	void OnHover ( bool isOver ) {
		if ( transmitHoverEvents ) {
			OnOverred ( self._ID_DRAFT, isOver );
		}		
	}

	// On drag end
	void OnDragEnd ( ) {
		if ( transmitDragEndEvents ) {
			OnDragEnded ( self._ID_DRAFT );
		}		
	}

	// On drag start
	void OnDragStart ( ) {
		if ( transmitDragStartEvents ) {
			OnDragStarted ( self._ID_DRAFT );
		}		
	}

}
