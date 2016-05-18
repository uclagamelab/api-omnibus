# API OMNIBUS

A collection of scripts and objects to assist with connecting Unity3D to various APIs.

Currently supported: Amazon Product Advertising API

## Installation

You can clone this entire Unity project or simply grab individual folders from within the Assets folder. There are also API specific unity packages that you can download.

### Amazon Product API

* Register with Amazon to receive API keys and an Associate Tag
* Make sure you have the scripts: `Amazon.cs` and `SignedRequestHelper.cs`
* `ExampleScript.cs` provides an example of how you can use the Amazon class.

If you have the scripts in your project and this shows up in the console:
> The type or namespace name `Web' does not exist in the namespace 'System'. Are you missing an assembly reference?
you'll need to enable the full .Net 2.0 API. Go to `Edit > Project Settings > Player` and in the inspector change the Api Compatibility level from _.Net 2.0 Subset_ to _.Net 2.0_.

## Usage

### Amazon Product API

* Add the `Amazon.cs` script to a new or existing Game Object
* Fill in the fields with your public key (AWS_ACCESS_KEY_ID), private key (AWS_SECRET_KEY) and associate tag.
* Create a new script within the same Game Object as `Amazon.cs`. Alternatively add the `ExampleScript.cs` file to your object and modify to suit your needs.

Please refer to the `ExampleScript.cs` file for more detailed usage
