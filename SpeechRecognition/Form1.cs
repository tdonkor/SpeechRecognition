using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.IO;
using System.Data.SQLite;
using System.Speech.Synthesis;

namespace SpeechRecognition
{
    public partial class Form1 : Form
    {
        Database databaseObj = new Database();
        List<string> order = new List<string>();
        SpeechSynthesizer synth = new SpeechSynthesizer();

        public Form1()
        {
            InitializeComponent();
            synth.Speak("Starting the speech recognition application. Choose from the menu list.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        // Create a simple handler for the SpeechRecognized event.
        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            order.Add(e.Result.ToString());

            MessageBox.Show("Item added to order: " + e.Result.Text);
            synth.Speak("The Item has been recognized as being in the database");
        }

        // Create a simple handler for the SpeechRecognized event.
        private void sre_SpeechRecognizedRejected(object sender, SpeechRecognizedEventArgs e)
        {
            synth.Speak("The Item was not recognized");
            MessageBox.Show("Item not recognized: " + e.Result.Text);
        }

        private void btnAddWord_Click(object sender, EventArgs e)
        {
           
            //insert into Database
             
             databaseObj.InsertToDatabase();
        }

        private void btnSpeech_Click(object sender, EventArgs e)
        {
            

            try
            {
                // Create a new SpeechRecognitionEngine instance.
                SpeechRecognizer recognizer = new SpeechRecognizer();

                // Create a simple grammar that recognizes the database items 
                Choices words = new Choices();

                words.Add(databaseObj.LoadFromDatabase().ToArray());

                // Create a GrammarBuilder object and append the Choices object.
                GrammarBuilder gb = new GrammarBuilder();
                gb.Append(words);

                // Create the Grammar instance and load it into the speech recognition engine.
                Grammar g = new Grammar(gb);
                recognizer.LoadGrammar(g);

                // Register a handler for the SpeechRecognized event.
                recognizer.SpeechRecognized +=
                  new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            }
            catch (Exception ex)
            {
                synth.Speak("An error occured");
                MessageBox.Show($"Error: {ex}");
            }
           

        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            // Output the order 
            string orderList = string.Empty;

            foreach(string str in order)
            {
                orderList += str + ".\n";
            }

            if (order.Count > 0)
            {
                MessageBox.Show(orderList);
            }
            else
            {
                MessageBox.Show("Your order list is empty");
                synth.Speak("Your order list is empty");
            }
                

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            synth.Speak("Exiting the application");
            Application.Exit();
        }
    }
}
