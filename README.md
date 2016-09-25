# AppleProductAvailabilitySnooper
Checks availability of iPhone 7 (or any other product model) from the apple website. (currently for apple.es) but can be modified to use in other countries. 
Sends you an email if any of the requested models that are in the config file are available in the requested store and postal code.      

Usage

Edit the config file ("app.config") and enter the desired e-mail account details (along with server which is by default the Gmail SMTP server)

To allow this app to send e-mails through gmail, you have to edit your account security options to allow less secure applications to send e-mails. A google search will help you with this.

In the 'From' app setting key, set the from e-mail. In the destination, set the email(s) you want to notify, for multiple e-mails use ';'.

Password, your account password. Don't leave this application running in an untrusted computer. Or if you use any other e-mail provider that allows app specific passwords, 
then generate one for this app. 

The 'ProductModels' key is for the desired Apple products that you want to be notified about. (you can see the models in the request URL when manually checking the availability of any product in the Apple website.)

The 'StoreCode' is an optional field, you can see the Store codes by looking at the response JSON from the apple website using your browser's dev tools 

The 'Location' is for the PostalCode that you want to check availability on. 

The 'RequestUrl' key is used to make the request. You can get this URL also by looking at the request that your browser makes when you click on the 'Check availability' button on the website.

By default, the URL is set for the spanish store.  