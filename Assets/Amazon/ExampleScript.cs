using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleScript : MonoBehaviour {

	/*
	 * 
	 * 
	 * Make sure you've registered yourself for the Amazon Product advertising API,
	 * I believe its through amazon web services (AWS). From the AWS console, you'll
	 * be able to create your Public/Secret keys (Access Keys). You'll also have to get 
	 * an Associate Tag, see:
	 * 
	 * http://docs.aws.amazon.com/AWSECommerceService/latest/DG/becomingAssociate.html
	 * 
	 * 
	 * Since things are asyncronous I'm using callback functions (delegates) 
	 * to do things when I get the results. You'll see that every Amazon method
	 * has one input and then ends with the name of the callback function to fire
	 * when the results come in. There's only three methods for searching defined in the 
	 * Amazon class. I've only included the ones that I wrote my project, but the 
	 * pattern is pretty much the same for all of them.
	 *
	 * The search method takes a search string and a callback.
	 * 
	 * The method will always pass an AmazonItem array and the delegate expects the callback
	 * function to look like:
	 * 
	 * void nameOfFunction (AmazonItem[] nameOfItemArray)
	 * 
	 * The findRelatedItems is almost the same as the search method
	 * but you'll pass an existing item ID (Asin) as the first parameter. The callback is
	 * the same structure.
	 * 
	 * The last method, getItemImages, will return an array of images of the products. 
	 * Like the findRelatedItems method, you pass an ID and a callback. This time the
	 * callback function looks like this:
	 * 
	 * void nameOfFunction (string[] nameOfStringArray)
	 * 
	 * You can always change or add things in the Amazon.cs script as well as see how the 
	 * requests are coming in. You can uncomment a Debug.Log in the response functions
	 * to inspect the XML response if you are looking for more than just the info that
	 * I've set up.
	 *
	 *
	 */


	Amazon amz;
	public string searchTerms = "coolest cooler";

	// Use this for initialization
	void Start () {
		amz = gameObject.GetComponent<Amazon> ();

		amz.search (searchTerms, getSearchResults); // newRequest(search term, callback function)
	}

	void getSearchResults(AmazonItem[] myResults){
		foreach(AmazonItem item in myResults){
			Debug.Log (item.Title + ", " + item.URL + ", " + item.Asin );
		}

		// Amazon's API wants the item's ASIN, so store it for the next two requests
		string itemID = myResults[0].Asin; 

		// search for related items to the itemID
		amz.findRelatedItems(itemID, getRelatedItemsResults);

		// grab image URLs from an item based off the itemID
		amz.getItemImages(itemID, getImageResults);

	}

	void getRelatedItemsResults(AmazonItem[] myRelatedItems){
		foreach(AmazonItem item in myRelatedItems){
			Debug.Log (item.Title + ", " + item.URL + ", " + item.Asin );
		}
	}
		
	void getImageResults(string[] myImageURLs){
		foreach(string url in myImageURLs){
			Debug.Log (url);
		}
	}
}
