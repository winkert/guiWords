#define DEBUG
//#undef DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace guiWords
{
    /// <summary>
    /// Interaction logic for AllForms.xaml
    /// </summary>
    public partial class AllForms : Window
    {
        #region Constructors
        public AllForms()
        {
            InitializeComponent();
        }
        public AllForms(int d_ID)
        {
            InitializeComponent();
            forms = getForms(d_ID);
            //eachForm = getForms(d_ID);
            word = forms[0].wf_Form;
            //remove
            partCode = forms[0].part_Name;
            wPartCode = forms[0].part_Name.ParseEnum<PartsOfSpeech>();
            //Remove
            allForms = new Forms(forms);
            buildGrid(partCode);
            Title = "All Forms of " + word;
        }
        #endregion
        #region Publics
        private List<FormsView> forms;
        private List<Form> eachForm;
        //Remove
        public Forms allForms;
        public string word;
        private PartsOfSpeech wPartCode;
        //Remove
        public string partCode;
        public FontFamily fForms = new FontFamily("Palatino Linotype");
        public FontFamily fHead = new FontFamily("Palatino Linotype Bold");
        public GridLength fGridLength = new GridLength(200);
        #endregion
        #region Public Methods
        public void setGridWidth(ref Grid g)
        {
            for (int i = 0; i < g.ColumnDefinitions.Count; i++)
            {
                g.ColumnDefinitions[i].Width = fGridLength;
            }
        }
        public void fillPanel(ref StackPanel panel, List<string> form, List<string> type, string other = "")
        {
            for (int i = 0; i < form.Count; i++)
            {
                TextBlock line = new TextBlock();
                line.FontFamily = fForms;
                line.FontSize = 16;
                line.Padding = new Thickness(5,1,5,1);
                line.Text = type[i] + " " + other + " :   " + form[i];
                panel.Children.Add(line);
            }
        }
        public List<FormsView> getForms(int d)
        {
            List<FormsView> AllForms;
            using (guiWordsDBMDataContext gWord = new guiWordsDBMDataContext(MainWindow.con))
            {
                #if DEBUG
                AllForms = gWord.sp_AllForms(d).ToList();
                #else
                AllForms = gWord.winkert_sp_AllForms(d).ToList();
                #endif
            }
            return AllForms;
        }
        /// <summary>
        /// Get Forms from Database and put them into a list of Forms
        /// </summary>
        /// <param name="d">d_ID</param>
        /// <returns>List of Forms</returns>
        //public List<Form> getForms(int d)
        //{
        //    List<FormsView> AllForms;
        //    using (guiWordsDBMDataContext gWord = new guiWordsDBMDataContext(MainWindow.con))
        //    {
        //        AllForms = gWord.sp_AllForms(d).ToList();
        //    }
        //    List<Form> forms = new List<Form>();
        //    foreach (FormsView form in AllForms)
        //    {
        //        forms.Add(new Form(form));
        //    }
        //    return forms;
        //}
        public TextBlock createHeader (string text)
        {
            TextBlock header = new TextBlock();
            header.Text = text;
            header.FontFamily = fHead;
            header.FontSize = 18;
            return header;
        }
        //Verb Functions
        #region All Verb Forms
        public Expander buildVerbs(Forms forms, string mainHeader)
        {
            Expander verbSet = new Expander();
            verbSet.Header = mainHeader;
            StackPanel holder = new StackPanel();
            //1. Split into voices
            //2. For each voice, create a grid in the expander/stackpanel
            List<Forms> verbSets = new List<Forms>();
            verbSets.Add(splitVerbByVoice(forms, "ACTIVE"));
            verbSets.Add(splitVerbByVoice(forms, "PASSIVE"));
            foreach (Forms f in verbSets)
            {
                List<string> tensesInVerb = f.tenses.Distinct().ToList();
                int numTenses = tensesInVerb.Count;
                Grid g = new Grid();
                for (int i = 0; i < numTenses; i++)
                {
                    g.ColumnDefinitions.Add(new ColumnDefinition());
                }
                setGridWidth(ref g);
                g.RowDefinitions.Add(new RowDefinition());
                g.RowDefinitions.Add(new RowDefinition());

                splitVerbByTense(f, ref g);

                holder.Children.Add(g);
            }

            //Add the grids to the panel and return it the expander
            
            verbSet.Content = holder;
            return verbSet;
        }
        public Forms splitVerbByVoice(Forms forms, string voice)
        {
            return forms.pickForms( "Voice", voice);
        }
        public void splitVerbByTense(Forms forms, ref Grid set)
        {
            List<Forms> formsByTense = new List<Forms>();
            //Add each set of forms according to mood
            formsByTense.Add(forms.pickForms( "Tense", "PRES"));
            formsByTense.Add(forms.pickForms( "Tense", "IMPF"));
            formsByTense.Add(forms.pickForms( "Tense", "FUT"));
            formsByTense.Add(forms.pickForms( "Tense", "PERF"));
            formsByTense.Add(forms.pickForms( "Tense", "PLUP"));
            formsByTense.Add(forms.pickForms( "Tense", "FUTP"));
            //int maxi = g.ColumnDefinitions.Count * 2
            //int i = 0
            //i+=2
            //IF(i>ROUNDUP(maxi/2,0)-1,1,0)
            //IF(i>ROUNDUP(maxi/2,0)-1,i-(ROUNDUP(maxi/2,0)),i)
            //row2 = row
            //column2 = column + 1
            int maxI = set.ColumnDefinitions.Count * 2;
            int f = (int)Math.Round((double)maxI / 2, MidpointRounding.AwayFromZero) - 1;
            for (int i = 0; i < maxI; i+=2)
            {
                StackPanel singular = splitVerbByNumber(formsByTense[f], "Singular", formsByTense[f].numbers);
                StackPanel plural = splitVerbByNumber(formsByTense[f], "Plural", formsByTense[f].numbers);

                //Add the panels to the grid based on the number
                //This sets it up to be a two rows by 6 columns
                int column;
                int row;
                if (i > f-1)
                {
                    column = i - f;
                    row = 1;
                }
                else
                {
                    column = i;
                    row = 0;
                }

                //Put it all together
                Grid.SetColumn(singular, column);
                Grid.SetRow(singular, row);
                set.Children.Add(singular);
                Grid.SetColumn(plural, column + 1);
                Grid.SetRow(plural, row);
                set.Children.Add(plural);

            }
        }
        public StackPanel splitVerbByNumber(Forms forms, string numberHeader, List<string> criteria)
        {
            StackPanel verbSet = new StackPanel();
            Forms splitForms = forms.pickForms( "Number", criteria[0]);
            if (criteria[0] != "X")
            {
                TextBlock header = new TextBlock();
                header.Text = numberHeader;
                header.FontFamily = fHead;
                verbSet.Children.Add(header); 
            }
            for (int i = 0; i < splitForms.words.Count; i++)
            {
                TextBlock wordset = new TextBlock();
                wordset.Text = splitForms.persons[i] + " : " + splitForms.words[i];
                verbSet.Children.Add(wordset);
            }
            return verbSet;
        }
        #endregion
        //New functions to deal with Forms
        #region UIDesign Functions
        public void displayNouns(ref Grid g)
        {
            Forms singularForms = allForms.pickForms("Number", "S");
            Forms pluralForms = allForms.pickForms("Number", "P");
            StackPanel sgPanel = new StackPanel();
            StackPanel plPanel = new StackPanel();
            sgPanel.Children.Add(createHeader("Singular"));
            plPanel.Children.Add(createHeader("Plural"));
            fillPanel(ref sgPanel, singularForms.words, singularForms.cases);
            fillPanel(ref plPanel, pluralForms.words, pluralForms.cases);
            Grid.SetColumn(sgPanel, 0);
            Grid.SetColumn(plPanel, 1);
            g.Children.Add(sgPanel);
            g.Children.Add(plPanel);
        }
        public void displayVerbs(ref Grid g)
        {
            StackPanel verbPanel = new StackPanel();
            List<Forms> formsByMood = new List<Forms>();
            //Add each set of forms according to mood
            formsByMood.Add(allForms.pickForms("Mood", "IND"));
            formsByMood.Add(allForms.pickForms("Mood", "INF"));
            formsByMood.Add(allForms.pickForms("Mood", "IMP"));
            formsByMood.Add(allForms.pickForms("Mood", "SUB"));
            formsByMood.Add(allForms.pickForms("Mood", "PPL"));
            //for each mood, pass into a function which will create the expander set
            for (int i = 0; i < formsByMood.Count; i++ )
            {
                Forms f = formsByMood[i];
                if (f.words.Count > 0)
                {
                    Expander verbSet = buildVerbs(f, f.moods[0]);
                    verbPanel.Children.Add(verbSet); 
                }
            }
            Grid.SetColumn(verbPanel, 0);
            Grid.SetRow(verbPanel, 0);
            g.Children.Add(verbPanel);
        }
        public void displayAdjectives(ref Grid g)
        {

        }
        public void displayNumbers(ref Grid g)
        {

        }
        public void displayPronouns(ref Grid g)
        {

        }
        public void displayAdverbs(ref Grid g)
        {

        }
        public void displaySingleForms(ref Grid g)
        {

        }
        #endregion
        //Main build function called by instatiation
        //public void buildGrid(PartsOfSpeech pos)
        public void buildGrid(string pos)
        {
            switch(pos)
            {
                    //For each part of speech the following happens:
                    //      A new list of forms is produced for each grid couplet (S/P)
                    //      That list and the grid for this word is passed into the function fillGrid
                    //      From there, the list of forms is put into two list of strings and passed
                    //      into the function fillPanel
                    //      This is done for each type of word (Gender, Tense, Mood, Voice, etc)
                    //      Each case has a different kind of loop which passes different list of forms
                case "N":
                //case PartsOfSpeech.N:
                    //For Nouns:
                    //      This is the simple one.
                    #region Noun
                    #region Components for Nouns
                    Grid nounGrid = new Grid();
                    #endregion
                    #region Build Grid
                    nounGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    nounGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    nounGrid.RowDefinitions.Add(new RowDefinition());
                    setGridWidth(ref nounGrid);
                    #endregion
                    #region Add Components
                    displayNouns(ref nounGrid);
                    grd_AllForms.Children.Add(nounGrid);
                    #endregion
                    break;
                    #endregion
                case "V":
                //case PartsOfSpeech.V:
                    //For Verbs:
                    //      There are (6 * 2) + (4 * 2) + (2 + (3 * 3)) sets of forms (31) if I don't count Infinitives and Imperatives
                    //      Uses Expanders to separate each mood
                    #region Verb
                    #region Components for Verbs
                    Grid verbGrid = new Grid();
                    #endregion
                    #region Build Grid
                    verbGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    verbGrid.RowDefinitions.Add(new RowDefinition());
                    #endregion
                    #region Add Components
                    displayVerbs(ref verbGrid);
                    grd_AllForms.Children.Add(verbGrid);
                    #endregion
                    break;
                    #endregion
                case "ADJ":
                //case PartsOfSpeech.ADJ:
                    break;
                case "NUM":
                //case PartsOfSpeech.NUM:
                    break;
                case "PRON":
                //case PartsOfSpeech.PRON:
                    break;
                case "ADV":
                //case PartsOfSpeech.ADV:
                    break;
                default:
                    //Everything else is really simple. They are a single form.
                    break;
            }
        }
        #endregion
    }
}