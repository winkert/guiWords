using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        }
        //Publics
        public FontFamily fHeader = new FontFamily("Palatino Linotype Bold");
        public FontFamily fResults = new FontFamily("Palatino Linotype");
        public Thickness tMargins = new Thickness(10);
        public Thickness tBorder = new Thickness(1);
        public Thickness noBorder = new Thickness(0);
        public List<qHistory> searchHistory = new List<qHistory>();
        public String con = "Data Source=mssql2.worldplanethosting.com;Initial Catalog=winkert_guiWords;Persist Security Info=True;User ID=winkert_winkert;Password=ViaPecuniae";
        //Buttons
        private void btn_Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            //Set Cursor
            this.Cursor = Cursors.Wait;
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
            //Variables and such
            List<string> qTerms = new List<string>();
            //Regular Expressions to deal with u/v and i/j spelling variations.
            String regI = "([^aeiou])?(i)([aeiouv])";
            String regJ = "(j)([aeiouv])";
            String regU = "([^jaeio])?(u)([aeijou])";
            String regV = "(v)([aeijou])";
            for (int i = 0; i < lquery.Count(); i++)
            {
                qTerms.Add(lquery[i]);
                qTerms.Add(Regex.Replace(Regex.Replace(lquery[i], regV, "u$2"), regI, "$1j$3"));
                qTerms.Add(Regex.Replace(Regex.Replace(lquery[i], regV, "u$2"), regJ, "i$2"));
                qTerms.Add(Regex.Replace(Regex.Replace(lquery[i], regU, "$1v$3"), regI, "$1j$3"));
                qTerms.Add(Regex.Replace(Regex.Replace(lquery[i], regU, "$1v$3"), regJ, "i$2"));
                qTerms = qTerms.Distinct().ToList();
            }
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
            rLine.FontFamily = fResults;
            rLine.Text = totForms + " Forms and " + totWords + " Words found.";
            rLine.Background = null;
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
                noResults.Text = "No results were found for the following: ";
                for (int n = 0; n < qTerms.Count(); n++)
                {
                    noResults.Text += (char)10 + qTerms[n];
                }
                noResults.FontFamily = fHeader;
                noResults.BorderThickness = noBorder;
                noResults.Foreground = new SolidColorBrush(Colors.Red);
                ResultGrid.Children.Add(noResults);
            }
            //add results to history
            searchHistory.Add(s);
            //Reset Cursor
            this.Cursor = Cursors.Arrow;
        }

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
        }

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
                String Trace = er.StackTrace;
                String Inner = "";
                if (er.InnerException != null)
                {
                    Inner = er.InnerException.Message;
                }
                MessageBox.Show("Exception: " + (char)10 + Error + (char)10 + "Inner Exception: " + (char)10 + Inner + (char)10 + "Stack Trace: " + (char)10 + Trace);
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
                Expander wSet = new Expander();
                StackPanel wResults = new StackPanel();
                TextBox wMeaning = new TextBox();
                wMeaning.Text = s.dMeanings[i];
                wMeaning.FontFamily = fHeader;
                wMeaning.BorderThickness = tBorder;
                wMeaning.Background = null;
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
                wResults.Children.Add(wMeaning);
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
                        String parsing = s.dForms[j] + (char)9 + s.pPerson[j] + s.pCase[j] + (char)9 + s.pNumber[j] + (char)9 + s.pGender[j] + (char)9 + s.pTense[j] + (char)9 + s.pMood[j] + (char)9 + s.pVoice[j];
                        char tab = '\u0009';
                        parsing = parsing.Replace(tab.ToString() + tab.ToString(), "");
                        wLine.Text = parsing;
                        wResults.Children.Add(wLine);
                    }
                }
                wSet.Content = wResults;
                ResultGrid.Children.Add(wSet);
            }
        }
    }
}
