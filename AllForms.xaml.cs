using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            this.Title = "All Forms of " + word;
            buildGrid(partCode);
        }
        #endregion
        #region Publics
        public List<FormsView> forms;
        public String word;
        public String partCode;
        public FontFamily fForms = new FontFamily("Palatino Linotype");
        public FontFamily fHead = new FontFamily("Palatino Linotype Bold");
        public GridLength fGridLength = new GridLength(200);
        #endregion
        #region Button Events
        #endregion
        #region Public Methods
        public List<FormsView> limitForms(List<FormsView> forms, String mood, String tense)
        {
            List<FormsView> filteredForms = new List<FormsView>();
            foreach (FormsView form in forms)
            {
                if (form.vm_Name == mood && form.vt_Name == tense)
                {
                    filteredForms.Add(form);
                }
            }
            return filteredForms;
        }
        public void fillPanel(ref StackPanel panel, List<String> form, List<String> type, String other = "")
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
                AllForms = gWord.sp_AllForms(d).ToList();
                word = AllForms[0].d_Word;
                partCode = AllForms[0].part_Name;
            }
            return AllForms;
        }
        public TextBlock createHeader (String text)
        {
            TextBlock header = new TextBlock();
            header.Text = text;
            header.FontFamily = fHead;
            header.FontSize = 18;
            return header;
        }
        public void fillGrid(ref Grid g, List<FormsView> forms, int column, int row, String headerTitle)
        {
            StackPanel singular = new StackPanel();
            StackPanel plural = new StackPanel();
            List<String> singularForms = new List<String>();
            List<String> singularTypes = new List<String>();
            List<String> pluralForms = new List<String>();
            List<String> pluralTypes = new List<String>();
            for (int i = 0; i < g.ColumnDefinitions.Count; i++)
            {
                g.ColumnDefinitions[i].Width = fGridLength;
            }
            foreach (FormsView form in forms)
            {
                if (form.num_Name == "S")
                {
                    singularForms.Add(form.wf_Form);
                    if (form.nc_Name != String.Empty || form.nc_Name.Length > 1)
                    {
                        singularTypes.Add(form.nc_Name); 
                        
                    }
                    else
                    {
                        singularTypes.Add(form.vp_Name); 
                    }
                }
                else
                {
                    pluralForms.Add(form.wf_Form);
                    if (form.nc_Name != String.Empty || form.nc_Name.Length > 1)
                    {
                        pluralTypes.Add(form.nc_Name);
                    }
                    else
                    {
                        pluralTypes.Add(form.vp_Name);
                    }
                }
            }
            if (headerTitle != "")
            {
                singular.Children.Add(createHeader(headerTitle)); 
            }
            singular.Children.Add(createHeader("Singular"));
            plural.Children.Add(createHeader("Plural"));
            fillPanel(ref singular, singularForms, singularTypes);
            fillPanel(ref plural, pluralForms, pluralTypes);
            Grid.SetColumn(singular, column);
            Grid.SetRow(singular, row);
            g.Children.Add(singular);
            Grid.SetColumn(plural, column+1);
            Grid.SetRow(plural, row);
            g.Children.Add(plural);
        }
        public void buildGrid(String pos)
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
                    //For Nouns:
                    //      The whole list is passed. There is only one couplet.
                    #region Noun
                    #region Components for Nouns
                    Grid nounGrid = new Grid();
                    #endregion
                    #region Build Grid
                    nounGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    nounGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    nounGrid.RowDefinitions.Add(new RowDefinition());
                    nounGrid.ColumnDefinitions[0].Width = fGridLength;
                    nounGrid.ColumnDefinitions[0].Width = fGridLength;
                    #endregion
                    #region Add Components
                    fillGrid(ref nounGrid, forms, 0, 0, "");
                    this.grd_AllForms.Children.Add(nounGrid);
                    #endregion
                    break;
                    #endregion
                case "V":
                    //For Verbs:
                    //      There are (6 * 2) + (4 * 2) + (2 + (3 * 3)) sets of forms (31) if I don't count Infinitives and Imperatives
                    //      Expander or Tab control?
                    //      
                    //      FIRST: Get all the Moods, Tenses, and Voices into a list
                    //      SECOND: Loop through rows and columns passing sets limited by LINQ to the fillGrid
                    //      THIRD: ?????
                    //      FOURTH: Profit!
                    #region Verb
                    #region Components for Verbs
                    Expander vIndicative = new Expander();
                    Expander vSubjunctive = new Expander();
                    Expander vParticiple = new Expander();
                    Expander vInfinitive = new Expander();
                    Expander vImperative = new Expander();
                    StackPanel verbs = new StackPanel();
                    Grid vIndGrid = new Grid();
                    Grid vSubGrid = new Grid();
                    Grid vParGrid = new Grid();
                    Grid vInfGrid = new Grid();
                    Grid vImpGrid = new Grid();
                    //Create lists to join to the main list
                    List<String> vMoods = new List<String> { "IND", "SUB", "INF", "IMP", "PPL", "SUPINE" };
                    List<String> vTenses = new List<String> { "PRES", "IMPERF", "FUT", "PERF", "PLUP", "FUTP" };
                    List<String> vVoices = new List<String> { "ACTIVE", "PASSIVE" };
                    #endregion
                    #region Build Grids
                    //Indicative
                    vIndGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vIndGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vIndGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vIndGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vIndGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vIndGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vIndGrid.RowDefinitions.Add(new RowDefinition());
                    vIndGrid.RowDefinitions.Add(new RowDefinition());
                    vIndGrid.RowDefinitions.Add(new RowDefinition());
                    vIndGrid.RowDefinitions.Add(new RowDefinition());
                    //Subjunctive
                    vSubGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vSubGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vSubGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vSubGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vSubGrid.RowDefinitions.Add(new RowDefinition());
                    vSubGrid.RowDefinitions.Add(new RowDefinition());
                    vSubGrid.RowDefinitions.Add(new RowDefinition());
                    vSubGrid.RowDefinitions.Add(new RowDefinition());
                    //Infinitive
                    //Pass Tense as other
                    vInfGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vInfGrid.RowDefinitions.Add(new RowDefinition());
                    //Imperative
                    //Pass Tense as other
                    vImpGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vImpGrid.RowDefinitions.Add(new RowDefinition());
                    //Participle
                    //Pass Case
                    vParGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vParGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vParGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vParGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    vParGrid.RowDefinitions.Add(new RowDefinition());
                    vParGrid.RowDefinitions.Add(new RowDefinition());
                    #endregion
                    #region Fill Grids
                    //TODO: There needs to be a way to break up the main list into its parts more perfectly.
                    //      There are some serious issues with this current code and I have not been able to figure it out
                    for (int i = 0; i < vIndGrid.RowDefinitions.Count; i++)
                    {
                        for (int j = 0; j < vIndGrid.ColumnDefinitions.Count; j +=2)
                        {
                            fillGrid(ref vIndGrid, limitForms(forms,"IND", "PERF"), j, i,"");
                        }
                    }
                    #endregion
                    #region Design Sets
                    vIndicative.Header = "Indicative";
                    vIndicative.Content = vIndGrid;
                    vSubjunctive.Header = "Subjunctive";
                    vSubjunctive.Content = vSubGrid;
                    vParticiple.Header = "Participles";
                    vParticiple.Content = vParGrid;
                    vInfinitive.Header = "Infinitives";
                    vInfinitive.Content = vInfGrid;
                    vImperative.Header = "Imperatives";
                    vImperative.Content = vImpGrid;
                    #endregion
                    #region Add Components
                    verbs.Children.Add(vIndicative);
                    verbs.Children.Add(vSubjunctive);
                    verbs.Children.Add(vParticiple);
                    verbs.Children.Add(vInfinitive);
                    verbs.Children.Add(vImperative);
                    Grid.SetColumn(verbs, 0);
                    Grid.SetRow(verbs, 0);
                    this.grd_AllForms.Children.Add(verbs);
                    #endregion
                    break;
                    #endregion
                case "ADJ":
                    break;
                case "NUM":
                    break;
                case "PRON":
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
