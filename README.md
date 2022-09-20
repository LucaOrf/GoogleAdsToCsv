# GoogleAdsToCsv

This is a simple exporter of Google Ads Query to CSV
This has created starting from the example in the official library directory: https://github.com/googleads/google-ads-dotnet

## How to use it
First of all modify the app.config with your values, those are the same as per the offical library linked before, you can also find the specific information of those fields in the app.config

You will need to change those config to start:
DeveloperToken
OAuth2ClientId
OAuth2ClientSecret
OAuth2RefreshToken

LoginCustomerId (if you are using a manager account)

After everything is setup you can launch the application, write your query in the big textbox, the customer id in the smaller textbox and press EXECUTE QUERY
After that you will be prompt for where to save the result and it will be created a CSV with your results and the field specified in the query
