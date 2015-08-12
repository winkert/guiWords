using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace guiWords
{
    /// <summary>
    /// Individual Form of a word with all information stored in a way which can be sorted and split up.
    /// </summary>
    public class Form
    {
        public Form()
        {

        }
        public Form(FormsView form)
        {
            wForm = form.d_Word;
            wPartOfSpeech = form.part_Name.ParseEnum<PartsOfSpeech>();
            wCase = form.nc_Name.ParseEnum<Cases>();
            wNumber = form.num_Name.ParseEnum<Numbers>();
            wGender = form.ge_Name.ParseEnum<Genders>();
            wPerson = form.vp_Name.ParseEnum<Persons>();
            wTense = form.vt_Name.ParseEnum<Tenses>();
            wMood = form.vm_Name.ParseEnum<Moods>();
            wVoice = form.vv_Name.ParseEnum<Voices>();
        }
        #region Fields
        string wForm;
        PartsOfSpeech wPartOfSpeech;
        Cases wCase;
        Numbers wNumber;
        Genders wGender;
        Persons wPerson;
        Tenses wTense;
        Moods wMood;
        Voices wVoice;
        #endregion


    }
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
        public string partofspeech;
        public List<string> words = new List<string>();
        public List<string> cases = new List<string>();
        public List<string> numbers = new List<string>();
        public List<string> genders = new List<string>();
        public List<string> persons = new List<string>();
        public List<string> tenses = new List<string>();
        public List<string> moods = new List<string>();
        public List<string> voices = new List<string>();
        public PartsOfSpeech wPartOfSpeech = new PartsOfSpeech();
        public List<Cases> wCases = new List<Cases>();
        public List<Numbers> wNumbers = new List<Numbers>();
        public List<Genders> wGenders = new List<Genders>();
        public List<Persons> wPersons = new List<Persons>();
        public List<Tenses> wTenses = new List<Tenses>();
        public List<Moods> wMoods = new List<Moods>();
        public List<Voices> wVoices = new List<Voices>();
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
        public void addForm(string w, string c, string n, string g, string p, string t, string m, string v)
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
    #region Static Classes
    public static class FormsExtensions
    {
        public static Forms pickForms(this Forms f, string filter, string criteria)
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
        public static Forms splitOnNumber(this Forms f, Numbers n)
        {
            Forms forms = new Forms();
            for (int i = 0; i < f.count; i++)
            {
                if (f.wNumbers[i] == n)
                {
                    forms.addForm(f, i);
                }
            }
            return forms;
        }
        public static Forms splitOnNumber(Forms f, string criteria)
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
        public static Forms splitOnGender(Forms f, string criteria)
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
        public static Forms splitOnTense(Forms f, string criteria)
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
        public static Forms splitOnMood(Forms f, string criteria)
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
        public static Forms splitOnVoice(Forms f, string criteria)
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
        public static int countFormsLike(List<string> forms, string filter)
        {
            int count = 0;
            foreach (string f in forms)
            {
                if (f == filter)
                {
                    count++;
                }
            }
            return count;
        }
        public static int countFormSets(List<string> forms)
        {
            int count = 0;
            List<string> distinctForms = (List<string>)forms.Distinct();
            count = distinctForms.Count;
            return count;
        }
    }
    public static class EnumExtension
    {
        #region Enum Methods
        /// <summary>
        /// Returns the Description attribute of an enum value if that attribute exists. Otherwise, it returns the name.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns>String</returns>
        public static string GetDescription(this Enum enumValue)
        {
            object[] attr = enumValue.GetType().GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attr.Length > 0
               ? ((DescriptionAttribute)attr[0]).Description
               : enumValue.ToString();
        }
        /// <summary>
        /// Returns an Enum with the description of the string. Otherwise it returns an Enum with the name of the string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stringVal"></param>
        /// <param name="defaultValue"></param>
        /// <returns>Enum</returns>
        public static T ParseEnum<T>(this string stringVal)
        {
            Type type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            System.Reflection.MemberInfo[] fields = type.GetFields();
            foreach (var field in fields)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0 && attributes[0].Description == stringVal)
                {
                    return (T)Enum.Parse(typeof(T), field.Name);
                }
            }
            return (T)Enum.Parse(typeof(T), stringVal);
        }
        #endregion
    }
    public static class OtherExtensions
    {
        public static string addParsing(this string p, string info)
        {
            char block = (char)9;
            if (info.Length > 0)
            {
                p += block + info;
            }
            return p;
        }
    }
    #endregion
    #region Components
    public enum PartsOfSpeech
    {
        None = 0,
        V,
        N,
        ADJ,
        NUM,
        PRON,
        ADV,
        INTERJ,
        CONJ
    }
    public enum Persons
    {
        None = 0,
        [Description("1")]
        First = 1,
        [Description("2")]
        Second,
        [Description("3")]
        Third
    }
    public enum Tenses
    {
        None = 0,
        PRES,
        FUT,
        IMPF,
        PERF,
        PLUP,
        FUTP
    }
    public enum Voices
    {
        None = 0,
        ACTIVE,
        PASSIVE
    }
    public enum Moods
    {
        None = 0,
        IND,
        INF,
        IMP,
        SUP,
        PPL

    }
    public enum Numbers
    {
        X = 0,
        S,
        P
    }
    public enum Cases
    {
        X = 0,
        NOM,
        GEN,
        DAT,
        ACC,
        ABL,
        LOC,
        VOC
    }
    public enum Genders
    {
        X= 0,
        M,
        F,
        N,
        C
    }
    #endregion
}

