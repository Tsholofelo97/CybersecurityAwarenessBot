# Cybersecurity Awareness Chatbot

## Project Overview

The Cybersecurity Awareness Chatbot is a C# console application that provides users with basic information about cybersecurity and online safety.

The chatbot allows users to ask questions about common cybersecurity topics and provides helpful responses.

## Features

* Interactive chatbot conversation
* Cybersecurity awareness information
* Password safety advice
* Phishing awareness
* Safe browsing guidance
* Malware information
* Social engineering awareness
* Input validation
* Default responses for unknown questions
* Coloured console interface
* ASCII art logo
* WAV voice greeting
* Error handling

## Technologies Used

* C#
* .NET
* Visual Studio Code
* Console Application
* WAV audio

## Project Structure

* `Program.cs` – Starts the chatbot application.
* `Chatbot.cs` – Controls the main chatbot conversation.
* `AsciiArt.cs` – Displays the chatbot logo and console design.
* `UserProfile.cs` – Stores the user's name.
* `ResponseHandler.cs` – Processes questions and provides responses.
* `VoiceGreeting.cs` – Plays the WAV greeting.
* `Audio/greeting.wav` – Contains the chatbot's voice greeting.

## How to Run

1. Open the project folder in Visual Studio Code.
2. Make sure all C# files are included.
3. Make sure the `Audio` folder contains `greeting.wav`.
4. Build the project.
5. Run the application.
6. Follow the instructions displayed in the console.

## Example Questions

The chatbot can answer questions such as:

* What is phishing?
* How can I create a strong password?
* How can I browse safely?
* What is malware?
* What is social engineering?
* What is your purpose?
* How are you?

## Input Validation

The chatbot checks whether the user has entered a valid question.

If the user enters an empty input, the chatbot asks them to enter a question.

If the chatbot does not recognise the question, it provides a default response and suggests cybersecurity topics that the user can ask about.

## Error Handling

The application uses error handling to manage unexpected errors.

The voice greeting checks whether the WAV file exists before attempting to play it. If the audio file cannot be found or an audio error occurs, an informative message is displayed instead of allowing the application to crash.

## User Interface

The application uses coloured text, borders, spacing and ASCII art to make the console interface easier to read and more engaging.

## Author

**Tsholofelo Marabe**

## Project Purpose

The purpose of this project is to demonstrate C# programming and software development skills while creating an interactive chatbot that promotes cybersecurity awareness.
