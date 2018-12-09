using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class XmlDataParser  {

	static private string CARD_KEY = "cards";
	static private string DECK_KEY = "decks";

	#region Singleton Stuff
	private static XmlDataParser _instance = null;
	private static readonly object singletonLock = new object ( );
	#endregion

	public static XmlDataParser getInstance ( ) {
		lock ( singletonLock ) {
			if ( _instance == null ) {
				_instance = new XmlDataParser ( );
			}
			return _instance;
		}
	}
	
	private static Dictionary<string, XmlDocument>	xmlDocs;

	private XmlDataParser ( ) {
		xmlDocs = new Dictionary<string, XmlDocument> ( );
	}
	
	public void loadCardDescriptorXML ( string file ) {
		if ( !xmlDocs.ContainsKey ( CARD_KEY ) )
			xmlDocs.Add ( CARD_KEY, loadXML ( file ) );
	}

	public void loadDeckXML ( string file ) {
		if ( !xmlDocs.ContainsKey ( DECK_KEY ) )
			xmlDocs.Add ( DECK_KEY, loadXML ( file ) );
	}

	private XmlDocument loadXML ( string file ) {
		XmlDocument xmlDoc	= new XmlDocument ( );
		TextAsset textAsset = ( TextAsset ) Resources.Load ( file );
		xmlDoc.LoadXml ( textAsset.text );

		return xmlDoc;
	}

	public int getCardCount ( ) {
		if ( xmlDocs[CARD_KEY] != null ) {
			return xmlDocs[CARD_KEY].GetElementsByTagName ( "card" ).Count;
		}
		else {
			return -1;
		}		
	}

	public Hashtable getCardData ( string id ) {
		if ( xmlDocs[CARD_KEY] != null ) {
			Hashtable data = new Hashtable ( );
			foreach ( XmlNode node in xmlDocs[CARD_KEY].SelectSingleNode ( "/descriptor_root/card[@id='" + id + "']" ) ) {
				data[node.Name] = node.InnerText;
			}
			return data;
		}
		else {
			return null;
		}
	}

	public List<string> getDeck ( int id ) {
		if ( xmlDocs[DECK_KEY] != null ) {
			List<string> newDeck = new List<string> ( );
			foreach ( XmlNode deck in xmlDocs[DECK_KEY].SelectSingleNode ( "/decks_root/deck[@id='" + id + "']" ) )
			{
				foreach ( XmlNode card in deck ) {
					for ( int i = 0; i < int.Parse ( deck.SelectSingleNode ( "amount" ).InnerText ); ++i )
					{
						newDeck.Add ( deck.SelectSingleNode ( "id" ).InnerText );
					}
				}
			}
			return newDeck;
		}
		else {
			return null;
		}
	}
}
