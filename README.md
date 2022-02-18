# Kameleo Local API Example Codes
With [Kameleo](https://kameleo.io), you can easily create multiple virtual browser profiles to work with multiple accounts. It helps you hide your actual timezone, geolocation, language, IP address and creates natural browser fingerprints to prevent detection by anti-bot systems. Kameleo is compatible with [Selenium](https://www.selenium.dev/), [Playwright](https://playwright.dev/), and [Puppeteer](https://pptr.dev/) frameworks for automating web scraping tasks. This repository shows useful and easy to understand examples written in Node.js, Python and .NET Core (C#) about web scraping and automated browsing with Kameleo Client.

# Features
- Stay completely undetected, so websites won’t be able to detect that you are using automation tools
- Start unlimited number of profiles with different natural browser fingerprints
- Use authenticated HTTP/SOCKS/SSH proxies in browsers
- Create isolated browsing environments simultaneously
- Use real browser profiles of Chrome, Firefox, Safari and Edge
- Edit, Import or Export browser cookies
- Modify WebRTC parameters
- Modify Geolocation settings
- Modify Timezone and Language settings
- Modify WebGL fingerprint
- Modify 2D Canvas fingerprint
- Modify Navigator properties
- Modify Screen resolution

> Note: _You need [Automation package](https://kameleo.io/learn-more/automation/) of Kameleo to access the features described below._

# How to start examples
## 1. Start the Kameleo.CLI on your computer
```
./Kameleo.CLI.exe email="your@email.com" password="Pa$$w0rd"
```

## 2. Start the example code that you are interested in
- For projects located in the `nodejs` folder you must run `npm install` and then `npm start`.
- For projects located in the `python` folder you must run `pip install -r requirements.txt` and then `python app.py`.
- For project located in the `dotnet-csharp` folder just open the .csproj file using Visual Studio 2019 or later.

## 3. Check out Kameleo Help Center
If you are interested in more information about Kameleo, or have encountered an issue with using it, please check out our [Help Center](https://help.kameleo.io/) .

| Folder                                            | Description                                                                                                                                               |
|---------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------|
| connect_to_selenium                               | This code shows you how to launch a browser in Kameleo, perform actions using [Selenium](https://www.selenium.dev/) commands, and then close the browser. |
| connect_with_playwright_to_chrome                 | This code shows how to start a Chromium-based browser in Kameleo and automate tasks using the [Playwright](https://playwright.dev/) framework.            |
| connect_with_playwright_to_firefox                | This code illustrates how to start a Firefox in Kameleo and then automate actions using [Playwright](https://playwright.dev/) framework.                  |
| connect_with_puppeteer                            | This code illustrates how to start a Chromium based browser in Kameleo and then automate actions using [Puppeteer](https://pptr.dev/) framework.          |
| create_profile                                    | This is an example for creating a Kameleo profile using the Local API.                                                                                    |
| find_baseprofile                                  | This quick example shows you different ways to find the perfect base profile using Kameleo Local API.                                                     |
| manage_cookies                                    | Kameleo allows you to edit, modify or create cookies with just a few lines of code.                                                                       |
| profile_save_load                                 | This is a quick example of saving and loading profiles to/from .kameleo files on your computer.                                                           |
| start_chrome                                      | In this example you can see how you can change the browser started by Kameleo using profile attributes.                                                   |
| start_with_proxy                                  | Kameleo makes it easy to use any HTTP, SOCKS, or SSH proxy. Check out these few lines of code to see how.                                                 |
| update_profile                                    | This code is to illustrate how easy is to modify profile attributes after you have created one.                                                           |
| start_browser_with_additional_web_driver_settings | Check out this example if you want to pass command-line arguments or extra [Selenium](https://www.selenium.dev/) capabilities when starting a browser.    |
| test_proxy                                  | Kameleo makes it easy to use any HTTP, SOCKS, or SSH proxy. Check out these few lines of code how to check a proxy validity.                                                 |
