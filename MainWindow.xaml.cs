using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CybersecurityAwarenessBotPart2.Data;
using CybersecurityAwarenessBotPart2.Models;
using CybersecurityAwarenessBotPart2.Services;

namespace CybersecurityAwarenessBotPart2
{
    public partial class MainWindow : Window
    {
        // ----- Part 1 & 2 services -----
        private readonly ResponseHandler responseHandler = new ResponseHandler();
        private readonly SentimentDetector sentimentDetector = new SentimentDetector();
        private readonly MemoryManager memory = new MemoryManager();

        // ----- Part 3 services -----
        private readonly TaskRepository taskRepository = new TaskRepository();
        private readonly QuizManager quizManager = new QuizManager();
        private readonly NlpProcessor nlpProcessor = new NlpProcessor();
        private readonly ActivityLogger activityLogger = new ActivityLogger();

        // ----- Conversation state -----
        private bool waitingForName = true;
        private int? taskAwaitingReminderId;      // set when the bot asks "Would you like a reminder?"
        private string taskAwaitingReminderTitle = "";
        private List<CyberTask> currentTasks = new List<CyberTask>();
        private int activityCount = 5;            // 5 normally, 10 after "Show More"

        public MainWindow()
        {
            InitializeComponent();
            LogoText.Text = AsciiArt.GetLogo();
            ReminderPicker.SelectedDate = DateTime.Today.AddDays(1);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VoiceGreeting.PlayGreeting();

            AddBotMessage("Welcome to the Cybersecurity Awareness Assistant! 🛡");
            AddBotMessage("Before we start, what is your name?");

            LoadTasks();
            DisplayRecentActivity();
            UserInput.Focus();
        }

        // =====================================================================
        //  CHAT (integration point for Parts 1, 2 and 3)
        // =====================================================================

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserMessage();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ProcessUserMessage();
        }

        private void ProcessUserMessage()
        {
            string userText = UserInput.Text.Trim();

            // Input validation (Part 1)
            if (string.IsNullOrWhiteSpace(userText))
            {
                AddBotMessage("Please type a message before sending.");
                return;
            }

            AddUserMessage(userText);
            UserInput.Clear();

            // Part 1: ask for the user's name first
            if (waitingForName)
            {
                HandleName(userText);
                return;
            }

            // Answer to "Would you like a reminder?"
            if (taskAwaitingReminderId.HasValue && HandleReminderFollowUp(userText))
            {
                DisplayRecentActivity();
                return;
            }

            // Part 3: NLP decides what the user wants
            IntentResult result = nlpProcessor.DetectIntent(userText);

            switch (result.Intent)
            {
                case UserIntent.AddTask:
                    activityLogger.Log("NLP recognised an add-task request");
                    AddBotMessage(AddTaskFromChat(result.TaskTitle, result.ReminderDate, false));
                    break;

                case UserIntent.AddTaskWithReminder:
                    activityLogger.Log("NLP recognised a reminder request");
                    AddBotMessage(AddTaskFromChat(result.TaskTitle, result.ReminderDate, true));
                    break;

                case UserIntent.StartQuiz:
                    activityLogger.Log("NLP recognised a quiz request");
                    StartQuiz();
                    MainTabs.SelectedItem = QuizTab;
                    AddBotMessage("Let's test your knowledge! I've opened the Cyber Quiz tab. Good luck!");
                    break;

                case UserIntent.ShowTasks:
                    activityLogger.Log("NLP recognised a show-tasks request");
                    AddBotMessage(DescribeTasks());
                    MainTabs.SelectedItem = TasksTab;
                    break;

                case UserIntent.ShowActivityLog:
                    activityLogger.Log("NLP recognised an activity-log request");
                    AddBotMessage(DescribeRecentActivity());
                    MainTabs.SelectedItem = ActivityTab;
                    break;

                default:
                    // Not a Part 3 command: use the Part 2 chatbot (keywords, memory, sentiment)
                    AddBotMessage(GetChatResponse(userText));
                    break;
            }

            DisplayRecentActivity();
        }

        private void HandleName(string text)
        {
            // Accept "Tsholo", "my name is Tsholo", "I'm Tsholo", etc.
            string name = Regex.Replace(text, @"^(my name is|i am|i'm|im|it's|its|call me)\s+", "",
                                        RegexOptions.IgnoreCase).Trim().TrimEnd('.', '!');

            if (name.Length < 2 || name.Length > 30 || !Regex.IsMatch(name, @"^[A-Za-z][A-Za-z '\-]*$"))
            {
                AddBotMessage("I didn't quite catch that. Please enter your name using letters only.");
                return;
            }

            name = char.ToUpper(name[0]) + name.Substring(1);
            memory.UserName = name;
            waitingForName = false;

            activityLogger.Log($"Session started for {name}");
            AddBotMessage($"Nice to meet you, {name}! I'm your Cybersecurity Awareness Assistant.");
            AddBotMessage(ResponseHandler.GetHelpText());

            CheckDueReminders();
            DisplayRecentActivity();
        }

        // Part 2 logic: sentiment changes the tone, ResponseHandler gives the answer.
        private string GetChatResponse(string userText)
        {
            string sentiment = sentimentDetector.DetectSentiment(userText);
            string response = responseHandler.GetResponse(userText, memory);

            string prefix = sentiment switch
            {
                "worried" => "It's completely understandable to feel that way. Let me help. ",
                "frustrated" => "I know this can feel overwhelming. Let's take it one step at a time. ",
                "curious" => "Great question! ",
                "positive" when !userText.ToLower().Contains("thank") => "That's great to hear! ",
                _ => ""
            };

            return prefix + response;
        }

        private void AddBotMessage(string text)
        {
            ChatDisplay.AppendText("Bot: " + text + "\n");
            ChatDisplay.ScrollToEnd();
        }

        private void AddUserMessage(string text)
        {
            string label = memory.UserName ?? "You";
            ChatDisplay.AppendText("\n" + label + ": " + text + "\n");
            ChatDisplay.ScrollToEnd();
        }

        // =====================================================================
        //  TASKS FROM CHAT (NLP -> database)
        // =====================================================================

        private string AddTaskFromChat(string title, DateTime? reminderDate, bool reminderRequested)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return "I can see you want to add a task, but what should it be? " +
                       "Try: \"Add a task to review my privacy settings\".";
            }

            CyberTask task = new CyberTask
            {
                Title = title,
                Description = nlpProcessor.SuggestDescription(title),
                ReminderDate = reminderDate,
                IsCompleted = false
            };

            try
            {
                task.TaskId = taskRepository.AddTask(task);
                activityLogger.Log($"Task added: '{title}'");
                LoadTasks();

                if (reminderDate.HasValue)
                {
                    activityLogger.Log($"Reminder set: '{title}' on {reminderDate.Value:dd MMM yyyy}");
                    return $"Task added: \"{title}\". I'll remind you on {reminderDate.Value:dddd, dd MMMM yyyy}.";
                }

                // No date given yet, so ask a follow-up question.
                taskAwaitingReminderId = task.TaskId;
                taskAwaitingReminderTitle = title;

                if (reminderRequested)
                    return $"Task added: \"{title}\". When should I remind you? (e.g. \"tomorrow\", \"in 3 days\" or \"next week\")";

                return $"Task added: \"{title}\". Description: {task.Description} " +
                       "Would you like a reminder? (e.g. \"yes, in 3 days\" or \"no\")";
            }
            catch (Exception ex)
            {
                return "Sorry, I couldn't save that task to the database: " + ex.Message;
            }
        }

        // Handles the answer to "Would you like a reminder?". Returns true if handled.
        private bool HandleReminderFollowUp(string text)
        {
            string lower = text.ToLower().Trim();
            int taskId = taskAwaitingReminderId!.Value;

            // A brand-new command means the user moved on.
            if (lower.Contains("remind me to") || lower.Contains("task") || lower.Contains("quiz"))
            {
                ClearPendingReminder();
                return false;
            }

            DateTime? date = nlpProcessor.ExtractReminderDate(lower);
            if (date.HasValue)
            {
                try
                {
                    taskRepository.UpdateReminder(taskId, date.Value);
                    activityLogger.Log($"Reminder set: '{taskAwaitingReminderTitle}' on {date.Value:dd MMM yyyy}");
                    AddBotMessage($"Done! I'll remind you about \"{taskAwaitingReminderTitle}\" on {date.Value:dddd, dd MMMM yyyy}.");
                    LoadTasks();
                }
                catch (Exception ex)
                {
                    AddBotMessage("Sorry, I couldn't save the reminder: " + ex.Message);
                }

                ClearPendingReminder();
                return true;
            }

            if (Regex.IsMatch(lower, @"^(no|nope|nah|not now)\b"))
            {
                AddBotMessage("No problem, no reminder set. You can add one later on the Tasks tab.");
                ClearPendingReminder();
                return true;
            }

            if (Regex.IsMatch(lower, @"^(yes|yeah|yep|sure|ok|okay|please)\b"))
            {
                AddBotMessage("When should I remind you? You can say \"tomorrow\", \"in 3 days\" or \"next week\".");
                return true;
            }

            // Something unrelated - forget the question and treat it normally.
            ClearPendingReminder();
            return false;
        }

        private void ClearPendingReminder()
        {
            taskAwaitingReminderId = null;
            taskAwaitingReminderTitle = "";
        }

        private string DescribeTasks()
        {
            if (!LoadTasks())
                return "I couldn't load your tasks from the database. Please check that MySQL is running.";

            List<CyberTask> pending = currentTasks.Where(t => !t.IsCompleted).ToList();

            if (currentTasks.Count == 0)
                return "You don't have any tasks yet. Try \"Add a task to enable two-factor authentication\".";

            if (pending.Count == 0)
                return $"All {currentTasks.Count} of your tasks are completed. Well done! I've opened the Tasks tab.";

            string list = string.Join("\n", pending.Select(t => $"  • {t.Title} ({t.ReminderText})"));
            return $"You have {pending.Count} pending task(s):\n{list}\nI've opened the Tasks tab for you.";
        }

        private void CheckDueReminders()
        {
            List<CyberTask> due = currentTasks
                .Where(t => !t.IsCompleted && t.ReminderDate.HasValue && t.ReminderDate.Value.Date <= DateTime.Today)
                .ToList();

            if (due.Count == 0) return;

            AddBotMessage($"⏰ Reminder: you have {due.Count} task(s) due: " +
                          string.Join(", ", due.Select(t => $"\"{t.Title}\"")) + ".");
            activityLogger.Log($"Shown {due.Count} due reminder(s)");
        }

        // =====================================================================
        //  TASKS TAB (buttons)
        // =====================================================================

        // Loads tasks from MySQL into the grid. Returns false if the database failed.
        private bool LoadTasks()
        {
            try
            {
                currentTasks = taskRepository.GetAllTasks();
                TasksGrid.ItemsSource = currentTasks;
                return true;
            }
            catch (Exception ex)
            {
                TasksGrid.ItemsSource = null;
                TaskStatusText.Text = "Could not load tasks from MySQL: " + ex.Message;
                return false;
            }
        }

        private CyberTask? GetSelectedTask()
        {
            return TasksGrid.SelectedItem as CyberTask;
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleInput.Text.Trim();
            string description = TaskDescriptionInput.Text.Trim();

            if (title == "" || description == "")
            {
                TaskStatusText.Text = "Please enter both a title and a description.";
                return;
            }

            DateTime? reminder = null;
            if (UseReminderCheck.IsChecked == true)
            {
                if (!ReminderPicker.SelectedDate.HasValue || ReminderPicker.SelectedDate.Value.Date < DateTime.Today)
                {
                    TaskStatusText.Text = "Please choose a reminder date from today onwards.";
                    return;
                }
                reminder = ReminderPicker.SelectedDate.Value.Date;
            }

            CyberTask task = new CyberTask
            {
                Title = title,
                Description = description,
                ReminderDate = reminder,
                IsCompleted = false
            };

            try
            {
                taskRepository.AddTask(task);
                activityLogger.Log($"Task added: '{title}'");
                if (reminder.HasValue)
                    activityLogger.Log($"Reminder set: '{title}' on {reminder.Value:dd MMM yyyy}");

                TaskStatusText.Text = reminder.HasValue
                    ? $"Task added with a reminder on {reminder.Value:dd MMM yyyy}."
                    : "Task added successfully.";

                TaskTitleInput.Clear();
                TaskDescriptionInput.Clear();
                UseReminderCheck.IsChecked = false;

                LoadTasks();
                DisplayRecentActivity();
            }
            catch (Exception ex)
            {
                TaskStatusText.Text = "Task could not be saved: " + ex.Message;
            }
        }

        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            CyberTask? task = GetSelectedTask();
            if (task == null)
            {
                TaskStatusText.Text = "Please select a task first.";
                return;
            }
            if (task.IsCompleted)
            {
                TaskStatusText.Text = "That task is already completed.";
                return;
            }

            try
            {
                taskRepository.MarkTaskCompleted(task.TaskId);
                activityLogger.Log($"Task completed: '{task.Title}'");
                TaskStatusText.Text = $"\"{task.Title}\" marked as completed. Nice work!";
                LoadTasks();
                DisplayRecentActivity();
            }
            catch (Exception ex)
            {
                TaskStatusText.Text = "Could not complete task: " + ex.Message;
            }
        }

        private void SetReminderButton_Click(object sender, RoutedEventArgs e)
        {
            CyberTask? task = GetSelectedTask();
            if (task == null)
            {
                TaskStatusText.Text = "Please select a task first.";
                return;
            }
            if (!ReminderPicker.SelectedDate.HasValue || ReminderPicker.SelectedDate.Value.Date < DateTime.Today)
            {
                TaskStatusText.Text = "Please choose a reminder date (from today onwards) on the left.";
                return;
            }

            DateTime date = ReminderPicker.SelectedDate.Value.Date;

            try
            {
                taskRepository.UpdateReminder(task.TaskId, date);
                activityLogger.Log($"Reminder set: '{task.Title}' on {date:dd MMM yyyy}");
                TaskStatusText.Text = $"Reminder for \"{task.Title}\" set to {date:dd MMM yyyy}.";
                LoadTasks();
                DisplayRecentActivity();
            }
            catch (Exception ex)
            {
                TaskStatusText.Text = "Could not set reminder: " + ex.Message;
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            CyberTask? task = GetSelectedTask();
            if (task == null)
            {
                TaskStatusText.Text = "Please select a task first.";
                return;
            }

            MessageBoxResult answer = MessageBox.Show(
                $"Delete this task?\n\n{task.Title}",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (answer != MessageBoxResult.Yes) return;

            try
            {
                taskRepository.DeleteTask(task.TaskId);
                activityLogger.Log($"Task deleted: '{task.Title}'");
                TaskStatusText.Text = "Task deleted.";
                LoadTasks();
                DisplayRecentActivity();
            }
            catch (Exception ex)
            {
                TaskStatusText.Text = "Could not delete task: " + ex.Message;
            }
        }

        private void RefreshTasksButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoadTasks())
                TaskStatusText.Text = "Task list refreshed from MySQL.";
        }

        // =====================================================================
        //  QUIZ TAB
        // =====================================================================

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            StartQuiz();
            DisplayRecentActivity();
        }

        private void StartQuiz()
        {
            quizManager.StartQuiz();
            activityLogger.Log("Cybersecurity quiz started");

            QuizFeedbackText.Text = "";
            StartQuizButton.Content = "Restart Quiz";
            SubmitAnswerButton.IsEnabled = true;
            NextQuestionButton.IsEnabled = false;

            DisplayCurrentQuestion();
        }

        private void DisplayCurrentQuestion()
        {
            QuizQuestion question = quizManager.CurrentQuestion;

            QuestionNumberText.Text = $"Question {quizManager.CurrentIndex + 1} of {quizManager.TotalQuestions}" +
                                      (question.Options.Count == 2 ? "  (True / False)" : "  (Multiple choice)");
            QuestionText.Text = question.QuestionText;

            RadioButton[] buttons = { OptionA, OptionB, OptionC, OptionD };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].IsChecked = false;
                buttons[i].IsEnabled = true;

                if (i < question.Options.Count)
                {
                    buttons[i].Content = question.Options[i];
                    buttons[i].Visibility = Visibility.Visible;
                }
                else
                {
                    // True/False questions only need two options.
                    buttons[i].Visibility = Visibility.Collapsed;
                }
            }

            QuizScoreText.Text = $"Score: {quizManager.Score}/{quizManager.TotalQuestions}";
        }

        private int GetSelectedQuizOption()
        {
            if (OptionA.IsChecked == true) return 0;
            if (OptionB.IsChecked == true) return 1;
            if (OptionC.IsChecked == true) return 2;
            if (OptionD.IsChecked == true) return 3;
            return -1;
        }

        private void SubmitAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            int selected = GetSelectedQuizOption();
            if (selected == -1)
            {
                QuizFeedbackText.Text = "Please select an answer first.";
                return;
            }

            QuizAnswerResult result = quizManager.CheckAnswer(selected);
            QuizFeedbackText.Text = result.Feedback;
            QuizScoreText.Text = $"Score: {quizManager.Score}/{quizManager.TotalQuestions}";

            // Lock the answer so the same question can't be scored twice.
            foreach (RadioButton rb in new[] { OptionA, OptionB, OptionC, OptionD })
                rb.IsEnabled = false;

            SubmitAnswerButton.IsEnabled = false;
            NextQuestionButton.IsEnabled = true;
            NextQuestionButton.Content =
                quizManager.CurrentIndex == quizManager.TotalQuestions - 1 ? "See Results" : "Next Question";
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (quizManager.MoveNext())
            {
                QuizFeedbackText.Text = "";
                SubmitAnswerButton.IsEnabled = true;
                NextQuestionButton.IsEnabled = false;
                DisplayCurrentQuestion();
                return;
            }

            // Quiz finished
            string summary = $"Quiz complete! Final score: {quizManager.Score}/{quizManager.TotalQuestions}. " +
                             quizManager.GetFinalComment();

            QuestionNumberText.Text = "Quiz finished";
            QuestionText.Text = "";
            foreach (RadioButton rb in new[] { OptionA, OptionB, OptionC, OptionD })
                rb.Visibility = Visibility.Collapsed;

            QuizFeedbackText.Text = summary;
            SubmitAnswerButton.IsEnabled = false;
            NextQuestionButton.IsEnabled = false;
            NextQuestionButton.Content = "Next Question";

            activityLogger.Log($"Quiz completed - score {quizManager.Score}/{quizManager.TotalQuestions}");
            AddBotMessage(summary);
            DisplayRecentActivity();
        }

        // =====================================================================
        //  ACTIVITY LOG TAB
        // =====================================================================

        private void DisplayRecentActivity()
        {
            ActivityList.Items.Clear();
            List<ActivityEntry> recent = activityLogger.GetRecent(activityCount);

            ActivityTitleText.Text = $"Recent activity (last {activityCount})";
            ShowMoreButton.Content = activityCount == 5 ? "Show More" : "Show Less";

            if (recent.Count == 0)
            {
                ActivityList.Items.Add("No activity recorded yet.");
                return;
            }

            foreach (ActivityEntry entry in recent)
                ActivityList.Items.Add(entry.ToString());
        }

        // Summary shown in the chat when the user asks "What have you done for me?"
        private string DescribeRecentActivity()
        {
            List<ActivityEntry> recent = activityLogger.GetRecent(5);
            if (recent.Count == 0)
                return "Nothing yet! Try adding a task or starting the quiz.";

            string list = string.Join("\n", recent.Select((a, i) => $"  {i + 1}. {a.Description}"));
            return $"Here's a summary of recent actions:\n{list}\nI've opened the Activity Log tab for the full view.";
        }

        private void RefreshActivityButton_Click(object sender, RoutedEventArgs e)
        {
            DisplayRecentActivity();
        }

        private void ShowMoreButton_Click(object sender, RoutedEventArgs e)
        {
            activityCount = activityCount == 5 ? 10 : 5;
            DisplayRecentActivity();
        }
    }
}
