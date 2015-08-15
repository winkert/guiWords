#define DEBUG
//#undef DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
namespace guiWords
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txt_Query.Focus();
            using (SqlConnection connect = new SqlConnection(con))
            {
                Console.WriteLine(connect.ConnectionTimeout);
                SqlCommand command = new SqlCommand(@"select top 1 * from tWordForms", connect);
                connect.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine(reader[0]);
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    
                }
                connect.Close();
            }
        }
        #region Publics
        public delegate void InitiateSearch(object o, RoutedEventArgs e);
        public FontFamily fHeader = new FontFamily("Palatino Linotype Bold");
        public FontFamily fResults = new FontFamily("Palatino Linotype");
        public Thickness tMargins = new Thickness(10);
        public Thickness tBorder = new Thickness(1);
        public Thickness noBorder = new Thickness(0);
        public List<qHistory> searchHistory = new List<qHistory>();
        #if DEBUG
        public static String con = "Data Source=SUPERCOMPUTER;Integrated Security=True;Connect Timeout=15;Encrypt=False;Initial Catalog=Words;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        #else
        public static string con = "Data Source=mssql2.worldplanethosting.com;Initial Catalog=winkert_guiWords;Integrated Security=False;User ID=winkert_winkert;Password=ViaPecuniae;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        #endif
        #endregion
        #region Event Handlers
        //Quit button
        private void btn_Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        ///<summary>Search button</summary>
        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            //Set Cursor
            Cursor = Cursors.Wait;
            #region Initial Checks
            //Check that txt_Query is not empty
            if (txt_Query.Text == "")
            {
                MessageBox.Show("Please enter a search term.");
                return;
            }
            //clear the previous results
            ResultGrid.Children.Clear();
            //Populated a list of the query term(s)
            string query = txt_Query.Text.Replace(", ", ",");
            List<string> lquery = query.Split(new Char[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            //Check the search history for the search term(s) before continuing
            for (int i = 0; i < searchHistory.Count(); i++)
            {
                if (searchHistory[i].isContained(lquery))
                {
                    BuildControls(searchHistory[i]);
                    return;
                }
            }
            #endregion
            //Variables and such
            #region String Manipulation
            List<string> qTerms = new List<string>();
            //Regular Expressions to deal with u/v and i/j spelling variations.
            string regI = "([^aeiou])?(i)([aeiouv])";
            string regJ = "(j)([aeiouv])";
            string regU = "([^jaeio])?(u)([aeijou])";
            string regV = "(v)([aeijou])";
            string regR = "([aeiouvj])(r)([a-z])";
            string regB = "([aeiouvj])(b)([a-z])";
            for (int i = 0; i < lquery.Count(); i++)
            {
                qTerms.Add(lquery[i]);
                qTerms.Add(Regex.Replace(Regex.Replace(lquery[i], regV, "u$2"), regI, "$1j$3"));
                qTerms.Add(Regex.Replace(Regex.Replace(lquery[i], regV, "u$2"), regJ, "i$2"));
                qTerms.Add(Regex.Replace(Regex.Replace(lquery[i], regU, "$1v$3"), regI, "$1j$3"));
                qTerms.Add(Regex.Replace(Regex.Replace(lquery[i], regU, "$1v$3"), regJ, "i$2"));
                qTerms.Add(Regex.Replace(lquery[i], regR, "$1rr$3"));
                qTerms.Add(Regex.Replace(lquery[i], regB, "$1bb$3"));
                qTerms = qTerms.Distinct().ToList();
            }
            #endregion
            #region Search and Build
            //Run the search and collect the results
            int totWords = 0;
            int totForms = 0;
            qHistory s = new qHistory();
            s = SearchForms(qTerms, query);

            //Tally the number of words
            totWords = s.dWordIDs.Count();
            totForms = s.dForms.Count();

            //Populate the Screen
            TextBlock rLine = new TextBlock();
            TextBlock rSLine = new TextBlock();
            rSLine.FontFamily = fResults;
            rSLine.Background = null;
            rSLine.FontSize = 16;
            rLine.FontFamily = fResults;
            rLine.Background = null;
            rLine.FontSize = 14;
            rSLine.Inlines.Add("Searched for: ");
            rSLine.Inlines.Add(new Run(string.Join(", ", qTerms)) { FontWeight = FontWeights.Bold });
            rLine.Text = totForms + " Forms and " + totWords + " Words found.";
            ResultGrid.Children.Add(rSLine);
            ResultGrid.Children.Add(rLine);

            //Main Loop
            if (totWords > 0)
            {
                //Only create controls and results if there are results to show.
                BuildControls(s);
            }
            else
            {
                TextBox noResults = new TextBox();
                noResults.Text = "No results were found for the following: " + (char)10 + string.Join(", ", qTerms);
                noResults.FontFamily = fHeader;
                noResults.FontSize = 20;
                noResults.BorderThickness = noBorder;
                noResults.Foreground = new SolidColorBrush(Colors.Red);
                ResultGrid.Children.Add(noResults);
            }
            
            //add results to history
            searchHistory.Add(s);
            #endregion
            //Reset Cursor
            Cursor = Cursors.Arrow;
            resetUI();
        }
        ///<summary>Load history</summary>
        private void btn_History_Click(object sender, RoutedEventArgs e)
        {
            //Set Cursor
            Cursor = Cursors.Wait;
            ResultGrid.Children.Clear();
            for (int i = 0; i < searchHistory.Count(); i++)
            {
                BuildControls(searchHistory[i]);
            }
            Cursor = Cursors.Arrow;
            resetUI();
        }
        ///<summary>Enter key</summary>
        private void txt_Query_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InitiateSearch search = btn_Search_Click;
                search.Invoke(sender, e); 
            }
        }
        ///<summary>Open Perseus website</summary>
        private void btn_OpenPerseus(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string form = b.Tag.ToString();
            System.Diagnostics.Process.Start("http://www.perseus.tufts.edu/hopper/morph?l=" + form + "&la=la");
        }
        ///<summary>Open all forms window</summary>
        private void btn_ViewAllForms(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Button b = sender as Button;
            int d_id = int.Parse(b.Tag.ToString());
            AllForms f = new AllForms(d_id);
            f.Show();
            Cursor = Cursors.Arrow;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Calls a stored procedure which searches for the word entered. Creates a history item for the search.
        /// </summary>
        /// <param name="qTerms">List of terms to search. This includes variant spellings</param>
        /// <param name="q">Query; passed into the history object</param>
        /// <returns>new qHistory(q)</returns>
        public qHistory SearchForms(List<string> qTerms, string q)
        {
            string query = string.Join(",", qTerms);
            qHistory s = new qHistory(q);
            try
            {
                using (guiWordsDBMDataContext gWord = new guiWordsDBMDataContext(con))
                {
                    #if DEBUG
                    List<FormsView> d = gWord.sp_guiWords_Parse(query).ToList();
                    #else
                    List<FormsView> d = gWord.winkert_sp_guiWords_Parse(query).ToList();
                    #endif
                    //List<FormsView> d = gWord.FormsViews.ToList();
                    //var r = from all in d
                    //        join f in qTerms on all.wf_Form equals f
                    //        select all;
                    //foreach(var item in r)
                    foreach (var item in d)
                    {
                        if (!s.dWordIDs.Contains((int)item.d_ID))
                        {
                            //Items added i times.
                            s.dWordIDs.Add((int)item.d_ID);
                            s.dWords.Add(item.d_Word);
                            s.dMeanings.Add(item.d_Meaning);
                            s.pPart.Add(item.part_Name + "");
                            s.pDecl.Add(item.nd_Name + "");
                            s.pConj.Add(item.vc_Name + "");
                        }
                        //Items added j times.
                        s.allIDs.Add((int)item.d_ID);
                        s.dForms.Add(item.wf_Form);
                        s.pPerson.Add(item.vp_Name + "");
                        s.pCase.Add(item.nc_Name + "");
                        s.pNumber.Add(item.num_Name + "");
                        s.pGender.Add(item.ge_Name + "");
                        s.pTense.Add(item.vt_Name + "");
                        s.pMood.Add(item.vm_Name + "");
                        s.pVoice.Add(item.vv_Name + "");
                    }

                }
            }
            catch (Exception er)
            {
                string Error = er.Message;
                //String Trace = er.StackTrace;
                string Inner = "";
                if (er.InnerException != null)
                {
                    Inner = er.InnerException.Message;
                }
                MessageBox.Show("Exception: " + (char)10 + Error + (char)10 + "Inner Exception: " + (char)10 + Inner /*+ (char)10 + "Stack Trace: " + (char)10 + Trace*/);
                Cursor = Cursors.Arrow;
            }
            //return the result set
            return s;
        }
        private void BuildControls(qHistory s)
        {
            int totWords = s.dWordIDs.Count();
            int totForms = s.dForms.Count();
            for (int i = 0; i < totWords; i++)
            {
                #region Declare Controls
                Expander wSet = new Expander();
                StackPanel wResults = new StackPanel();
                TextBox wMeaning = new TextBox();
                Grid wLinkGrid = new Grid();
                Button wAllForms = new Button();
                Button wPerseus = new Button();
                ColumnDefinition ncol = new ColumnDefinition();
                ColumnDefinition ncol2 = new ColumnDefinition();
                ColumnDefinition ncol3 = new ColumnDefinition();
                ColumnDefinition ncol4 = new ColumnDefinition();
                RowDefinition nrow = new RowDefinition();
                #endregion
                #region Define Controls
                //Meaning text
                wMeaning.Text = s.dMeanings[i];
                wMeaning.FontFamily = fHeader;
                wMeaning.BorderThickness = tBorder;
                wMeaning.Background = null;
                wMeaning.IsReadOnly = true;
                wMeaning.TextWrapping = TextWrapping.WrapWithOverflow;
                //Grid setup
                ncol.Width = new GridLength(150);
                ncol2.Width = new GridLength(150);
                ncol3.Width = new GridLength(150);
                ncol4.Width = new GridLength(150);
                nrow.Height = new GridLength(30);
                wLinkGrid.RowDefinitions.Add(nrow);
                wLinkGrid.ColumnDefinitions.Add(ncol);
                wLinkGrid.ColumnDefinitions.Add(ncol2);
                wLinkGrid.ColumnDefinitions.Add(ncol3);
                wLinkGrid.ColumnDefinitions.Add(ncol4);
                ///All Forms is an incomplete part of this application.
                /// I need to refactor the entire thing before any sort of release is possible.
                #region All Forms
                //All Forms button
                wAllForms.Content = "All Forms";
                wAllForms.Width = 150;
                wAllForms.Tag = s.dWordIDs[i];
                wAllForms.Click += btn_ViewAllForms;
                Grid.SetColumn(wAllForms, 1);
                wLinkGrid.Children.Add(wAllForms);
                #endregion
                //Perseus button
                wPerseus.Content = "Perseus Entry";
                wPerseus.Width = 150;
                wPerseus.Tag = s.dForms[i];
                wPerseus.Click += btn_OpenPerseus;
                Grid.SetColumn(wPerseus, 2);
                wLinkGrid.Children.Add(wPerseus);
                //Result set text
                wSet.Margin = tMargins;
                wSet.FontSize = 18;
                wSet.FontFamily = fHeader;
                wSet.Header = s.dWords[i] + (char)9 + s.pPart[i] + (char)9 + s.pConj[i] + s.pDecl[i];
                wSet.HorizontalAlignment = HorizontalAlignment.Left;
                wSet.Width = 600;
                wSet.BorderThickness = tBorder;
                wSet.BorderBrush = new SolidColorBrush(Colors.LightSteelBlue);
                wSet.IsExpanded = false;
                wResults.Margin = tMargins;
                wResults.Background = null;
                #endregion
                //Need to add these before the loop
                //Otherwise they end up below the results
                wResults.Children.Add(wMeaning);
                wResults.Children.Add(wLinkGrid);
                #region Build Results
                for (int j = 0; j < totForms; j++)
                {
                    if (s.dWordIDs[i] == s.allIDs[j])
                    {
                        TextBox wLine = new TextBox();
                        wLine.BorderThickness = noBorder;
                        wLine.Background = null;
                        wLine.FontSize = 16;
                        wLine.FontFamily = fResults;
                        wLine.IsReadOnly = true;
                        string parsing = s.dForms[j];
                        parsing = parsing.addParsing(s.pPerson[j]);
                        parsing = parsing.addParsing(s.pCase[j]);
                        parsing = parsing.addParsing(s.pNumber[j]);
                        parsing = parsing.addParsing(s.pGender[j]);
                        parsing = parsing.addParsing(s.pTense[j]);
                        parsing = parsing.addParsing(s.pMood[j]);
                        parsing = parsing.addParsing(s.pVoice[j]);
                        wLine.Text = parsing;
                        wResults.Children.Add(wLine);
                    }
                }
                wSet.Content = wResults;
                #endregion
                ResultGrid.Children.Add(wSet);
            }
        }
        private void resetUI()
        {
            txt_Query.Text = string.Empty;
            txt_Query.Focus();
        }
        #endregion
    }
}
