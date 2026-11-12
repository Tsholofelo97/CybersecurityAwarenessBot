# Cybersecurity Awareness Assistant – PROG6221 Final POE

## Student Details

**Student Name:** Tsholofelo Marabe
**Student Number:** ST10499950
**Institution:** Rosebank College
**Module:** PROG6221 Programming 2A

## Project Overview

A WPF desktop application that helps users learn about cybersecurity and stay safe online. It combines the features built in Parts 1, 2 and 3 into one application with four tabs: **Chat**, **Tasks**, **Cyber Quiz** and **Activity Log**.

## Features

### Part 1
- Voice greeting when the app opens (`Audio/greeting.wav`)
- ASCII logo shown in the header
- Asks for and remembers the user's name
- Input validation (blank messages and invalid names are handled)

### Part 2
- WPF GUI chat interface
- Keyword recognition for passwords, phishing, safe browsing, privacy, scams, malware, 2FA and public Wi-Fi
- Random responses (several tips per topic, no immediate repeats)
- Conversation flow ("tell me more" continues the last topic)
- Memory and recall (name and favourite topic, e.g. "I'm interested in privacy")
- Sentiment detection (worried, frustrated, curious, positive) that changes the bot's tone

### Part 3
- **Task Assistant:** add tasks with a title, description and optional reminder; mark complete; set a reminder; delete
- **MySQL database:** all tasks are stored in MySQL and stay in sync with the GUI
- **Due reminders:** tasks due today or earlier are announced after the user enters their name
- **Cybersecurity quiz:** 12 shuffled questions (multiple choice + true/false), one at a time, immediate feedback, score and final comment
- **NLP simulation:** recognises different phrasings with keyword detection and regular expressions, extracts task titles and dates ("tomorrow", "in 3 days", "next week")
- **Activity Log:** records tasks, reminders, quiz and NLP actions; shows the last 5, with Show More for 10

## Technologies Used
- C# / .NET 8 / WPF
- MySQL + MySql.Data NuGet package
- Visual Studio 2022

## Database Setup
1. Install and start MySQL Server.
2. Open MySQL Workbench and run `database_setup.sql`.
3. Open `Data/DatabaseConfig.cs` and replace `YOUR_PASSWORD` with your local MySQL root password.
4. Build and run the application.

> The real password is not stored in this repository.

## How to Use
- **Chat tab:** enter your name, then ask cybersecurity questions or type commands.
- **Tasks tab:** add, complete, set reminders for and delete tasks.
- **Cyber Quiz tab:** press Start Quiz, choose an answer, Submit, then Next.
- **Activity Log tab:** view recent actions; Show More shows up to 10.

### Example chat commands
| You type | What happens |
|---|---|
| Add a task to enable two-factor authentication | Task saved; bot asks if you want a reminder |
| yes, in 3 days | Reminder added to that task |
| Remind me to update my password tomorrow | Task + reminder saved |
| Create task - review privacy settings | Task saved |
| Show my tasks / What tasks do I have? | Lists pending tasks and opens Tasks tab |
| Start the quiz / Test my knowledge | Opens and starts the quiz |
| What have you done for me? / Show activity log | Summary + opens Activity Log |
| I'm worried about phishing | Empathetic tone + phishing tip |
| Tell me more | Another tip on the last topic |
| I'm interested in privacy → What's my name? | Memory recall |

## Project Structure
```text
CybersecurityAwarenessBotPart2/
├── App.xaml / App.xaml.cs
├── MainWindow.xaml            GUI (Chat, Tasks, Quiz, Activity tabs)
├── MainWindow.xaml.cs         Event handlers + integration
├── Audio/greeting.wav
├── Data/
│   ├── DatabaseConfig.cs      MySQL connection string
│   └── TaskRepository.cs      Add / read / complete / remind / delete
├── Models/
│   ├── CyberTask.cs
│   ├── QuizQuestion.cs
│   ├── QuizAnswerResult.cs
│   └── ActivityEntry.cs
├── Services/
│   ├── ResponseHandler.cs     Part 2 keyword responses, memory, follow-ups
│   ├── SentimentDetector.cs
│   ├── MemoryManager.cs
│   ├── VoiceGreeting.cs
│   ├── AsciiArt.cs
│   ├── NlpProcessor.cs        Part 3 intent detection
│   ├── UserIntent.cs / IntentResult.cs
│   ├── QuizManager.cs
│   └── ActivityLogger.cs
├── database_setup.sql
└── README.md
```

## Releases
- `v1.0-part1` – Console chatbot
- `v2.0-part2` – WPF GUI with dynamic chatbot features
- `v3.0-final` – Tasks + MySQL + Quiz + NLP + Activity Log

## Video Presentation
Unlisted YouTube link: https://youtu.be/zRjM6L2w_Tw

