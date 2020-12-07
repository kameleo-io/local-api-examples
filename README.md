# Kameleo Local API Example Codes
[Kameleo](https://kameleo.io) is a complete and integrated solution for browser fingerprinting protection, and also for easy browser automation using W3C WebDriver. This repository shows useful and easy to understand examples written in Node.js and .NET Core (C#) about web scraping and automated browsing with Kameleo Client.
# Features
- Protection from Selenium/WebDriver detection
- Start unlimited number of profiles with different browser fingerprints
- Use authenticated HTTP/SOCKS/SSH proxies in browsers
- Create isolated browsing environments simultaneously
- Use real browser profiles of Chrome, Firefox, Safari and Edge
- Edit, Import or Export browser cookies
- Modify WebRTC parameters
- Modify Geolocation settings
- Modify WebGL fingerprint
- Modify 2D Canvas fingerprint
- Modify Navigator properties
- Modify Screen resolution
# How to start examples
## 1. Start the Kameleo.CLI on your computer
```
./Kameleo.CLI.exe email="your@email.com" password="Pa$$w0rd"
```
> Note: _You need [Automation package](https://kameleo.io/pricing) of Kameleo to access the features described below._
## 2. Start the example code that you are interested in
For projects located in the `nodejs` folder you must run `npm install` and then `npm start`. For project located in the `dotnet-csharp` folder just open the .csproj file using Visual Studio 2019 or later.
| Folder                          | Description                                                                                                                        |
|---------------------------------|------------------------------------------------------------------------------------------------------------------------------------|
| connect_to_selenium             | This code illustrates how to start a browser in Kameleo and then automate actions using Selenium commands.                         |
| create_profile                  | This is an example for creating a Kameleo profile.                                                                                 |
| external_browser_with_puppeteer | This example showcases a way to start a browser using Puppeteer and connect it with Kameleo to add browser fingerprint protection. |
| find_baseprofile                | This quick example shows you different ways to find the perfect base profile for your work.                                        |
| manage_cookies                  | Kameleo's features to edit, modify or create cookies are easy to use, just a few lines of code needed.                             |
| profile_save_load               | Quick example of saving and loading profiles to/from .kameleo files on your computer.                                              |
| start_chrome                    | In this example you can see how you can influence the started browser type using profile attributes.                               |
| start_with_proxy                | Using any HTTP, SOCKS or SSH proxy is easy with Kameleo, just check out these few lines of code to see yourself.                   |
| update_profile                  | This code is to illustrate how easy is to modify profile attributes after you have created one.                                    |