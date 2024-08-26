using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//---------------- Hello It's Me Amro Omran    ----------------------------- \\
//---------------- Hotmail.Home@gmail.com     ------------------------------ \\
//---------------- Visit My Youtube Channel ---------------------------------\\
//---------------- https://www.youtube.com/@Amro-Omran ----------------------\\
//---------------- Please Like And subscribe For More useful Code ---------- \\
//---------------- buy me acoffee ------------------------------------------ \\
namespace Chatbot
{
    public partial class FinalChatBot : Form
    {
        private Dictionary<string, string> responsePatterns;
        private Dictionary<string, string> memory; // Chatbot memory
        private WebBrowser webBrowser; // WebBrowser control for searching
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public FinalChatBot()
        {
            InitializeComponent();
            InitializeChatbot();
            this.Size = new Size(1100, 750);
            chatBox.ReadOnly = true;
            memory = new Dictionary<string, string>(); // Initialize memory
            this.Load += Form1_Load;
            this.MouseDown += ChatBotFormNew_MouseDown;
            CloseForm.Click += CloseForm_Click;
            MaximizeForm.Click += MaximizeForm_Click;
            MinimizeForm.Click += MinimizeForm_Click;
            ClearChatButton.Click += ClearChatButton_Click;
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true; // Enable double buffering
            SetStyle(ControlStyles.ResizeRedraw, true); // This reduces flicker on resize
            this.MaximizedBounds = new Rectangle(2, 2, Screen.PrimaryScreen.WorkingArea.Width - 4, Screen.PrimaryScreen.WorkingArea.Height - 4);
        }
        private void CloseForm_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void MaximizeForm_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                //MaximizeForm.Text = "1";
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                //MaximizeForm.Text = "2";
            }
        }
        private void MinimizeForm_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void ChatBotFormNew_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Chatbot";
            // WebBrowser control for searching
            webBrowser = new WebBrowser
            {
                ScriptErrorsSuppressed = true,
                Visible = false
            };
            this.Controls.Add(webBrowser);
            // Event handler for the send button click
            sendButton.Click += (s, ev) =>
            {
                ProcessUserInput(userInput.Text.Trim(), chatBox);
                userInput.Clear();
            };
            // Event handler for the Enter key in the TextBox
            userInput.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                {
                    ev.SuppressKeyPress = true; // Prevent the ding sound
                    sendButton.PerformClick(); // Simulate button click
                }
            };
            // Register the LinkClicked event once
            chatBox.LinkClicked += richTextBoxChat_LinkClicked;
        }
        private void ProcessUserInput(string userMessage, RichTextBox chatBox)
        {
            if (!string.IsNullOrEmpty(userMessage))
            {
                // Load and resize image for better alignment
                Image userImage = ResizeImage(Properties.Resources.UserImage, chatBox.Font.Height);
                InsertImage(chatBox, userImage);
                chatBox.AppendText("You: " + userMessage + "\n");
                // Get bot response
                string botResponse = GetBotResponse(userMessage);
                // Load and resize image for better alignment
                Image botImage = ResizeImage(Properties.Resources.BotImage, chatBox.Font.Height);
                InsertImage(chatBox, botImage);
                chatBox.AppendText("Bot: " + botResponse + "\n");
                // Apply text highlighting (background color) for userMessage
                int startUserMessageIndex = chatBox.Text.LastIndexOf("You: " + userMessage);
                chatBox.Select(startUserMessageIndex, userMessage.Length + 5); // +5 for "You: "
                chatBox.SelectionBackColor = Color.LightGray;
                // Apply text highlighting (background color) for botResponse
                int startBotResponseIndex = chatBox.Text.LastIndexOf("Bot: " + botResponse);
                chatBox.Select(startBotResponseIndex, botResponse.Length + 5); // +5 for "Bot: "
                chatBox.SelectionBackColor = Color.LightBlue;
                // Reset selection
                chatBox.SelectionStart = chatBox.Text.Length;
                chatBox.SelectionLength = 0;
                chatBox.SelectionColor = chatBox.ForeColor;
            }
            // Set the current caret position to the end
            chatBox.SelectionStart = chatBox.Text.Length;
            // Scroll it automatically
            chatBox.ScrollToCaret();
            ColorText(chatBox, "You: ", Color.Green);
            ColorText(chatBox, "Bot: ", Color.Red);
        }
        private Image ResizeImage(Image image, int targetHeight)
        {
            // Calculate new width to maintain aspect ratio
            float aspectRatio = (float)image.Width / image.Height;
            int newWidth = (int)(targetHeight * aspectRatio);
            // Resize the image
            Bitmap newImage = new Bitmap(image, newWidth, targetHeight);
            // Dispose of the original image if it’s no longer needed
            image.Dispose();
            return newImage;
        }
        private void InsertImage(RichTextBox richTextBox, Image image)
        {
            // Temporarily disable ReadOnly to insert the image
            bool wasReadOnly = richTextBox.ReadOnly;
            richTextBox.ReadOnly = false;
            // Save current selection
            int originalSelectionStart = richTextBox.SelectionStart;
            // Set the image in the clipboard
            Clipboard.SetImage(image);
            // Paste the image into the RichTextBox
            richTextBox.Paste();
            // Clear the clipboard to remove the image
            Clipboard.Clear();
            // Restore ReadOnly state
            richTextBox.ReadOnly = wasReadOnly;
            // Restore selection
            richTextBox.SelectionStart = originalSelectionStart + 1; // Move selection after the image
            richTextBox.SelectionLength = 0;
            image.Dispose();
        }
        // Initialize chatbot with more predefined patterns and responses
        private void InitializeChatbot()
        {
            responsePatterns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                 // Greetings and Introductions
    { "hello", "Hello! How can I assist you today?" },
    { "hi", "Hi there! What can I help you with?" },
    { "good morning", "Good morning! How can I help you today?" },
    { "good afternoon", "Good afternoon! What can I do for you?" },
    { "good evening", "Good evening! How can I assist you?" },
    // Time and Date
    { "time", $"The current time is: {DateTime.Now.ToShortTimeString()}" },
    { "date", $"Today's date is: {DateTime.Now.ToShortDateString()}" },
    // Personal Information
    { "how are you", "I'm just a bot, but I'm functioning as expected!" },
    { "what is your name", "I'm your friendly chatbot!" },
    // Memory Management
    { "remember", "Please specify what you want me to remember. Use the format: 'Remember that [key] is [value]'." },
    { "recall", "Let me know what you want me to recall, and I'll do my best to provide the information." },
    { "forget", "Please tell me what you want me to forget, and I'll remove it from my memory." },
    { "my name is", "Nice to meet you, {value}!" },
    { "remember that", "Got it! I'll remember that {key} is {value}." },
    { "what is", "You mentioned that {key} is {value}." },
    // Fun Facts and Trivia
    { "what is the speed of light", "The speed of light is approximately 299,792 kilometers per second (km/s)." },
    { "who painted the Mona Lisa", "The Mona Lisa was painted by Leonardo da Vinci." },
    { "what is the largest planet in our solar system", "The largest planet in our solar system is Jupiter." },
    { "how many bones are in the human body", "There are 206 bones in the adult human body." },
    { "who wrote 'Pride and Prejudice'", "'Pride and Prejudice' was written by Jane Austen." },
    // Interactive and Engaging
    { "tell me a riddle", "Here's a riddle: What has keys but can't open locks? A piano!" },
    { "what is your favorite TV show", "I don't watch TV, but 'Friends' is a popular choice among many people." },
    { "can you do math", "Yes, I can help with basic math calculations. Just ask!" },
    { "what is the square root of 16", "The square root of 16 is 4." },
    { "what is 2 plus 2", "2 plus 2 equals 4." },
    // User Preferences
    { "my favorite color is", "Got it! I'll remember that your favorite color is {value}." },
    { "my favorite movie is", "Thanks for sharing! I'll remember that your favorite movie is {value}." },
    { "my favorite food is", "Got it! I'll remember that your favorite food is {value}." },
    // Practical Information
    { "how do I reset my password", "To reset your password, follow the instructions on the login page of the website or service you're using. There should be a 'Forgot Password' link or similar option." },
    { "how can I contact support", "You can contact support through the 'Contact Us' section of the website, via email, or by calling the support phone number provided." },
    { "how do I update my profile", "To update your profile, log in to the relevant website or app, go to your profile settings, and make the necessary changes." },
    { "how do I install an app", "To install an app, visit the app store on your device, search for the app, and follow the installation instructions." },
    { "how do I delete an account", "To delete an account, go to the account settings of the service or website, and look for the 'Delete Account' option. Follow the instructions to complete the process." },
    // More Interactive Responses
    { "what is your favorite game", "I don't play games, but many people enjoy games like 'Chess' or 'Minecraft'." },
    { "what is your favorite activity", "As a bot, I don't have personal preferences, but I enjoy helping users with their queries!" },
    { "can you help me with coding", "Yes, I can help with coding questions and provide examples or explanations. What do you need help with?" },
    { "what is a good programming language to learn", "It depends on your goals, but popular languages include Python for its simplicity, JavaScript for web development, and C# for its versatility in various applications." },
    // Common Queries
    { "what is the best way to study", "Effective study techniques include setting specific goals, using active learning methods, taking breaks, and reviewing material regularly." },
    { "how can I improve my writing", "To improve your writing, practice regularly, read widely, seek feedback, and revise your work." },
    { "how can I save money", "Consider budgeting, tracking your expenses, and finding ways to cut unnecessary costs to save money." },
    // Advanced Topics
    { "what is blockchain technology", "Blockchain technology is a decentralized digital ledger that records transactions across many computers in a way that the registered transactions cannot be altered retroactively." },
    { "how does machine learning work", "Machine learning involves training algorithms on large datasets so they can make predictions or decisions without being explicitly programmed for each task." },
    { "what is virtual reality", "Virtual reality (VR) is a simulated experience that can be similar to or completely different from the real world, often achieved through VR headsets and interactive environments." },
    { "what is augmented reality", "Augmented reality (AR) overlays digital information onto the real world, enhancing the user's perception of their environment." },
    { "what is the Internet of Things", "The Internet of Things (IoT) refers to the network of physical devices embedded with sensors and software to connect and exchange data with other devices over the internet." },
    // Fun Interactions
    { "what is the most popular song right now", "I don't have real-time data, but you can check music charts or streaming platforms for the latest popular songs." },
    { "can you recommend a book", "Sure! If you like fiction, 'The Catcher in the Rye' by J.D. Salinger is a classic. For non-fiction, 'Sapiens: A Brief History of Humankind' by Yuval Noah Harari is highly recommended." },
    { "what is the best movie of all time", "Opinions on the best movie of all time vary, but classics like 'The Godfather' and 'The Shawshank Redemption' are often cited." },
    // Health and Wellness
    { "how can I improve my sleep", "Maintain a consistent sleep schedule, create a relaxing bedtime routine, and ensure your sleep environment is comfortable and free from distractions." },
    { "what are the benefits of meditation", "Meditation can help reduce stress, improve focus, enhance emotional well-being, and promote relaxation." },
    { "how can I stay hydrated", "Drink plenty of water throughout the day and consume foods with high water content, such as fruits and vegetables, to stay hydrated." },
    // Daily Life
    { "how do I plan a trip", "Start by choosing a destination, setting a budget, creating an itinerary, booking transportation and accommodations, and packing essentials." },
    { "how do I write a resume", "Include your contact information, a summary of your qualifications, work experience, education, and relevant skills when writing a resume." },
    { "how do I start a workout routine", "Set clear fitness goals, choose exercises you enjoy, create a schedule, and gradually increase the intensity of your workouts." },
    // Miscellaneous
    { "what is the purpose of life", "The purpose of life is a philosophical question and can vary greatly depending on personal beliefs and values." },
    { "what is the meaning of happiness", "Happiness is often described as a state of well-being and contentment, which can be influenced by various factors such as relationships, achievements, and personal fulfillment." },
    { "what is the key to a successful relationship", "Communication, trust, and mutual respect are key to a successful relationship." },
    // Additional Custom Queries
    { "what do you know about me", "I only know what you choose to share with me during our conversations. If there's something specific you want me to remember, just let me know." },
    { "how do I make a good impression", "To make a good impression, be genuine, listen actively, and show interest in the other person. Being polite and respectful also helps." },
    { "how do I get better at public speaking", "Practice regularly, prepare thoroughly, and work on your delivery techniques to improve your public speaking skills." },
    // Extended Fun Facts
    { "tell me a funny fact", "Did you know that sea otters hold hands while they sleep to keep from drifting apart?" },
    { "what is the longest river in the world", "The Nile River is often considered the longest river in the world, though some argue that the Amazon River is longer." },
    { "who invented the light bulb", "Thomas Edison is credited with inventing the practical electric light bulb." },
    { "what is the smallest country in the world", "Vatican City is the smallest country in the world by both area and population." },
            };
        }
        // Pattern matching chatbot logic with memory feature
        private string GetBotResponse(string message)
        {
            string lowerMessage = message.ToLower();
            // Check for specific pattern matches first
            foreach (var pattern in responsePatterns)
            {
                if (lowerMessage.Equals(pattern.Key.ToLower()))
                {
                    return pattern.Value;
                }
            }
            // Memory Management
            if (lowerMessage.StartsWith("my name is"))
            {
                string name = message.Substring(11).Trim();
                memory["name"] = name;
                return $"Nice to meet you, {name}!";
            }
            else if (lowerMessage.StartsWith("remember that"))
            {
                string[] parts = message.Substring(14).Split(new string[] { " is " }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    memory[parts[0].Trim()] = parts[1].Trim();
                    return $"Got it! I'll remember that {parts[0].Trim()} is {parts[1].Trim()}.";
                }
                return "I didn't understand that. Please use the format: 'Remember that [key] is [value]'.";
            }
            else if (lowerMessage.StartsWith("forget"))
            {
                string key = message.Substring(6).Trim();
                if (memory.ContainsKey(key))
                {
                    memory.Remove(key);
                    return $"I've forgotten {key}.";
                }
                return $"I don't have anything remembered for {key}.";
            }
            else if (lowerMessage.StartsWith("what is"))
            {
                string key = message.Substring(7).Trim();
                if (memory.ContainsKey(key))
                {
                    return $"{key} is {memory[key]}.";
                }
                // Handle mathematical expressions
                else
                {
                    string expression = key;
                    try
                    {
                        var result = new DataTable().Compute(expression, null);
                        return $"The result of {expression} is {result}.";
                    }
                    catch
                    {
                        return "Sorry, I couldn't calculate that. Please check your expression.";
                    }
                }
            }
            else if (lowerMessage.StartsWith("calculate"))
            {
                string key = message.Substring(9).Trim();
                if (memory.ContainsKey(key))
                {
                    return $"{key} is {memory[key]}.";
                }
                // Handle mathematical expressions
                else
                {
                    string expression = key;
                    try
                    {
                        var result = new DataTable().Compute(expression, null);
                        return $"The result of {expression} is {result}.";
                    }
                    catch
                    {
                        return "Sorry, I couldn't calculate that. Please check your expression.";
                    }
                }
            }
            // If no match found, search on Google
            return SearchGoogle(message);
        }
        private string SearchGoogle(string query)
        {
            // Start a search in the WebBrowser control
            string url = $"https://www.google.com/search?q={Uri.EscapeDataString(query)}";
            webBrowser.DocumentCompleted += (s, args) =>
            {
                if (webBrowser.Document != null)
                {
                    HtmlDocument document = webBrowser.Document;
                    HashSet<string> resultLinks = new HashSet<string>(); // To track unique links
                    List<string> results = new List<string>();
                    int maxResultsToDisplay = 2; // Control the number of results
                    // Gather all result items (e.g., search results)
                    var resultItems = document.GetElementsByTagName("div");
                    foreach (HtmlElement result in resultItems)
                    {
                        // Look for elements that contain links and description
                        var headers = result.GetElementsByTagName("h3");
                        var linkElements = result.GetElementsByTagName("a");
                        if (headers.Count > 0 && linkElements.Count > 0)
                        {
                            string headingText = headers[0].InnerText; // Heading text
                            string linkUrl = linkElements[0].GetAttribute("href"); // URL
                            // Check if the link is already added
                            if (!resultLinks.Add(linkUrl)) // Adds link to the set and returns false if it's a duplicate
                            {
                                continue; // Skip processing if the link is a duplicate
                            }
                            // Search for a description in the next sibling
                            string descriptionText = string.Empty;
                            var nextSibling = result.NextSibling;
                            while (nextSibling != null && string.IsNullOrWhiteSpace(nextSibling.InnerText))
                            {
                                nextSibling = nextSibling.NextSibling;
                            }
                            if (nextSibling != null)
                            {
                                descriptionText = nextSibling.InnerText.Trim();
                            }
                            results.Add($"Heading: {headingText}\nLink: {linkUrl}\nDescription: {descriptionText}");
                            // Stop collecting more results if the maximum is reached
                            if (results.Count >= maxResultsToDisplay)
                            {
                                break; // Exit the loop when the limit is reached
                            }
                        }
                    }
                    // Display the limited results
                    if (results.Any())
                    {
                        // Load and resize image for better alignment
                        Image botImage = ResizeImage(Properties.Resources.BotImage, chatBox.Font.Height);
                        InsertImage(chatBox, botImage);
                        //// Insert image before "Bot: "
                        //InsertImage(chatBox, Properties.Resources.BotImage); // Replace with your image
                        chatBox.AppendText($"Bot: Here are the top {Math.Min(maxResultsToDisplay, results.Count)} results:\n");
                        // Store the starting index for highlighting the bot response
                        int botResponseStartIndex = chatBox.TextLength;
                        foreach (var item in results)
                        {
                            chatBox.AppendText(item + Environment.NewLine + Environment.NewLine); // Add extra line for readability
                        }
                        // Apply highlighting for bot response (results)
                        chatBox.Select(botResponseStartIndex, chatBox.TextLength - botResponseStartIndex);
                        chatBox.SelectionBackColor = Color.LightYellow;
                    }
                    else
                    {
                        chatBox.AppendText("Bot: Sorry, I couldn't find any unique results." + Environment.NewLine);
                    }
                }
                else
                {
                    chatBox.AppendText("Bot: Error loading results." + Environment.NewLine);
                }
                // set the current caret position to the end
                chatBox.SelectionStart = chatBox.Text.Length;
                // scroll it automatically
                chatBox.ScrollToCaret();
                // Highlight the "You: " and "Bot: " labels
                ColorText(chatBox, "You: ", Color.Green);
                ColorText(chatBox, "Bot: ", Color.Red);
            };
            // Start navigation to the search query
            webBrowser.Navigate(url);
            return "Searching Google for your query...";
        }
        // Handle the LinkClicked event
        private void richTextBoxChat_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.LinkText) { UseShellExecute = true });
        }
        // This method colors specific text in the RichTextBox
        private void ColorText(RichTextBox richTextBox, string textToColor, Color color)
        {
            // Save the current selection
            int selectionStart = richTextBox.SelectionStart;
            int selectionLength = richTextBox.SelectionLength;
            // Find the text to color
            int startIndex = 0;
            while ((startIndex = richTextBox.Text.IndexOf(textToColor, startIndex)) != -1)
            {
                // Select the text to color
                richTextBox.SelectionStart = startIndex;
                richTextBox.SelectionLength = textToColor.Length;
                // Set the color
                richTextBox.SelectionColor = color;
                // Move to the next occurrence
                startIndex += textToColor.Length;
            }
            // Restore the original selection
            richTextBox.SelectionStart = selectionStart;
            richTextBox.SelectionLength = selectionLength;
            richTextBox.SelectionColor = richTextBox.ForeColor;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Get the graphics object
            Graphics g = e.Graphics;
            // Set smoothing mode for better quality
            g.SmoothingMode = SmoothingMode.AntiAlias;
            // Define the fill color
            Color fillColor = Color.LightBlue;
            int borderRadius = 5;
            // Create a brush with the desired color
            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            {
                // Iterate through all controls in the form
                foreach (Control ctrl in this.Controls)
                {
                    // Get the rectangle bounds of the control
                    Rectangle rect = ctrl.Bounds;
                    // Inflate the rectangle slightly to cover the area behind the control
                    rect.Inflate(5, 5);
                    // Create a GraphicsPath with rounded corners
                    using (GraphicsPath path = GetRoundedRectanglePath(rect, borderRadius))
                    {
                        if (ctrl is TextBox || ctrl is RichTextBox || ctrl is Button)
                            // Fill the rounded rectangle behind the control
                            g.FillPath(fillBrush, path);
                    }
                    if (ctrl is RichTextBox)
                    {
                        rect.Inflate(-5, -5);
                        using (GraphicsPath path = GetRoundedRectanglePath(rect, borderRadius))
                        {
                            // Fill the rounded rectangle behind the control
                            g.FillPath(new SolidBrush(Color.FromArgb(234, 238, 242)), path);
                        }
                        // Load and draw an image at the center of the control
                        Image image = Chatbot.Properties.Resources.pngegg__9_; // Use your image resource
                        // Resize the image
                        Bitmap newImage = new Bitmap(image, ClientSize.Width / 4, ClientSize.Width / 4);
                        if (newImage != null)
                        {
                            // Set opacity (0.0f = fully transparent, 1.0f = fully opaque)
                            float opacity = 0.2f; // Adjust this value as needed
                            // Create a color matrix and set the opacity
                            ColorMatrix matrix = new ColorMatrix
                            {
                                Matrix33 = opacity // Set the alpha value for the entire image
                            };
                            // Create image attributes and set the color matrix
                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            // Calculate the position to center the image
                            int x = (this.ClientSize.Width - newImage.Width) / 2;
                            int y = (this.ClientSize.Height - newImage.Height) / 2;
                            // Draw the image with the specified opacity
                            Rectangle destRect = new Rectangle(x, y, newImage.Width, newImage.Height);
                            e.Graphics.DrawImage(newImage, destRect, 0, 0, newImage.Width, newImage.Height, GraphicsUnit.Pixel, attributes);
                        }
                    }
                }
            }
            // Define border properties
            int borderWidth = 2; // Width of the border
            Color borderColor = Color.Aqua; // Color of the border
                                            // Create a Pen with the specified color and width
            using (Pen borderPen = new Pen(borderColor, borderWidth))
            {
                // Draw the border around the form
                e.Graphics.DrawRectangle(borderPen, 1, 1, this.ClientSize.Width - borderWidth, this.ClientSize.Height - borderWidth);
            }
        }
        // Helper method to create a GraphicsPath with rounded corners
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            // Add rounded corners
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90); // Top-left
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90); // Top-right
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90); // Bottom-right
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90); // Bottom-left
            // Close the path
            path.CloseFigure();
            return path;
        }
        private void ClearChatButton_Click(object sender, EventArgs e)
        {
            chatBox.Text = "";
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.youtube.com/@Amro-Omran") { UseShellExecute = true });
        }
    }
}
