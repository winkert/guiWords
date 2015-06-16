using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            this.txt_Query.Focus();
            using (SqlConnection connect = new SqlConnection(con))
            {
                Console.WriteLine(connect.ConnectionTimeout);
                SqlCommand command = new SqlCommand(@"select top 1 * from tWordForms", connect);
                connect.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader[0]);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    reader.Close();
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
        public static String con = "Data Source=zpxjdd8j5t.database.windows.net;Initial Catalog=guiWords;Integrated Security=False;User ID=winkert;Password=12Rimmer!;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False";
        //public static String con = "Data Source=mssql2.worldplanethosting.com;Initial Catalog=winkert_guiWords;Integrated Security=False;User ID=winkert_winkert;Password=ViaPecuniae;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
#endregion
#region Button Events
        //Quit button
        private void btn_Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Search button
        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            //Set Cursor
            this.Cursor = Cursors.Wait;
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
            String query = txt_Query.Text.Replace(", ", ",");
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
            String regI = "([^aeiou])?(i)([aeiouv])";
            String regJ = "(j)([aeiouv])";
            String regU = "([^jaeio])?(u)([aeijou])";
            String regV = "(v)([aeijou])";
            String regR = "([aeiouvj])(r)([a-z])";
            String regB = "([aeiouvj])(b)([a-z])";
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
            rSLine.Inlines.Add(new Run(String.Join(", ", qTerms)) { FontWeight = FontWeights.Bold });
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
                noResults.Text = "No results were found for the following: " + (char)10 + String.Join(", ", qTerms);
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
            this.Cursor = Cursors.Arrow;
            resetUI();
        }
        //Load history
        private void btn_History_Click(object sender, RoutedEventArgs e)
        {
            //Set Cursor
            this.Cursor = Cursors.Wait;
            ResultGrid.Children.Clear();
            for (int i = 0; i < searchHistory.Count(); i++)
            {
                BuildControls(searchHistory[i]);
            }
            this.Cursor = Cursors.Arrow;
            resetUI();
        }
        //Enter key
        private void txt_Query_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InitiateSearch search = btn_Search_Click;
                search.Invoke(sender, e); 
            }
        }
        //Open Perseus website
        private void btn_OpenPerseus(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            String form = b.Tag.ToString();
            System.Diagnostics.Process.Start("http://www.perseus.tufts.edu/hopper/morph?l=" + form + "&la=la");
        }
        //Open all forms window
        private void btn_ViewAllForms(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            int d_id = int.Parse(b.Tag.ToString());
            AllForms f = new AllForms(d_id);
            f.Show();
        }
#endregion
#region Public Methods
        //Public Methods
        public qHistory SearchForms(List<string> qTerms, String q)
        {
            string query = string.Join(",", qTerms);
            qHistory s = new qHistory(q);
            try
            {
                using (guiWordsDBMDataContext gWord = new guiWordsDBMDataContext(con))
                {
                    List<FormsView> d = gWord.sp_guiWords_Parse(query).ToList();
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
                String Error = er.Message;
                //String Trace = er.StackTrace;
                String Inner = "";
                if (er.InnerException != null)
                {
                    Inner = er.InnerException.Message;
                }
                MessageBox.Show("Exception: " + (char)10 + Error + (char)10 + "Inner Exception: " + (char)10 + Inner /*+ (char)10 + "Stack Trace: " + (char)10 + Trace*/);
                this.Cursor = Cursors.Arrow;
            }
            //return the result set
            return s;
        }
        public void BuildControls(qHistory s)
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
                //All Forms button
                wAllForms.Content = "All Forms";
                wAllForms.Width = 150;
                wAllForms.Tag = s.dWordIDs[i];
                wAllForms.Click += btn_ViewAllForms;
                //Perseus button
                wPerseus.Content = "Perseus Entry";
                wPerseus.Width = 150;
                wPerseus.Tag = s.dForms[i];
                wPerseus.Click += btn_OpenPerseus;
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
                //Result set text
                wSet.Margin = tMargins;
                wSet.FontSize = 18;
                wSet.FontFamily = fHeader;
                wSet.Header = s.dWords[i] + (char)9 + s.pPart[i] + (char)9 + s.pConj[i] + s.pDecl[i];
                wSet.HorizontalAlignment = HorizontalAlignment.Left;
                wSet.BorderThickness = tBorder;
                wSet.BorderBrush = new SolidColorBrush(Colors.LightSteelBlue);
                wSet.IsExpanded = false;
                wResults.Margin = tMargins;
                wResults.Background = null;
                #endregion
                #region Insert Controls
                //Need to add these before the loop
                //Otherwise they end up below the results
                wResults.Children.Add(wMeaning);
                Grid.SetColumn(wAllForms, 1);
                wLinkGrid.Children.Add(wAllForms);
                Grid.SetColumn(wPerseus, 2);
                wLinkGrid.Children.Add(wPerseus);
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
                        String parsing = s.dForms[j];
                        parsing = addParsing(parsing, s.pPerson[j]);
                        parsing = addParsing(parsing, s.pCase[j]);
                        parsing = addParsing(parsing, s.pNumber[j]);
                        parsing = addParsing(parsing, s.pGender[j]);
                        parsing = addParsing(parsing, s.pTense[j]);
                        parsing = addParsing(parsing, s.pMood[j]);
                        parsing = addParsing(parsing, s.pVoice[j]);
                        wLine.Text = parsing;
                        wResults.Children.Add(wLine);
                    }
                }
                wSet.Content = wResults;
                #endregion
                ResultGrid.Children.Add(wSet);
                #endregion
            }
        }
        public String addParsing(String p, String info)
        {
            char block = (char)9;
            if (info.Length > 0)
            {
                p += block + info;
            }
            return p;
        }
        public void resetUI()
        {
            this.txt_Query.Text = String.Empty;
            this.txt_Query.Focus();
        }
#endregion
    }
}
