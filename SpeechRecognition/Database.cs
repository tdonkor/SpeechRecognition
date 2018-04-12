using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;
using System.Speech.Synthesis;

namespace SpeechRecognition
{
    class Database
    {
        SQLiteConnection myConnection;
        SpeechSynthesizer synth = new SpeechSynthesizer();


        /// <summary>
        /// constructor
        /// </summary>
        public Database()
        {
            //initialise the database connection 
            myConnection = new SQLiteConnection("Data Source = C:\\sqlite\\words.db");

            if (!File.Exists(@"C:\sqlite\words.db"))
            {
                synth.Speak("No database is available. Exiting the application");
                MessageBox.Show("No database available- Exiting the program");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Allows the user to insert a new word into the database
        /// </summary>
        public void InsertToDatabase()
        {
            string promptValue = ShowDialog("Item", "Input Item");

            if (string.IsNullOrEmpty(promptValue))
            {
                synth.Speak("The word can't be empty or NULL");
                MessageBox.Show("Word can't be empty or Null");
                return;
            }

            //insert the value into Database
            string query = "INSERT INTO word ('item') VALUES (@item)";

            //create a command and add the query and connection as parameters
            SQLiteCommand myCommand = new SQLiteCommand(query, myConnection);

            OpenConnection();
            myCommand.Parameters.AddWithValue("@item", promptValue);

            var result = myCommand.ExecuteNonQuery();
            
            CloseConnection();
            synth.Speak("Your new word was added to the database successfully");
            MessageBox.Show($"Rows Added: {result}");
        }

        /// <summary>
        /// Loads all the words from the database 
        /// into a list
        /// </summary>
        /// <returns></returns>
        public List<string>  LoadFromDatabase()
        {
            List<string> words = new List<string>();
            OpenConnection();

            //read from the word database
            string query = "SELECT  * from  word";

            synth.Speak("All the words downloaded from the words database");

            SQLiteCommand myCommand = new SQLiteCommand(query, myConnection);
            SQLiteDataReader dataReader = myCommand.ExecuteReader();

            //Add the words from the database into the 
            //list
            while (dataReader.Read())
            {
                words.Add(dataReader[0].ToString());
            }

            CloseConnection();

            return words;
        }

        /// <summary>
        /// Open the connection
        /// </summary>
        public void OpenConnection()
        {

            if (myConnection.State != System.Data.ConnectionState.Open)
            {
              
                synth.Speak("The database connection has been opened");
                myConnection.Open();
            }
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void CloseConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                synth.Speak("The database connection has been closed");
                myConnection.Close();
            }
        }

        /// <summary>
        /// Show the input dialog box for the item input
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public string ShowDialog(string text, string caption)
        {
            synth.Speak("Add a new word into the words database");
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
