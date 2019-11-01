WeatherBeerReporter

API documentation:

Fetch weather report for given coordinates:
http://api.openweathermap.org/data/2.5/weather?lat=50&lon=4&APPID={key}

Rest is all done via commands / queue triggers and a framework for Azure Maps.
Callback is done via SAS token, doesn't require API.