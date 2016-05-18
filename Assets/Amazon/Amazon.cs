using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Xml;

public class Amazon : MonoBehaviour {


	// All of your KEYS, IDs, TAGs, etc....
	public string AWS_ACCESS_KEY_ID = "";
	public string AWS_SECRET_KEY    = "";
	public string ASSOCIATE_TAG	  	= "";
	public string DESTINATION 		= "ecs.amazonaws.com";
	public string NAMESPACE 		= "http://webservices.amazon.com/AWSECommerceService/2011-08-01";


	// Some delegates for returning search and image requests
	public delegate void AmazonDelegate (AmazonItem[] resultsDictionary);
	public delegate void AmazonImageDelegate (string[] imageURL);
	public AmazonDelegate amazonDelegate;
	public AmazonImageDelegate amazonImageDelegate;

	// amazon request signer object: from SignedRequestHelper.cs
	public SignedRequestHelper helper; 


	void Awake () {
		// Initialize the request helper
		helper = new SignedRequestHelper(AWS_ACCESS_KEY_ID, AWS_SECRET_KEY, ASSOCIATE_TAG, DESTINATION);
	}
		

	/*
	 * 
	 *  Search Functions
	 * 
	 */

	public void search(string keyword, AmazonDelegate callback){

		amazonDelegate = callback; // set the callback

		// build the request and init a new WWW object using the request string
		WWW searchRequest = new WWW(buildRequest("search", keyword));
		// send the request, the response will trigger searchResponse 
		StartCoroutine(searchResponse(searchRequest));
	}

	public void findRelatedItems(string itemID, AmazonDelegate callback){
		
		amazonDelegate = callback; // set the callback

		WWW relatedRequest = new WWW(buildRequest("related", itemID));
		StartCoroutine(searchResponse(relatedRequest));
	}


	IEnumerator searchResponse(WWW www){
		yield return www;

		// check for errors
		if (www.error == null)
		{

//			Debug.Log (www.text); // in case you want to see the raw xml

			// parse XML
			XmlNodeList resultASINs = parseXMLDocument(www.text, "ASIN"); // look for the Item IDs
			XmlNodeList resultTitles = parseXMLDocument(www.text,"Title"); // look for Titles
			XmlNodeList resultURLs = parseXMLDocument(www.text,"DetailPageURL"); // look for URLS

			// pick random item from search result
			int itemNumber = Random.Range(0,5);

			AmazonItem[] results = new AmazonItem[resultASINs.Count];

			for (int i = 0; i < resultASINs.Count; i++)
			{
				string id = "";
				string title = "";
				string url = "";
				if (resultASINs[i].InnerText.Length > 0)
					id = resultASINs[i].InnerText;
				if (resultTitles[i].InnerText.Length > 0)
					title = resultTitles[i].InnerText;
				if (resultURLs[i].InnerText.Length > 0)
					url = resultURLs[i].InnerText;	

				results[i] = new AmazonItem(id,title,url); // store everything in an AmazonItem Object
			}
				
			amazonDelegate (results); // pass the results to the callback function


		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}

	/*
	 * 
	 *  Requesting image URLs of an Item
	 * 
	 */

	public void getItemImages(string itemID, AmazonImageDelegate callback){
		
		amazonImageDelegate = callback; // set the callback

		// build the image request using the itemID 
		WWW imageRequest = new WWW(buildRequest("image", itemID));
		// send request, response triggers imageResponse
		StartCoroutine(imageResponse(imageRequest));

	}
		

	IEnumerator imageResponse(WWW www){
		yield return www;

		// check for errors
		if (www.error == null)
		{
//			Debug.Log (www.text); // in case you want to see the raw xml

			XmlNodeList resultURLs = parseXMLDocument(www.text, "LargeImage");

			string[] results = new string[resultURLs.Count];

			for (int i = 0; i < resultURLs.Count; i++) {
				results[i] = resultURLs [i].FirstChild.InnerText;
			}

			amazonImageDelegate (results);

		} else {
			Debug.Log("WWW Error: "+ www.error);
		} 
	}


	/*
	 * 
	 * Building the Search Request
	 * 
	 */

	string buildRequest(string type, string value){
		string req;

		// create dictionary for request

		IDictionary<string, string> r = new Dictionary<string, string>();
		r["Service"] = "AWSECommerceService";
		r["Version"] = "2011-08-01";

		// depending on type of request add different key pairs into dictionary
		if(type == "search"){
			r["Operation"] = "ItemSearch";
			r["SearchIndex"] = "All";
			r["Keywords"] = value;
			r["ResponseGroup"] = "Small";

		} else if(type == "image"){

			r["Operation"] = "ItemLookup";
			r["IdType"] = "ASIN";
			r["ItemId"] = value;
			r["ResponseGroup"] = "Images";

		} else if(type == "related"){

			r["Operation"] = "SimilarityLookup";
			r["ItemId"] = value;
			r["SimilarityType"] = "Random";
			r["ResponseGroup"] = "Small";

		} else {
			return req = "make sure you've selected a search type";
		}

		// sign the request via helper, see SignedRequestHelper.cs
		req = helper.Sign(r);

		return req;
	}




	/*
	 * 
	 * XML Parsing helper
	 * 
	 */

	XmlNodeList parseXMLDocument(string text, string lookupElement){
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(text);

		XmlNodeList errorMessageNodes = doc.GetElementsByTagName("Message", NAMESPACE);
		if (errorMessageNodes != null && errorMessageNodes.Count > 0)
		{
			string message = errorMessageNodes.Item(0).InnerText;

			Debug.Log("Error: " + message + " (but signature worked)");
		}


		XmlNodeList items = doc.GetElementsByTagName(lookupElement, NAMESPACE);
		return items;
	}

}

//
// Class for storing information about each Amazon Item
//

public class AmazonItem {

	public string Asin{ get; set;} // Item ID number, I decided to use Amazon's ASIN type of ID
	public string Title{ get; set;} // Name of Item
	public string URL { get; set;} // Image of item

	// Force init with ID number
	public AmazonItem(string _asin){
		Asin = _asin;
	}

	public AmazonItem(string _asin, string _title, string _url){
		Asin = _asin;
		Title = _title;
		URL = _url;
	}
}
