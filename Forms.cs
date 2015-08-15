﻿using System;
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
            wForm = form.wf_Form;
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
        public string wForm;
        PartsOfSpeech wPartOfSpeech;
        public Cases wCase;
        public Numbers wNumber;
        public Genders wGender;
        public Persons wPerson;
        public Tenses wTense;
        public Moods wMood;
        public Voices wVoice;
        #endregion
        #region Methods
        public string getFormType()
        {
            switch(wPartOfSpeech)
            {
                case PartsOfSpeech.ADJ:
                case PartsOfSpeech.N:
                case PartsOfSpeech.NUM:
                case PartsOfSpeech.PRON:
                    return wCase.GetDescription();
                case PartsOfSpeech.V:
                    if(wPerson.GetDescription() == "None" || wPerson.GetDescription() == "X")
                    {
                        return wCase.GetDescription();
                    }
                    else
                    {
                        return wPerson.GetDescription();
                    }
                default:
                    return string.Empty;
            }
        }
        #endregion
    }
    #region Static Classes
    public static class FormsExtensions
    {
        /// <summary>
        /// Universal function which takes the criteria selected and produces only the forms of the word which match that criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The Enum criteria</param>
        /// <returns>new List</returns>
        public static List<Form> SplitBy<T>(this List<Form> forms, T type)
        {
            List<Form> newList = new List<Form>();
            if(type is Numbers)
            {
                newList = forms.Where(f => f.wNumber.Equals(type)).Distinct().ToList();
            }
            if(type is Genders)
            {
                newList = forms.Where(f => f.wGender.Equals(type)).Distinct().ToList();
            }
            if (type is Tenses)
            {
                newList = forms.Where(f => f.wTense.Equals(type)).Distinct().ToList();
            }
            if (type is Moods)
            {
                newList = forms.Where(f => f.wMood.Equals(type)).Distinct().ToList();
            }
            if (type is Voices)
            {
                newList = forms.Where(f => f.wVoice.Equals(type)).Distinct().ToList();
            }
            return newList;
        }
        /// <summary>
        /// Universal function which takes the criteria selected and produces only the forms of the word which match that criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The Enum criteria as an array</param>
        /// <returns>new List</returns>
        public static List<Form> SplitBy<T>(this List<Form> forms, List<T> types)
        {
            List<Form> newList = new List<Form>();
            if (types is List<Numbers>)
            {
                newList = forms.Where(f => types.Contains((T)(object)f.wNumber)).Distinct().ToList();
            }
            if (types is List<Genders>)
            {
                newList = forms.Where(f => types.Contains((T)(object)f.wGender)).Distinct().ToList();
            }
            if (types is List<Tenses>)
            {
                newList = forms.Where(f => types.Contains((T)(object)f.wTense)).Distinct().ToList();
            }
            if (types is List<Moods>)
            {
                newList = forms.Where(f => types.Contains((T)(object)f.wMood)).Distinct().ToList();
            }
            if (types is List<Voices>)
            {
                newList = forms.Where(f => types.Contains((T)(object)f.wVoice)).Distinct().ToList();
            }
            return newList;
        }
    }
    public static class EnumExtension
    {
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
        /// Returns an Enum with the description matching the string. Otherwise it returns an Enum with the name matching the string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stringVal"></param>
        /// <param name="defaultValue"></param>
        /// <returns>Enum</returns>
        public static T ParseEnum<T>(this string stringVal)
        {
            if(stringVal == null)
            {
                return (T)Enum.Parse(typeof(T), "None");
            }
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
        [Description("Verb")]
        V,
        [Description("Noun")]
        N,
        [Description("Adjective")]
        ADJ,
        [Description("Numeral")]
        NUM,
        [Description("Pronoun")]
        PRON,
        [Description("Adverb")]
        ADV,
        [Description("Interjection")]
        INTERJ,
        [Description("Conjunction")]
        CONJ,
        [Description("Preposition")]
        PREP
    }
    public enum Persons
    {
        None = 0,
        [Description("1")]
        First = 1,
        [Description("2")]
        Second,
        [Description("3")]
        Third,
        X
    }
    public enum Tenses
    {
        None = 0,
        PRES,
        FUT,
        IMPERF,
        PERF,
        PLUP,
        FUTP,
        ALL
    }
    public enum Voices
    {
        None = 0,
        ACTIVE,
        PASSIVE,
        ALL
    }
    public enum Moods
    {
        None = 0,
        IND,
        INF,
        IMP,
        SUB,
        PPL,
        SUPINE

    }
    public enum Numbers
    {
        None = 0,
        S,
        P,
        X
    }
    public enum Cases
    {
        None = 0,
        NOM,
        GEN,
        DAT,
        ACC,
        ABL,
        LOC,
        VOC,
        X
    }
    public enum Genders
    {
        None= 0,
        M,
        F,
        N,
        C,
        X
    }
    #endregion
}

