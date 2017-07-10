# Demos

**LUIS Demo Bot**

To get this sample working you need to create a LUIS app and populate your LUIS app ID and Subscription key in the class attribute within LUISDialog.cs.
You can create the LUIS app for the sample using the HR Demo App.json file by importing it into the LUIS dashboard. 

**QnAMaker Bot**

To use this sample, create a new QnAMaker service over at qnamaker.ai.
Once your service is published just populate the knowledge base ID and the subscription key in the QnADialog.cs class.

**Tranlsation Bot**

You need to register for the Translation API and create a key from within Azure.
Once you have your key, just update the URL endpoint for retrieving your token within TranslationService.cs

For any help with these samples just reach out to me on Twitter @garypretty or via my blog www.garypretty.co.uk
