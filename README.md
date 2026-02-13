# NewsParser

![C#](https://img.shields.io/badge/language-C%23-blue)
![.NET](https://img.shields.io/badge/Framework-.NET%20WinForms-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)

**NewsParser** is a lightweight Windows desktop application that aggregates and displays news from multiple sources (RIA Novosti and BELTA). It scrapes news headlines and content using the HtmlAgilityPack library and presents them in a clean, dark-themed user interface.

## ✨ Features

- **Multi-Source Support**: Fetches news from `ria.ru` (Russia) and `belta.by` (Belarus).
- **Category Navigation**: Filter news by categories (World, Science, Economy, Society, Incidents).
- **In-App Reading**: Click a news button to open a detailed view with the title, full article text, and the main image.
- **Open in Browser**: If you prefer, you can open the original article in your default web browser.
- **Customizable UI**: Dark theme designed for comfortable reading.
- **Simple Source Switching**: Easily change the news source via a dedicated settings form.

## 🖼️ Screenshots

*(Consider adding screenshots here showing the main window, the news list, and the article details view.  
To add a screenshot, upload an image to your repo and use: `![Main Window](screenshots/main.png)`)*

## 🚀 How It Works

1.  The application sends an HTTP request to the selected news category URL.
2.  It uses **HtmlAgilityPack** to parse the HTML and extract news headlines and links.
3.  Headlines are displayed as buttons in a scrollable panel.
4.  Clicking a button scrapes the individual article page to extract the title, full text, and image.
5.  The article content is displayed in a new window.

## 🛠️ Technologies Used

- **Language:** C#
- **Framework:** .NET Windows Forms (WinForms)
- **Parsing:** [HtmlAgilityPack](https://html-agility-pack.net/)
- **HTTP Client:** `System.Net.Http.HttpClient`

## 📦 Getting Started

### Prerequisites
- Windows OS
- [.NET Framework](https://dotnet.microsoft.com/en-us/download/dotnet-framework) (Version 4.7.2 or higher recommended)
- Visual Studio (or any C# IDE)

### Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/YOUR_USERNAME/NewsParser.git
    ```
2.  **Open the solution** (`NewsParser.sln`) in Visual Studio.
3.  **Restore NuGet Packages:** If HtmlAgilityPack is not installed, open the NuGet Package Manager Console and run:
    ```bash
    Install-Package HtmlAgilityPack
    ```
4.  **Build** the solution (Ctrl+Shift+B).
5.  **Run** the application (F5).

## 🧩 Project Structure

- **`MainForm.cs`**: The main window containing the category buttons and the news panel.
- **`SettingsForm.cs`**: A pop-up window to switch between different news sources (Russia/Belarus).
- **`NewsDetailsForm.cs` / `NewsDetailsFormRB.cs`**: Windows that display the full content of a selected news article.

## ⚠️ Note on Web Scraping

- This application relies on the current HTML structure of `ria.ru` and `belta.by`. If these websites update their layout, the parser may break and require updates to the XPath selectors.
- The project is intended for **educational purposes**. Always respect the `robots.txt` and terms of service of the websites you scrape.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📧 Contact

Project Link: [https://github.com/YOUR_USERNAME/NewsParser](https://github.com/YOUR_USERNAME/NewsParser)
