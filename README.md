# OpenWeatherMapSharp

.NET Standard library to wrap around `api.openweathermap.org` API.

## Sample Usage

```
OpenWeatherMapClient owmClient = new OpenWeatherMapClient(ACCESS_TOKEN);
WeatherResponse response = await owmClient.GetWeatherAsync(123456, "in", default, 3).ConfigureAwait(false);
owmClient.Dispose();
if(response == null){
	// Error occured.
}

// handle the WeatherResponse 

```

Or...

```
WeatherResponse response = default;
using(OpenWeatherMapClient owmClient = new OpenWeatherMapClient(ACCESS_TOKEN)){
	response = owmClient.GetWeather(123456, "in", 3);
}

if(response == null){
	// Error occured.
}

// handle the WeatherResponse 

```

## Dependencies
* Newtonsoft.Json - for parsing api response.

## License
The MIT License (MIT)

Copyright (c) 2020 ArunPrakashG

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
