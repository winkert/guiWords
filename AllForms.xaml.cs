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
            eachForm = getFormsFromDatabase(d_ID);
            word = eachForm[0].wWord;
            wPartCode = eachForm[0].wPartOfSpeech;
            buildAllForms(eachForm, wPartCode);
            Title = "All Forms of " + word;
        }
        #endregion
        #region Publics
        private List<Form> eachForm;
        public string word;
        private PartsOfSpeech wPartCode;
        public FontFamily fForms = new FontFamily("Palatino Linotype");
        public FontFamily fHead = new FontFamily("Palatino Linotype Bold");
        
        #endregion
        #region Methods
        /// <summary>
        /// Get Forms from Database and put them into a list of Forms
        /// </summary>
        /// <param name="d">d_ID</param>
        /// <returns>List of Forms</returns>
        private List<Form> getFormsFromDatabase(int d)
        {
            List<FormsView> AllForms;
            using (guiWordsDBMDataContext gWord = new guiWordsDBMDataContext(MainWindow.con))
            {
                AllForms = gWord.sp_AllForms(d).ToList();
            }
            List<Form> forms = new List<Form>();
            foreach (FormsView form in AllForms)
            {
                forms.Add(new Form(form));
            }
            return forms;
        }
        /// <summary>
        /// Takes the form and part of speech and produce a grid of forms ordered logically.
        /// </summary>
        /// <param name="forms">List of Form objects</param>
        private void buildAllForms(List<Form> forms, PartsOfSpeech pos)
        {
            /// Step 1: Determine the part of speech
            /// Step 2: Split up the list into each logical set
            ///             Nouns: Number
            ///             Adjectives, Pronouns, Numbers: Gender, Number
            ///             Verbs: Voice, Mood, Tense, Number
            /// Step 3: Create an expander
            /// Step 4: Split up the grid in the expander based on needs
            /// Step 5: For Verbs use multiple expanders
            ///         multiply by number of distinct voices, moods, and numbers for nouns
            /// Step 6: Populate the grids/panels/expanders with the forms ordered out based on the breakdown
            switch (pos)
            {
                case PartsOfSpeech.N:
                case PartsOfSpeech.ADJ:
                case PartsOfSpeech.PRON:
                case PartsOfSpeech.NUM:
                    List<Form> sForms = forms.OrderBy(x => x.wCase).ToList();
                    Expander Set = createExpander(pos, word);
                    Set.Content = createGrid(forms, pos);
                    grd_AllForms.Children.Add(Set);
                    //Numbers
                    break;
                case PartsOfSpeech.V:
                    //Verbs
                    List<Form> vForms = forms.OrderBy(x => x.wPerson).ToList();
                    List<Tenses> PrimaryTenses = new List<Tenses>() { Tenses.PRES, Tenses.IMPERF, Tenses.FUT };
                    List<Tenses> SecondaryTenses = new List<Tenses>() { Tenses.PERF, Tenses.PLUP, Tenses.FUTP };
                    List<Moods> OtherMoods = new List<Moods>() { Moods.IMP, Moods.INF, Moods.SUPINE };
                    //Create expanders to hold the forms
                    Expander vPrimaryActiveIndicative = createExpander(word + " - Verb - Primary Active Indicative");
                    Expander vSecondaryActiveIndicative = createExpander(word + " - Verb - Secondary Active Indicative");
                    Expander vPrimaryPassiveIndicative = createExpander(word + " - Verb - Primary Passive Indicative");
                    //Expander vSecondaryPassiveIndicative = createExpander(word + " - Verb - Secondary Passive Indicative");
                    Expander vPrimaryActiveSubjunctive = createExpander(word + " - Verb - Primary Active Subjunctive");
                    Expander vSecondaryActiveSubjunctive = createExpander(word + " - Verb - Secondary Active Subjunctive");
                    Expander vPrimaryPassiveSubjunctive = createExpander(word + " - Verb - Primary Passive Subjunctive");
                    //Expander vSecondaryPassiveSubjunctive = createExpander(word + " - Verb - Secondary Passive Subjunctive");
                    Expander vPrimaryParticiples = createExpander(word + " - Verb - Primary Participles");
                    Expander vSecondaryParticiples = createExpander(word + " - Verb - Secondary Participles");
                    Expander vRemainingForms = createExpander(pos, word);
                    //Add content to the expanders
                    vPrimaryActiveIndicative.Content = createGrid(vForms.SplitBy(PrimaryTenses).SplitBy(Voices.ACTIVE).SplitBy(Moods.IND), pos);
                    vSecondaryActiveIndicative.Content = createGrid(vForms.SplitBy(SecondaryTenses).SplitBy(Voices.ACTIVE).SplitBy(Moods.IND), pos);
                    vPrimaryPassiveIndicative.Content = createGrid(vForms.SplitBy(PrimaryTenses).SplitBy(Voices.PASSIVE).SplitBy(Moods.IND), pos);
                    //vSecondaryPassiveIndicative.Content = createGrid(vForms.SplitBy(SecondaryTenses).SplitBy(Voices.PASSIVE).SplitBy(Moods.IND), pos);
                    vPrimaryActiveSubjunctive.Content = createGrid(vForms.SplitBy(PrimaryTenses).SplitBy(Voices.ACTIVE).SplitBy(Moods.SUB), pos);
                    vSecondaryActiveSubjunctive.Content = createGrid(vForms.SplitBy(SecondaryTenses).SplitBy(Voices.ACTIVE).SplitBy(Moods.SUB), pos);
                    vPrimaryPassiveSubjunctive.Content = createGrid(vForms.SplitBy(PrimaryTenses).SplitBy(Voices.PASSIVE).SplitBy(Moods.SUB), pos);
                    //vSecondaryPassiveSubjunctive.Content = createGrid(vForms.SplitBy(SecondaryTenses).SplitBy(Voices.PASSIVE).SplitBy(Moods.SUB), pos);
                    vPrimaryParticiples.Content = createGrid(vForms.SplitBy(PrimaryTenses).SplitBy(Moods.PPL), pos);
                    vSecondaryParticiples.Content = createGrid(vForms.SplitBy(SecondaryTenses).SplitBy(Moods.PPL), pos);
                    vRemainingForms.Content = createGrid(vForms.SplitBy(OtherMoods), pos);
                    //Add the expanders to the panel
                    grd_AllForms.Children.Add(vPrimaryActiveIndicative);
                    grd_AllForms.Children.Add(vSecondaryActiveIndicative);
                    grd_AllForms.Children.Add(vPrimaryPassiveIndicative);
                    //grd_AllForms.Children.Add(vSecondaryPassiveIndicative);
                    grd_AllForms.Children.Add(vPrimaryActiveSubjunctive);
                    grd_AllForms.Children.Add(vSecondaryActiveSubjunctive);
                    grd_AllForms.Children.Add(vPrimaryPassiveSubjunctive);
                    //grd_AllForms.Children.Add(vSecondaryPassiveSubjunctive);
                    grd_AllForms.Children.Add(vPrimaryParticiples);
                    grd_AllForms.Children.Add(vSecondaryParticiples);
                    grd_AllForms.Children.Add(vRemainingForms);
                    break;
                case PartsOfSpeech.None:
                    //Throw an error?
                    break;
                default:
                    //Single form.
                    Expander wForm = createExpander(pos, word);
                    wForm.Content = createGrid(forms, pos);
                    grd_AllForms.Children.Add(wForm);
                    break;
            }
            //Add Result to window

        }
        /// <summary>
        /// Creates a grid for the forms to be added to an expander.
        /// </summary>
        /// <param name="forms">The forms to be added to this grid</param>
        /// <param name="pos">Part of Speech</param>
        /// <returns>new Grid()</returns>
        private Grid createGrid(List<Form> forms, PartsOfSpeech pos)
        {
            if(forms.Count < 1)
            {
                return new Grid();
            }
            Grid formGrid = new Grid();
            switch (pos)
            {
                case PartsOfSpeech.N:
                    //Nouns
                    List<Form> nSingular = forms.SplitBy(Numbers.S);
                    List<Form> nPlural = forms.SplitBy(Numbers.P);
                    formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    formGrid.RowDefinitions.Add(new RowDefinition());
                    formGrid.RowDefinitions.Add(new RowDefinition());
                    formGrid.Children.Add(createHeader(forms[0].wGender.GetDescription(), 0));
                    formGrid.Children.Add(createFormList(nSingular, 0, 1));
                    formGrid.Children.Add(createFormList(nPlural, 1, 1));
                    break;
                case PartsOfSpeech.ADJ:
                case PartsOfSpeech.PRON:
                case PartsOfSpeech.NUM:
                    //Adjectives
                    //Pronouns
                    //Numbers
                    List<Form> aSingular = forms.SplitBy(Numbers.S);
                    List<Form> aPlural = forms.SplitBy(Numbers.P);
                    if (aSingular.SplitBy(Genders.C).Count > 0)
                    {
                        List<Form> SingularNeuter = aSingular.SplitBy(Genders.N);
                        List<Form> SingularCommon = aSingular.SplitBy(Genders.C);
                        List<Form> PluralNeuter = aPlural.SplitBy(Genders.N).SplitBy(Numbers.P);
                        List<Form> PluralCommon = aPlural.SplitBy(Genders.C).SplitBy(Numbers.P);
                        formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        formGrid.RowDefinitions.Add(new RowDefinition());
                        formGrid.RowDefinitions.Add(new RowDefinition());
                        formGrid.Children.Add(createHeader("Common", 0));
                        formGrid.Children.Add(createHeader("Neuter", 1));
                        formGrid.Children.Add(createHeader("Common", 2));
                        formGrid.Children.Add(createHeader("Neuter", 3));
                        formGrid.Children.Add(createFormList(SingularCommon, 0, 1));
                        formGrid.Children.Add(createFormList(SingularNeuter, 1, 1));
                        formGrid.Children.Add(createFormList(PluralCommon, 2, 1));
                        formGrid.Children.Add(createFormList(PluralNeuter, 3, 1));
                    }
                    else
                    {
                        List<Form> SingularMasculine = aSingular.SplitBy(Genders.M);
                        List<Form> SingularFeminine = aSingular.SplitBy(Genders.F);
                        List<Form> SingularNeuter = aSingular.SplitBy(Genders.N);
                        List<Form> PluralMasculine = aPlural.SplitBy(Genders.M);
                        List<Form> PluralFeminine = aPlural.SplitBy(Genders.F);
                        List<Form> PluralNeuter = aPlural.SplitBy(Genders.N);
                        formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                        formGrid.RowDefinitions.Add(new RowDefinition());
                        formGrid.RowDefinitions.Add(new RowDefinition());
                        formGrid.Children.Add(createHeader("Masculine", 0));
                        formGrid.Children.Add(createHeader("Neuter", 2));
                        formGrid.Children.Add(createHeader("Feminine", 4));
                        formGrid.Children.Add(createFormList(SingularMasculine, 0, 1));
                        formGrid.Children.Add(createFormList(SingularFeminine, 1, 1));
                        formGrid.Children.Add(createFormList(SingularNeuter, 2, 1));
                        formGrid.Children.Add(createFormList(PluralMasculine, 3, 1));
                        formGrid.Children.Add(createFormList(PluralFeminine, 4, 1));
                        formGrid.Children.Add(createFormList(PluralNeuter, 5, 1));
                    }
                    break;
                case PartsOfSpeech.V:
                    //Verbs
                    List<Form> vSingular = forms.SplitBy(Numbers.S);
                    List<Form> vPlural = forms.SplitBy(Numbers.P);
                    List<Form> vOther = forms.SplitBy(Numbers.X);
                    Moods vMood = forms[0].wMood;
                    Tenses vTense = forms[0].wTense;
                    switch(vMood)
                    {
                        case Moods.IND:
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.RowDefinitions.Add(new RowDefinition());
                            formGrid.RowDefinitions.Add(new RowDefinition());

                            if (vTense == Tenses.PRES || vTense == Tenses.FUT || vTense == Tenses.IMPERF)
                            {
                                formGrid.Children.Add(createHeader("Present", 0));
                                formGrid.Children.Add(createHeader("Imperfect", 2));
                                formGrid.Children.Add(createHeader("Future", 4));
                                //Primary Tenses
                                formGrid.Children.Add(createFormList(vSingular.SplitBy(Tenses.PRES), 0, 1));
                                formGrid.Children.Add(createFormList(vPlural.SplitBy(Tenses.PRES), 1, 1));
                                formGrid.Children.Add(createFormList(vSingular.SplitBy(Tenses.IMPERF), 2, 1));
                                formGrid.Children.Add(createFormList(vPlural.SplitBy(Tenses.IMPERF), 3, 1));
                                formGrid.Children.Add(createFormList(vSingular.SplitBy(Tenses.FUT), 4, 1));
                                formGrid.Children.Add(createFormList(vPlural.SplitBy(Tenses.FUT), 5, 1));
                            }
                            else
                            {
                                formGrid.Children.Add(createHeader("Perfect", 0));
                                formGrid.Children.Add(createHeader("Pluperfect", 2));
                                formGrid.Children.Add(createHeader("Future Perfect", 4));
                                //Secodnary Tenses
                                formGrid.Children.Add(createFormList(vSingular.SplitBy(Tenses.PERF), 0, 1));
                                formGrid.Children.Add(createFormList(vPlural.SplitBy(Tenses.PERF), 1, 1));
                                formGrid.Children.Add(createFormList(vSingular.SplitBy(Tenses.PLUP), 2, 1));
                                formGrid.Children.Add(createFormList(vPlural.SplitBy(Tenses.PLUP), 3, 1));
                                formGrid.Children.Add(createFormList(vSingular.SplitBy(Tenses.FUTP), 4, 1));
                                formGrid.Children.Add(createFormList(vPlural.SplitBy(Tenses.FUTP), 5, 1));
                            }
                            break;
                        case Moods.SUB:
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.RowDefinitions.Add(new RowDefinition());
                            formGrid.RowDefinitions.Add(new RowDefinition());
                            if (vTense == Tenses.PRES || vTense == Tenses.IMPERF)
                            {
                                formGrid.Children.Add(createHeader("Present", 0));
                                formGrid.Children.Add(createHeader("Imperfect", 2));
                                //Primary Tenses
                                formGrid.Children.Add(createFormList(vSingular.SplitBy(Tenses.PRES), 0, 1));
                                formGrid.Children.Add(createFormList(vPlural.SplitBy(Tenses.PRES), 1, 1));
                                formGrid.Children.Add(createFormList(vSingular.SplitBy(Tenses.IMPERF), 2, 1));
                                formGrid.Children.Add(createFormList(vPlural.SplitBy(Tenses.IMPERF), 3, 1));
                            }
                            else
                            {
                                formGrid.Children.Add(createHeader("Perfect", 0));
                                formGrid.Children.Add(createHeader("Pluperfect", 2));
                                //Secodnary Tenses
                                formGrid.Children.Add(createFormList(vSingular.SplitBy(Tenses.PERF), 0, 1));
                                formGrid.Children.Add(createFormList(vPlural.SplitBy(Tenses.PERF), 1, 1));
                                formGrid.Children.Add(createFormList(vSingular.SplitBy(Tenses.PLUP), 2, 1));
                                formGrid.Children.Add(createFormList(vPlural.SplitBy(Tenses.PLUP), 3, 1));
                            }
                            break;
                        case Moods.PPL:
                            List<Form> pSingular = forms.SplitBy(Numbers.S);
                            List<Form> pPlural = forms.SplitBy(Numbers.P);
                            if (pSingular.SplitBy(Genders.C).Count > 0)
                            {
                                List<Form> SingularNeuter = pSingular.SplitBy(Genders.N);
                                List<Form> SingularCommon = pSingular.SplitBy(Genders.C);
                                List<Form> PluralNeuter = pPlural.SplitBy(Genders.N).SplitBy(Numbers.P);
                                List<Form> PluralCommon = pPlural.SplitBy(Genders.C).SplitBy(Numbers.P);
                                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                formGrid.RowDefinitions.Add(new RowDefinition());
                                formGrid.RowDefinitions.Add(new RowDefinition());
                                formGrid.RowDefinitions.Add(new RowDefinition());
                                formGrid.RowDefinitions.Add(new RowDefinition());
                                formGrid.Children.Add(createHeader("Common - Present", 0));
                                formGrid.Children.Add(createHeader("Neuter - Present", 1));
                                formGrid.Children.Add(createHeader("Common - Present", 2));
                                formGrid.Children.Add(createHeader("Neuter - Present", 3));
                                formGrid.Children.Add(createHeader("Common - Future", 0, 2));
                                formGrid.Children.Add(createHeader("Neuter - Future", 1, 2));
                                formGrid.Children.Add(createHeader("Common - Future", 2, 2));
                                formGrid.Children.Add(createHeader("Neuter - Future", 3, 2));
                                formGrid.Children.Add(createFormList(SingularCommon.SplitBy(Tenses.PRES), 0, 1));
                                formGrid.Children.Add(createFormList(SingularNeuter.SplitBy(Tenses.PRES), 1, 1));
                                formGrid.Children.Add(createFormList(PluralCommon.SplitBy(Tenses.PRES), 2, 1));
                                formGrid.Children.Add(createFormList(PluralNeuter.SplitBy(Tenses.PRES), 3, 1));
                                formGrid.Children.Add(createFormList(SingularCommon.SplitBy(Tenses.FUT), 0, 3));
                                formGrid.Children.Add(createFormList(SingularNeuter.SplitBy(Tenses.FUT), 1, 3));
                                formGrid.Children.Add(createFormList(PluralCommon.SplitBy(Tenses.FUT), 2, 3));
                                formGrid.Children.Add(createFormList(PluralNeuter.SplitBy(Tenses.FUT), 3, 3));
                            }
                            else
                            {
                                List<Form> SingularMasculine = pSingular.SplitBy(Genders.M);
                                List<Form> SingularFeminine = pSingular.SplitBy(Genders.F);
                                List<Form> SingularNeuter = pSingular.SplitBy(Genders.N);
                                List<Form> PluralMasculine = pPlural.SplitBy(Genders.M);
                                List<Form> PluralFeminine = pPlural.SplitBy(Genders.F);
                                List<Form> PluralNeuter = pPlural.SplitBy(Genders.N);
                                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                formGrid.RowDefinitions.Add(new RowDefinition());
                                formGrid.RowDefinitions.Add(new RowDefinition());
                                formGrid.Children.Add(createHeader("Masculine", 0));
                                formGrid.Children.Add(createHeader("Neuter", 2));
                                formGrid.Children.Add(createHeader("Feminine", 4));
                                formGrid.Children.Add(createFormList(SingularMasculine, 0, 1));
                                formGrid.Children.Add(createFormList(SingularFeminine, 1, 1));
                                formGrid.Children.Add(createFormList(SingularNeuter, 2, 1));
                                formGrid.Children.Add(createFormList(PluralMasculine, 3, 1));
                                formGrid.Children.Add(createFormList(PluralFeminine, 4, 1));
                                formGrid.Children.Add(createFormList(PluralNeuter, 5, 1));
                            }
                            break;
                        default:
                            List<Form> vImperative = forms.SplitBy(Moods.IMP);
                            List<Form> vInfinitive = forms.SplitBy(Moods.INF);
                            List<Form> vSupine = forms.SplitBy(Moods.SUPINE);
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            formGrid.RowDefinitions.Add(new RowDefinition());
                            formGrid.RowDefinitions.Add(new RowDefinition());
                            formGrid.Children.Add(createHeader("Imperative", 0));
                            formGrid.Children.Add(createHeader("Infinitive", 2));
                            formGrid.Children.Add(createHeader("Supine", 3));
                            formGrid.Children.Add(createFormList(vImperative.SplitBy(Numbers.S), 0, 1));
                            formGrid.Children.Add(createFormList(vImperative.SplitBy(Numbers.P), 1, 1));
                            formGrid.Children.Add(createFormList(vInfinitive, 2, 1));
                            formGrid.Children.Add(createFormList(vSupine, 3, 1));
                            break;
                    }
                    break;
                case PartsOfSpeech.None:
                    //Throw an error?
                    break;
                default:
                    //Single form.
                    formGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    formGrid.RowDefinitions.Add(new RowDefinition());
                    formGrid.Children.Add(createFormList(forms));
                    break;
            }
            formGrid.Background = new SolidColorBrush(Colors.White);
            return formGrid;
        }
        private Expander defaultExpander()
        {
            Expander e = new Expander();
            e.FontFamily = new FontFamily("Palatino Linotype Bold");
            e.BorderBrush = new SolidColorBrush(Colors.LightSteelBlue);
            e.Background = new SolidColorBrush(Colors.LightGray);
            e.FontSize = 16;
            e.Width = 650;
            return e;
        }
        /// <summary>
        /// Creates an expander with a header correctly labeled and other consistent attributes.
        /// </summary>
        /// <param name="pos">Part of Speech</param>
        /// <param name="w">Word</param>
        /// <returns>new Expander()</returns>
        private Expander createExpander(PartsOfSpeech pos, string w)
        {
            Expander e = defaultExpander();
            e.Header = w + " - " + pos.GetDescription();
            return e;
        }
        /// <summary>
        /// Creates an expander with a header correctly labeled and other consistent attributes.
        /// </summary>
        /// <param name="head">Header text</param>
        /// <returns>new Expander()</returns>
        private Expander createExpander(string head)
        {
            Expander e = defaultExpander();
            e.Header = head;
            return e;
        }
        /// <summary>
        /// Creates a stackpanel with textblocks for each form
        /// </summary>
        /// <param name="forms">List of forms to be added to the stackpanel</param>
        /// <returns>new StackPanel()</returns>
        private StackPanel createFormList(List<Form> forms, int col = 0, int row = 0)
        {
            if(forms.Count < 1)
            {
                return new StackPanel();
            }
            StackPanel formStack = new StackPanel();
            TextBlock fHeader = new TextBlock();
            //Define fHeader
            fHeader.Text = forms[0].wNumber.GetDescription();
            fHeader.FontFamily = new FontFamily("Palatino Linotype Bold");
            fHeader.FontSize = 16;
            formStack.Children.Add(fHeader);
            foreach (Form f in forms)
            {
                TextBlock formText = new TextBlock();
                formText.FontFamily = new FontFamily("Palatino Linotype");
                formText.FontSize = 14;
                //formText.HorizontalAlignment = HorizontalAlignment.Center;
                formText.Text = f.getFormType() + " : " + f.wForm;
                formStack.Children.Add(formText);
            }
            Grid.SetColumn(formStack, col);
            Grid.SetRow(formStack, row);
            return formStack;
        }
        private TextBlock createHeader(string headText, int col, int row = 0)
        {
            TextBlock header = new TextBlock();
            header.FontFamily = new FontFamily("Palatino Linotype Bold");
            header.FontSize = 16;
            header.Text = headText;
            Grid.SetRow(header, row);
            Grid.SetColumn(header, col);
            return header;
        }
        #endregion
    }
}
