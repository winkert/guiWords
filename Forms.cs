using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace guiWords
{
    
    public class Forms
    {
        #region Constructors
        public Forms()
        {

        }
        public Forms(List<FormsView> forms)
        {
            partofspeech = forms[0].part_Name;
            foreach (FormsView f in forms)
            {
                words.Add(f.wf_Form);
                cases.Add(f.nc_Name);
                numbers.Add(f.num_Name);
                genders.Add(f.ge_Name);
                persons.Add(f.vp_Name);
                tenses.Add(f.vt_Name);
                moods.Add(f.vm_Name);
                voices.Add(f.vv_Name);
                count++;
            }
        }
        #endregion
        #region Variables
        public String partofspeech;
        public List<String> words = new List<String>();
        public List<String> cases = new List<String>();
        public List<String> numbers = new List<String>();
        public List<String> genders = new List<String>();
        public List<String> persons = new List<String>();
        public List<String> tenses = new List<String>();
        public List<String> moods = new List<String>();
        public List<String> voices = new List<String>();
        public int count = 0;
        #endregion
        #region Public Functions
        public void addForm(Forms f, int i)
        {
            words.Add(f.words[i]);
            cases.Add(f.cases[i]);
            numbers.Add(f.numbers[i]);
            genders.Add(f.genders[i]);
            persons.Add(f.persons[i]);
            tenses.Add(f.tenses[i]);
            moods.Add(f.moods[i]);
            voices.Add(f.voices[i]);
            count++;
        }
        public void addForm(String w, String c, String n, String g, String p, String t, String m, String v)
        {
            words.Add(w);
            cases.Add(c);
            numbers.Add(n);
            genders.Add(g);
            persons.Add(p);
            tenses.Add(t);
            moods.Add(m);
            voices.Add(v);
            count++;
        }
        #endregion
    }

    public static class ChangeForms
    {
        public static Forms pickForms(Forms f, String filter, String criteria)
        {
            Forms lessForms = new Forms();
            switch (filter)
            {
                //case "Case":
                //    lessForms = splitOnCase(f, criteria);
                //    break;
                case "Number":
                    lessForms = splitOnNumber(f, criteria);
                    break;
                case "Gender":
                    lessForms = splitOnGender(f, criteria);
                    break;
                //case "Person":
                //    lessForms = splitOnPerson(f, criteria);
                //    break;
                case "Tense":
                    lessForms = splitOnTense(f, criteria);
                    break;
                case "Mood":
                    lessForms = splitOnMood(f, criteria);
                    break;
                case "Voice":
                    lessForms = splitOnVoice(f, criteria);
                    break;
                default:
                    break;
            }
            return lessForms;
        }
        #region Splitting Functions
        //public static Forms splitOnCase(Forms f, String criteria)
        //{
        //    Forms forms = new Forms();
        //    for (int i = 0; i < f.count; i++)
        //    {
        //        if (f.cases[i] == criteria)
        //        {
        //            forms.addForm(f, i);
        //        }
        //    }
        //    return forms;
        //}
        public static Forms splitOnNumber(Forms f, String criteria)
        {
            Forms forms = new Forms();
            for (int i = 0; i < f.count; i++)
            {
                if (f.numbers[i] == criteria)
                {
                    forms.addForm(f, i);
                }
            }
            return forms;
        }
        public static Forms splitOnGender(Forms f, String criteria)
        {
            Forms forms = new Forms();
            for (int i = 0; i < f.count; i++)
            {
                if (f.genders[i] == criteria)
                {
                    forms.addForm(f, i);
                }
            }
            return forms;
        }
        //public static Forms splitOnPerson(Forms f, String criteria)
        //{
        //    Forms forms = new Forms();
        //    for (int i = 0; i < f.count; i++)
        //    {
        //        if (f.persons[i] == criteria)
        //        {
        //            forms.addForm(f, i);
        //        }
        //    }
        //    return forms;
        //}
        public static Forms splitOnTense(Forms f, String criteria)
        {
            Forms forms = new Forms();
            for (int i = 0; i < f.count; i++)
            {
                if (f.tenses[i] == criteria)
                {
                    forms.addForm(f, i);
                }
            }
            return forms;
        }
        public static Forms splitOnMood(Forms f, String criteria)
        {
            Forms forms = new Forms();
            for (int i = 0; i < f.count; i++)
            {
                if (f.moods[i] == criteria)
                {
                    forms.addForm(f, i);
                }
            }
            return forms;
        }
        public static Forms splitOnVoice(Forms f, String criteria)
        {
            Forms forms = new Forms();
            for (int i = 0; i < f.count; i++)
            {
                if (f.voices[i] == criteria)
                {
                    forms.addForm(f, i);
                }
            }
            return forms;
        }
        #endregion
        public static int countFormsLike(List<String> forms, String filter)
        {
            int count = 0;
            foreach (String f in forms)
            {
                if (f == filter)
                {
                    count++;
                }
            }
            return count;
        }
        public static int countFormSets(List<String> forms)
        {
            int count = 0;
            List<String> distinctForms = (List<String>)forms.Distinct();
            count = distinctForms.Count;
            return count;
        }
    }
}

